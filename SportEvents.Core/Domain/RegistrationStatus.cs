namespace SportEvents.Core.Domain;

/// <summary>Визначає стани заявки учасника в життєвому циклі змагань.</summary>
public enum RegistrationStatus
{
    /// <summary>Нова заявка, що очікує підтвердження.</summary>
    New = 0,

    /// <summary>Підтверджена заявка допущеного учасника.</summary>
    Confirmed = 1,

    /// <summary>Учасник фінішував і отримав результат.</summary>
    Finished = 2,

    /// <summary>Заявку скасовано до старту.</summary>
    Cancelled = 3,

    /// <summary>Учасника дискваліфіковано за порушення регламенту.</summary>
    Disqualified = 4,
}
