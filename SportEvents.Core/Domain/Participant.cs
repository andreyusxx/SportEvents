namespace SportEvents.Core.Domain;

/// <summary>Учасник змагань.</summary>
public sealed class Participant
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsLicensed { get; init; }
}