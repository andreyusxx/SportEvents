namespace SportEvents.Core.Legacy;

/// <summary>
/// Успадкована модель позиції заявки з класу-Бога.
/// </summary>
public sealed class LegacyRegistrationLine
{
    /// <summary>
    /// Gets or sets отримує або задає код дисципліни.
    /// </summary>
    public string EventCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets отримує або задає кількість слотів.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets отримує або задає ціну за одиницю.
    /// </summary>
    public decimal UnitPrice { get; set; }
}
