namespace SportEvents.Core.Registrations;

/// <summary>
/// Описує відомості про учасника змагань для розрахунку заявки.
/// </summary>
/// <param name="Name">Прізвище та ім'я учасника.</param>
/// <param name="Email">Електронна пошта учасника.</param>
/// <param name="IsLicensed">Ознака наявності чинної спортивної ліцензії.</param>
public sealed record ParticipantInfo(
    string Name,
    string Email,
    bool IsLicensed);
