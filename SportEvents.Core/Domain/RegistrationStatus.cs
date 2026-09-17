namespace SportEvents.Core.Domain;

/// <summary>Стан реєстрації на змагання в життєвому циклі.</summary>
public enum RegistrationStatus
{
    New = 0,
    Confirmed = 1,
    Finished = 2,
    Cancelled = 3,
    Disqualified = 4,
}