using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;

namespace SportEvents.Core.Pricing;

/// <summary>
/// Реалізує тарифну політику зі знижкою для пільгових або ліцензованих учасників.
/// </summary>
public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private const decimal MaxDiscountRate = 0.50m;
    private readonly decimal _rate;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscountPricingPolicy"/> class.
    /// Ініціалізує новий екземпляр політики зі знижкою.
    /// </summary>
    /// <param name="rate">Ставка знижки у частках одиниці (від 0 до 0.50).</param>
    /// <exception cref="ArgumentOutOfRangeException">Ставка знижки виходить за межі [0; 0.50].</exception>
    public DiscountPricingPolicy(decimal rate)
    {
        if (rate < 0m || rate > MaxDiscountRate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rate),
                "Ставка знижки має належати діапазону від 0 до 0.50.");
        }

        _rate = rate;
    }

    /// <inheritdoc />
    public decimal PriceOf(RegistrationItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return Math.Round(item.LineTotal * (1m - _rate), 2, MidpointRounding.AwayFromZero);
    }
}
