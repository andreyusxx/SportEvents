namespace SportEvents.Core.Domain;

/// <summary>Спортивна подія або дисципліна в каталозі.</summary>
public sealed class SportEvent
{
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public decimal Fee { get; init; }
    public int MaxParticipants { get; init; }
}