using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;

namespace SportEvents.Tests.Doubles;

/// <summary>
/// Ручний стаб політики ціноутворення, який повертає фіксовану задану ціну для кожної позиції.
/// </summary>
/// <param name="fixedItemPrice">Фіксована ціна однієї позиції, яку повертатиме стаб.</param>
public sealed class FixedPricingPolicyStub(decimal fixedItemPrice) : IPricingPolicy
{
    /// <inheritdoc />
    public decimal PriceOf(RegistrationItem item) => fixedItemPrice;
}
