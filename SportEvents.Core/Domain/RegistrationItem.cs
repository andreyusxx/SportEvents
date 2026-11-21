namespace SportEvents.Core.Domain;

/// <summary>Позиція реєстрації: код дисципліни, кількість категорій та вартість.</summary>
public sealed class RegistrationItem
{
    /// <summary>Gets отримує або встановлює значення властивості.</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>Gets отримує або встановлює значення властивості.</summary>
    public int Quantity { get; init; }

    /// <summary>Gets отримує або встановлює значення властивості.</summary>
    public decimal UnitPrice { get; init; }

    /// <summary>Gets отримує або встановлює значення властивості.</summary>
    public decimal Amount => Quantity * UnitPrice;
}
