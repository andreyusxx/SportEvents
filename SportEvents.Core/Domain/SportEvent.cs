namespace SportEvents.Core.Domain;

/// <summary>Спортивна подія або дисципліна в каталозі.</summary>
public sealed class SportEvent
{
    /// <summary>Gets отримує або встановлює значення властивості.</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>Gets отримує або встановлює значення властивості.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Gets отримує або встановлює значення властивості.</summary>
    public decimal Fee { get; init; }

    /// <summary>Gets отримує або встановлює значення властивості.</summary>
    public int MaxParticipants { get; init; }
}
