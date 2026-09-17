namespace SportEvents.Core.Domain;

/// <summary>Реєстрація учасника на змагання.</summary>
public sealed class Registration
{
    private readonly List<RegistrationItem> _items = new();

    public int Id { get; init; }
    public int ParticipantId { get; init; }
    public RegistrationStatus Status { get; set; } = RegistrationStatus.New;
    public DateTimeOffset CreatedAt { get; init; }
    public IReadOnlyList<RegistrationItem> Items => _items;

    /// <summary>Додає позицію до реєстрації.</summary>
    public void AddItem(RegistrationItem item) => _items.Add(item);

    /// <summary>Обчислює загальну суму реєстраційного внеску.</summary>
    public decimal Total()
    {
        decimal sum = 0m;
        foreach (RegistrationItem item in _items)
        {
            sum += item.Amount;
        }
        return sum;
    }
}