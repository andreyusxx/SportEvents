namespace SportEvents.Core.Domain;

/// <summary>Позиція реєстрації: код дисципліни, кількість категорій та вартість.</summary>
public sealed class RegistrationItem
{
    public string Code { get; init; } = string.Empty;

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal Amount => Quantity * UnitPrice;
}
