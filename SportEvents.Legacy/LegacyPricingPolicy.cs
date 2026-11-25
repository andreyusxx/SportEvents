#pragma warning disable
using System.Collections.Generic;

namespace SportEvents.Legacy;

public interface ILegacyDiscountRule
{
    bool IsMatch(Athlete athlete, decimal subtotal);

    decimal Calculate(decimal subtotal);
}

public sealed class VipDiscountRule : ILegacyDiscountRule
{
    private const decimal Threshold = 500m;
    private const decimal HighRate = 0.15m;
    private const decimal BaseRate = 0.10m;

    public bool IsMatch(Athlete athlete, decimal subtotal) => athlete.Kind == "vip";

    public decimal Calculate(decimal subtotal) => subtotal > Threshold ? subtotal * HighRate : subtotal * BaseRate;
}

public sealed class PartnerDiscountRule : ILegacyDiscountRule
{
    private const decimal Threshold = 500m;
    private const decimal HighRate = 0.12m;
    private const decimal BaseRate = 0.05m;

    public bool IsMatch(Athlete athlete, decimal subtotal) => athlete.Kind == "partner";

    public decimal Calculate(decimal subtotal) => subtotal > Threshold ? subtotal * HighRate : subtotal * BaseRate;
}

public sealed class LoyalAthleteDiscountRule : ILegacyDiscountRule
{
    private const int MinDoneCount = 10;
    private const decimal Threshold = 500m;
    private const decimal LoyalRate = 0.07m;

    public bool IsMatch(Athlete athlete, decimal subtotal) => athlete.DoneCount > MinDoneCount && subtotal > Threshold;

    public decimal Calculate(decimal subtotal) => subtotal * LoyalRate;
}

public sealed class LegacyPricingPolicy
{
    private const decimal FreeShippingThreshold = 1000m;
    private const decimal UrgentShippingFee = 150m;
    private const decimal StandardShippingFee = 50m;

    private readonly IReadOnlyList<ILegacyDiscountRule> _rules =
    [
        new VipDiscountRule(),
        new PartnerDiscountRule(),
        new LoyalAthleteDiscountRule(),
    ];

    public decimal CalculateSubtotal(IReadOnlyList<EventLineItem> items)
    {
        decimal sum = 0m;
        for (int i = 0; i < items.Count; i++)
        {
            sum += items[i].Qty * items[i].Price;
        }

        return sum;
    }

    public decimal CalculateDiscount(Athlete athlete, decimal subtotal)
    {
        for (int i = 0; i < _rules.Count; i++)
        {
            if (_rules[i].IsMatch(athlete, subtotal))
            {
                return _rules[i].Calculate(subtotal);
            }
        }

        return 0m;
    }

    public decimal CalculateShipping(decimal amountAfterDiscount, bool urgent)
    {
        if (amountAfterDiscount >= FreeShippingThreshold)
        {
            return 0m;
        }

        return urgent ? UrgentShippingFee : StandardShippingFee;
    }
}