using SportEvents.Core.Domain;

namespace SportEvents.Core.Registrations;

/// <summary>
/// Надає чисті підпрограми для поетапного розрахунку вартості та стану заявки на змагання.
/// </summary>
public static class RegistrationCalculator
{
    /// <summary>
    /// Обчислює вартість однієї позиції заявки з урахуванням гуртової знижки.
    /// </summary>
    /// <param name="line">Позиція заявки на змагання.</param>
    /// <returns>Сума за позицією без побічних ефектів.</returns>
    public static decimal LineSubtotalOf(RegistrationLine line)
    {
        RegistrationGuards.EnsureLineValid(line);
        decimal sum = line.Quantity * line.UnitPrice;
        return line.Quantity >= PricingRules.BulkMinQuantity
            ? sum * (1m - PricingRules.BulkRate)
            : sum;
    }

    /// <summary>
    /// Обчислює проміжний підсумок за всіма позиціями заявки.
    /// </summary>
    /// <param name="lines">Перелік позицій заявки.</param>
    /// <returns>Загальна сума позицій до застосування знижок на документ.</returns>
    public static decimal SubtotalOf(IReadOnlyList<RegistrationLine> lines)
    {
        RegistrationGuards.EnsureLinesValid(lines);
        decimal total = 0m;
        foreach (RegistrationLine line in lines)
        {
            total += LineSubtotalOf(line);
        }

        return total;
    }

    /// <summary>
    /// Обчислює розмір пільгової знижки для ліцензованого учасника.
    /// </summary>
    /// <param name="amount">Поточна сума заявки.</param>
    /// <param name="isLicensed">Ознака наявності чинної спортивної ліцензії.</param>
    /// <returns>Сума знижки за ліцензію.</returns>
    public static decimal LicensedParticipantDiscountOf(decimal amount, bool isLicensed)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return isLicensed
            ? amount * PricingRules.LicensedParticipantRate
            : 0m;
    }

    /// <summary>
    /// Обчислює розмір знижки за загальний обсяг заявки.
    /// </summary>
    /// <param name="amount">Поточна сума заявки.</param>
    /// <returns>Сума знижки за обсяг.</returns>
    public static decimal VolumeDiscountOf(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        if (amount > PricingRules.HighVolumeThreshold)
        {
            return amount * PricingRules.HighVolumeRate;
        }

        if (amount > PricingRules.MediumVolumeThreshold)
        {
            return amount * PricingRules.MediumVolumeRate;
        }

        return 0m;
    }

    /// <summary>
    /// Обчислює розмір знижки за промокодом.
    /// </summary>
    /// <param name="amount">Поточна сума заявки.</param>
    /// <param name="promoCode">Промокод учасника.</param>
    /// <returns>Сума знижки за промокодом.</returns>
    public static decimal PromoDiscountOf(decimal amount, string? promoCode)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        if (promoCode == PricingRules.PercentPromoCode)
        {
            return amount * PricingRules.PercentPromoRate;
        }

        if (promoCode == PricingRules.FixedPromoCode && amount > PricingRules.FixedPromoMinTotal)
        {
            return PricingRules.FixedPromoAmount;
        }

        return 0m;
    }

    /// <summary>
    /// Обчислює розмір недільної знижки за датою оформлення заявки.
    /// </summary>
    /// <param name="amount">Поточна сума заявки.</param>
    /// <param name="createdAt">Дата створення заявки.</param>
    /// <returns>Сума недільної знижки.</returns>
    public static decimal WeekendDiscountOf(decimal amount, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return createdAt.DayOfWeek == DayOfWeek.Sunday
            ? amount * PricingRules.WeekendRate
            : 0m;
    }

    /// <summary>
    /// Обчислює вартість доставки стартового пакета залежно від суми заявки.
    /// </summary>
    /// <param name="amount">Сума заявки після всіх знижок.</param>
    /// <param name="deliveryPrice">Базова вартість доставки.</param>
    /// <returns>Плата за доставку або 0, якщо досягнуто поріг безкоштовної доставки.</returns>
    public static decimal DeliveryFeeOf(decimal amount, decimal deliveryPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        ArgumentOutOfRangeException.ThrowIfNegative(deliveryPrice);
        return amount < PricingRules.FreeDeliveryFrom
            ? deliveryPrice
            : 0m;
    }

    /// <summary>
    /// Визначає наступний стан заявки на змагання без побічних ефектів.
    /// </summary>
    /// <param name="current">Поточний стан заявки.</param>
    /// <param name="total">Підсумкова сума до сплати.</param>
    /// <returns>Оновлений стан у життєвому циклі <see cref="RegistrationStatus"/>.</returns>
    public static RegistrationStatus NextStatusOf(RegistrationStatus current, decimal total)
        => current switch
        {
            RegistrationStatus.New when total > 0m => RegistrationStatus.Confirmed,
            RegistrationStatus.Confirmed => RegistrationStatus.Finished,
            _ => current,
        };

    /// <summary>
    /// Виконує повний розрахунок вартості та оновленого стану заявки на змагання.
    /// </summary>
    /// <param name="request">Структурований запит на розрахунок заявки.</param>
    /// <returns>Об'єкт результату з підсумковою сумою та новим станом.</returns>
    public static TotalResult Calculate(TotalRequest request)
    {
        RegistrationGuards.EnsureRequestValid(request);

        decimal total = SubtotalOf(request.Lines);
        total -= LicensedParticipantDiscountOf(total, request.Participant.IsLicensed);
        total -= VolumeDiscountOf(total);
        total -= PromoDiscountOf(total, request.PromoCode);
        total -= WeekendDiscountOf(total, request.CreatedAt);
        total += DeliveryFeeOf(total, request.DeliveryPrice);

        if (total < 0m)
        {
            total = 0m;
        }

        total = Math.Round(total, PricingRules.MoneyDecimals);

        RegistrationStatus nextStatus = NextStatusOf(request.Status, total);
        if (request.Status == RegistrationStatus.Cancelled)
        {
            total = 0m;
        }

        return new TotalResult(total, nextStatus);
    }
}
