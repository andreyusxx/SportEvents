using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;

namespace SportEvents.Core.Pricing;

/// <summary>
/// Реалізує базову тарифну політику без надання знижок.
/// </summary>
public sealed class RegularPricingPolicy : IPricingPolicy
{
    /// <inheritdoc />
    public decimal PriceOf(RegistrationItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return Math.Round(item.LineTotal, 2, MidpointRounding.AwayFromZero);
    }
}
