using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;
using SportEvents.Core.Errors;

namespace SportEvents.Core.Services;

/// <summary>
/// Виконує прикладні сценарії оформлення заявок та розрахунку реєстраційних внесків.
/// </summary>
public sealed class RegistrationService
{
    private static readonly Action<ILogger, int, int, Exception?> _logStartingPlace =
        LoggerMessage.Define<int, int>(
            LogLevel.Debug,
            new EventId(1001, nameof(Place)),
            "Початок розрахунку та оформлення заявки {RegistrationId} учасника {ParticipantId}");

    private static readonly Action<ILogger, int, decimal, Exception?> _logPlacedSuccess =
        LoggerMessage.Define<int, decimal>(
            LogLevel.Information,
            new EventId(1002, nameof(Place)),
            "Заявку {RegistrationId} успішно зареєстровано на суму {Amount}");

    private static readonly Action<ILogger, int, RegistrationStatus, Exception?> _logCancelRejected =
        LoggerMessage.Define<int, RegistrationStatus>(
            LogLevel.Warning,
            new EventId(1003, nameof(Cancel)),
            "Відхилено скасування заявки {RegistrationId} у неприпустимому стані {Status}");

    private static readonly Action<ILogger, int, Exception?> _logStorageFailure =
        LoggerMessage.Define<int>(
            LogLevel.Error,
            new EventId(1004, nameof(Place)),
            "Критичний збій збереження заявки {RegistrationId} у сховище");

    private readonly IRegistrationRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;
    private readonly ILogger<RegistrationService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistrationService"/> class.
    /// Ініціалізує новий екземпляр сервісу заявок із впровадженням залежностей та логера.
    /// </summary>
    /// <param name="repository">Сховище заявок.</param>
    /// <param name="pricing">Політика розрахунку вартості позицій.</param>
    /// <param name="notifier">Сервіс сповіщення учасників.</param>
    /// <param name="logger">Структурований журнал подій сервісу.</param>
    /// <exception cref="ArgumentNullException">Одна із залежностей дорівнює null.</exception>
    public RegistrationService(
        IRegistrationRepository repository,
        IPricingPolicy pricing,
        INotifier notifier,
        ILogger<RegistrationService>? logger = null)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        _logger = logger ?? NullLogger<RegistrationService>.Instance;
    }

    /// <summary>
    /// Реєструє нову заявку у сховищі, обчислює внесок і надсилає сповіщення.
    /// </summary>
    /// <param name="registration">Заявка учасника на змагання.</param>
    /// <returns>Підсумкова сума внеску у гривнях.</returns>
    /// <exception cref="ArgumentNullException">Заявка дорівнює null.</exception>
    /// <exception cref="InvalidOperationException">У заявці відсутні обрані дисципліни.</exception>
    public decimal Place(Registration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);

        string correlationId = Guid.NewGuid().ToString("N")[..12];
        using IDisposable? scope = _logger.BeginScope(
            new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["CorrelationId"] = correlationId,
                ["RegistrationId"] = registration.Id,
            });

        _logStartingPlace(_logger, registration.Id, registration.ParticipantId, null);

        if (registration.Items.Count == 0)
        {
            throw new InvalidOperationException("Неможливо оформити порожню заявку без позицій.");
        }

        decimal total = TotalOf(registration);
        try
        {
            _repository.Add(registration);
        }
        catch (StorageException ex)
        {
            _logStorageFailure(_logger, registration.Id, ex);
            throw;
        }

        _notifier.Notify(registration, total);
        _logPlacedSuccess(_logger, registration.Id, total, null);
        return total;
    }

    /// <summary>
    /// Скасовує заявку за її ідентифікатором із перевіркою доменних правил.
    /// </summary>
    /// <param name="registrationId">Унікальний ідентифікатор заявки.</param>
    /// <exception cref="DomainRuleException">Заявку не знайдено або її стан не дозволяє скасування.</exception>
    public void Cancel(int registrationId)
    {
        using IDisposable? scope = _logger.BeginScope(
            new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["RegistrationId"] = registrationId,
            });

        Registration registration = _repository.GetById(registrationId)
            ?? throw new DomainRuleException(
                "registration.exists",
                $"Заявку з ідентифікатором {registrationId} не знайдено.");

        if (registration.Status != RegistrationStatus.New)
        {
            _logCancelRejected(_logger, registrationId, registration.Status, null);
            throw new DomainRuleException(
                "registration.cancellable",
                $"Заявку #{registrationId} у стані {registration.Status} неможливо скасувати.");
        }

        registration.Cancel();
    }

    /// <summary>
    /// Обчислює підсумкову суму заявки згідно з поточною політикою ціноутворення.
    /// </summary>
    /// <param name="registration">Заявка учасника.</param>
    /// <returns>Підсумкова вартість у гривнях.</returns>
    public decimal TotalOf(Registration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        decimal total = 0m;
        foreach (RegistrationItem item in registration.Items)
        {
            total += _pricing.PriceOf(item);
        }

        return total;
    }
}
