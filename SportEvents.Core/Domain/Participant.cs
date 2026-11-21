namespace SportEvents.Core.Domain;

/// <summary>Учасник змагань.</summary>
public sealed class Participant
{
    /// <summary>Gets отримує або встановлює значення властивості.</summary>
    public int Id { get; init; }

    /// <summary>Gets отримує або встановлює значення властивості.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Gets a value indicating whether отримує або встановлює значення властивості.</summary>
    public bool IsLicensed { get; init; }
}
