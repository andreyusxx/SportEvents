namespace SportEvents.Legacy;

/// <summary>
/// Обчислює проміжну суму, знижки учасників та вартість доставки для успадкованого модуля.
/// </summary>
public sealed class LegacyPricingPolicy
{
    private const decimal FreeShippingThreshold = 1000m;
    private const decimal UrgentShippingFee = 150m;
    private const decimal StandardShippingFee = 50m;

    private readonly IReadOnlyList<IDiscountRule> _rules =
    [
        new VipDiscountRule(),
        new PartnerDiscountRule(),
        new LoyalAthleteDiscountRule(),
    ];

    private interface IDiscountRule
    {
        bool IsMatch(Athlete athlete, decimal subtotal);

        decimal Calculate(decimal subtotal);
    }

    /// <summary>
    /// Обчислює загальну вартість усіх позицій заявки до застосування знижок.
    /// </summary>
    /// <param name="items">Позиції заявки.</param>
    /// <returns>Сума вартостей позицій.</returns>
    public static decimal CalculateSubtotal(IReadOnlyList<EventLineItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        decimal sum = 0m;
        for (int i = 0; i < items.Count; i++)
        {
            sum += items[i].Qty * items[i].Price;
        }

        return sum;
    }

    /// <summary>
    /// Обчислює вартість доставки залежно від суми після знижки та терміновості.
    /// </summary>
    /// <param name="amountAfterDiscount">Сума після вирахування знижок.</param>
    /// <param name="urgent">Ознака термінової доставки.</param>
    /// <returns>Вартість доставки.</returns>
    public static decimal CalculateShipping(decimal amountAfterDiscount, bool urgent)
    {
        if (amountAfterDiscount >= FreeShippingThreshold)
        {
            return 0m;
        }

        return urgent ? UrgentShippingFee : StandardShippingFee;
    }

    /// <summary>
    /// Обчислює розмір знижки за категорією та лояльністю учасника.
    /// </summary>
    /// <param name="athlete">Учасник змагань.</param>
    /// <param name="subtotal">Проміжна сума заявки.</param>
    /// <returns>Сума знижки.</returns>
    public decimal CalculateDiscount(Athlete athlete, decimal subtotal)
    {
        ArgumentNullException.ThrowIfNull(athlete);

        for (int i = 0; i < _rules.Count; i++)
        {
            if (_rules[i].IsMatch(athlete, subtotal))
            {
                return _rules[i].Calculate(subtotal);
            }
        }

        return 0m;
    }

    private sealed class VipDiscountRule : IDiscountRule
    {
        private const decimal Threshold = 500m;
        private const decimal HighRate = 0.15m;
        private const decimal BaseRate = 0.10m;

        public bool IsMatch(Athlete athlete, decimal subtotal) => athlete.Kind == "vip";

        public decimal Calculate(decimal subtotal) => subtotal > Threshold ? subtotal * HighRate : subtotal * BaseRate;
    }

    private sealed class PartnerDiscountRule : IDiscountRule
    {
        private const decimal Threshold = 500m;
        private const decimal HighRate = 0.12m;
        private const decimal BaseRate = 0.05m;

        public bool IsMatch(Athlete athlete, decimal subtotal) => athlete.Kind == "partner";

        public decimal Calculate(decimal subtotal) => subtotal > Threshold ? subtotal * HighRate : subtotal * BaseRate;
    }

    private sealed class LoyalAthleteDiscountRule : IDiscountRule
    {
        private const int MinDoneCount = 10;
        private const decimal Threshold = 500m;
        private const decimal LoyalRate = 0.07m;

        public bool IsMatch(Athlete athlete, decimal subtotal) => athlete.DoneCount > MinDoneCount && subtotal > Threshold;

        public decimal Calculate(decimal subtotal) => subtotal * LoyalRate;
    }
}
