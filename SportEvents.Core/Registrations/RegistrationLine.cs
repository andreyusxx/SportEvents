namespace SportEvents.Core.Registrations;

/// <summary>
/// Позиція заявки на участь у спортивних змаганнях для успадкованого обробника.
/// </summary>
public sealed class RegistrationLine
{
    /// <summary>
    /// Gets or sets отримує або задає код дисципліни чи послуги змагань.
    /// </summary>
    public string EventCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets отримує або задає кількість слотів або категорій участі.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets отримує або задає вартість реєстраційного внеску за одиницю.
    /// </summary>
    public decimal UnitPrice { get; set; }
}
