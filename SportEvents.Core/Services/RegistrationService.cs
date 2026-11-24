using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;
using SportEvents.Core.Errors;

namespace SportEvents.Core.Services;

/// <summary>
/// Виконує прикладні сценарії оформлення заявок та розрахунку реєстраційних внесків.
/// </summary>
public sealed class RegistrationService
{
    private readonly IRegistrationRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistrationService"/> class.
    /// Ініціалізує новий екземпляр сервісу заявок із впровадженням залежностей.
    /// </summary>
    /// <param name="repository">Сховище заявок.</param>
    /// <param name="pricing">Політика розрахунку вартості позицій.</param>
    /// <param name="notifier">Сервіс сповіщення учасників.</param>
    /// <exception cref="ArgumentNullException">Одна із залежностей дорівнює null.</exception>
    public RegistrationService(
        IRegistrationRepository repository,
        IPricingPolicy pricing,
        INotifier notifier)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
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
        if (registration.Items.Count == 0)
        {
            throw new InvalidOperationException("Неможливо оформити порожню заявку без позицій.");
        }

        decimal total = TotalOf(registration);
        _repository.Add(registration);
        _notifier.Notify(registration, total);
        return total;
    }

    /// <summary>
    /// Скасовує заявку за її ідентифікатором із перевіркою доменних правил.
    /// </summary>
    /// <param name="registrationId">Унікальний ідентифікатор заявки.</param>
    /// <exception cref="DomainRuleException">Заявку не знайдено або її стан не дозволяє скасування.</exception>
    public void Cancel(int registrationId)
    {
        Registration registration = _repository.GetById(registrationId)
            ?? throw new DomainRuleException(
                "registration.exists",
                $"Заявку з ідентифікатором {registrationId} не знайдено.");

        if (registration.Status != RegistrationStatus.New)
        {
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
