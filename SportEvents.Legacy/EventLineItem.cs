namespace SportEvents.Legacy;

/// <summary>
/// Подає окрему позицію заявки учасника на спортивну подію.
/// </summary>
public class EventLineItem
{
    /// <summary>Gets or sets код спортивної дисципліни або послуги.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Gets or sets кількість замовлених одиниць.</summary>
    public int Qty { get; set; }

    /// <summary>Gets or sets ціну однієї одиниці.</summary>
    public decimal Price { get; set; }
}
