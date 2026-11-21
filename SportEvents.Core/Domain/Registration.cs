namespace SportEvents.Core.Domain;

/// <summary>Реєстрація учасника на змагання.</summary>
public sealed class Registration
{
    private readonly List<RegistrationItem> _items = new();

    /// <summary>Gets отримує або встановлює значення властивості.</summary>
    public int Id { get; init; }

    /// <summary>Gets отримує або встановлює значення властивості.</summary>
    public int ParticipantId { get; init; }

    /// <summary>Gets or sets отримує або встановлює значення властивості.</summary>
    public RegistrationStatus Status { get; set; } = RegistrationStatus.New;

    /// <summary>Gets отримує або встановлює значення властивості.</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>Gets отримує або встановлює значення властивості.</summary>
    public IReadOnlyList<RegistrationItem> Items => _items;

    /// <summary>Додає позицію до реєстрації.</summary>
    /// <param name="item">Позиція реєстрації, яку потрібно додати.</param>
    public void AddItem(RegistrationItem item) => _items.Add(item);

    /// <summary>Обчислює загальну суму реєстраційного внеску.</summary>
    /// <returns>Загальна сума внеску типу decimal.</returns>
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
