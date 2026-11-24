using SportEvents.Core.Domain;

namespace SportEvents.Core.Abstractions;

/// <summary>
/// Визначає контракт політики розрахунку вартості окремої позиції заявки.
/// </summary>
public interface IPricingPolicy
{
    /// <summary>
    /// Обчислює підсумкову вартість позиції з урахуванням тарифної політики.
    /// </summary>
    /// <param name="item">Незмінна позиція заявки.</param>
    /// <returns>Вартість позиції у гривнях.</returns>
    decimal PriceOf(RegistrationItem item);
}
