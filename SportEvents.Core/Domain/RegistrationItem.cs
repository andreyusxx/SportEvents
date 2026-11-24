namespace SportEvents.Core.Domain;

/// <summary>
/// Описує незмінну позицію заявки на участь у спортивних змаганнях.
/// </summary>
public sealed record RegistrationItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegistrationItem"/> class.
    /// Ініціалізує новий екземпляр позиції заявки з перевіркою інваріантів.
    /// </summary>
    /// <param name="code">Код спортивної дисципліни або стартового пакета.</param>
    /// <param name="quantity">Кількість замовлених слотів або категорій.</param>
    /// <param name="unitPrice">Вартість реєстраційного внеску за одну одиницю.</param>
    /// <exception cref="ArgumentException">Код дисципліни порожній або складається з пробілів.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Кількість менша або дорівнює нулю, або ціна від'ємна.</exception>
    public RegistrationItem(string code, int quantity, decimal unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice);

        Code = code;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    /// <summary>
    /// Gets отримує код дисципліни або послуги.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets отримує кількість слотів у позиції.
    /// </summary>
    public int Quantity { get; }

    /// <summary>
    /// Gets отримує ціну за одиницю у гривнях.
    /// </summary>
    public decimal UnitPrice { get; }

    /// <summary>
    /// Gets отримує повну вартість позиції без урахування знижок на заявку.
    /// </summary>
    public decimal LineTotal => UnitPrice * Quantity;
}
