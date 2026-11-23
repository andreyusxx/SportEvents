using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;

namespace SportEvents.Core.Services;

/// <summary>
/// Виконує прикладні сценарії оформлення заявок та розрахунку реєстраційних внесків.
/// </summary>
public sealed class RegistrationService
{
    private readonly IRegistrationRepository _repository;
    private readonly IPricingPolicy _pricing;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistrationService"/> class.
    /// Ініціалізує новий екземпляр сервісу заявок із впровадженням залежностей.
    /// </summary>
    /// <param name="repository">Сховище заявок.</param>
    /// <param name="pricing">Політика розрахунку вартості позицій.</param>
    /// <exception cref="ArgumentNullException">Одна із залежностей дорівнює null.</exception>
    public RegistrationService(IRegistrationRepository repository, IPricingPolicy pricing)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
    }

    /// <summary>
    /// Реєструє нову заявку у сховищі після перевірки наявності позицій.
    /// </summary>
    /// <param name="registration">Заявка учасника на змагання.</param>
    /// <exception cref="ArgumentNullException">Заявка дорівнює null.</exception>
    /// <exception cref="InvalidOperationException">У заявці відсутні обрані дисципліни.</exception>
    public void Place(Registration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        if (registration.Items.Count == 0)
        {
            throw new InvalidOperationException("Неможливо оформити порожню заявку без позицій.");
        }

        _repository.Add(registration);
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
