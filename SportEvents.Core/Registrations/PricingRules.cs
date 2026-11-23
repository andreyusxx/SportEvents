namespace SportEvents.Core.Registrations;

/// <summary>
/// Містить іменовані константи тарифної політики та знижок для заявок на змагання.
/// </summary>
public static class PricingRules
{
    /// <summary>
    /// Мінімальна кількість слотів у позиції для нарахування гуртової знижки.
    /// </summary>
    public const int BulkMinQuantity = 10;

    /// <summary>
    /// Ставка гуртової знижки на окрему позицію (5 %).
    /// </summary>
    public const decimal BulkRate = 0.05m;

    /// <summary>
    /// Ставка пільгової знижки для ліцензованого учасника змагань (7 %).
    /// </summary>
    public const decimal LicensedParticipantRate = 0.07m;

    /// <summary>
    /// Поріг суми заявки для надання підвищеної знижки за обсяг.
    /// </summary>
    public const decimal HighVolumeThreshold = 5000m;

    /// <summary>
    /// Підвищена ставка знижки за обсяг заявки (10 %).
    /// </summary>
    public const decimal HighVolumeRate = 0.10m;

    /// <summary>
    /// Базовий поріг суми заявки для надання знижки за обсяг.
    /// </summary>
    public const decimal MediumVolumeThreshold = 2000m;

    /// <summary>
    /// Базова ставка знижки за обсяг заявки (5 %).
    /// </summary>
    public const decimal MediumVolumeRate = 0.05m;

    /// <summary>
    /// Промокод на відсоткову знижку 10 %.
    /// </summary>
    public const string PercentPromoCode = "SALE10";

    /// <summary>
    /// Відсоткова ставка знижки за промокодом SALE10 (10 %).
    /// </summary>
    public const decimal PercentPromoRate = 0.10m;

    /// <summary>
    /// Промокод на фіксовану знижку 200 грн.
    /// </summary>
    public const string FixedPromoCode = "MINUS200";

    /// <summary>
    /// Фіксована сума знижки за промокодом MINUS200.
    /// </summary>
    public const decimal FixedPromoAmount = 200m;

    /// <summary>
    /// Мінімальна сума заявки для активації фіксованого промокоду.
    /// </summary>
    public const decimal FixedPromoMinTotal = 1000m;

    /// <summary>
    /// Ставка додаткової знижки в неділю (2 %).
    /// </summary>
    public const decimal WeekendRate = 0.02m;

    /// <summary>
    /// Поріг суми заявки для безкоштовної доставки стартового пакета.
    /// </summary>
    public const decimal FreeDeliveryFrom = 1000m;

    /// <summary>
    /// Кількість десяткових знаків при округленні підсумкової суми.
    /// </summary>
    public const int MoneyDecimals = 2;
}
