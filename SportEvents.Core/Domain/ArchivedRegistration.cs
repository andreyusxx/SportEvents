namespace SportEvents.Core.Domain;

/// <summary>
/// Представляє архівний запис завершеної або закритої заявки на основі композиції.
/// </summary>
public sealed class ArchivedRegistration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArchivedRegistration"/> class.
    /// Ініціалізує новий архівний запис на основі наявного документа заявки.
    /// </summary>
    /// <param name="document">Документ заявки для архівування.</param>
    /// <param name="archivedAt">Дата й час перенесення до архіву.</param>
    /// <exception cref="ArgumentNullException">Документ дорівнює null.</exception>
    /// <exception cref="InvalidOperationException">Спроба архівувати нову непідтверджену заявку.</exception>
    public ArchivedRegistration(Registration document, DateTimeOffset archivedAt)
    {
        ArgumentNullException.ThrowIfNull(document);
        if (document.Status == RegistrationStatus.New)
        {
            throw new InvalidOperationException("Неможливо архівувати заявку у стані New.");
        }

        Document = document;
        ArchivedAt = archivedAt;
    }

    /// <summary>
    /// Gets отримує інкапсульований документ заявки.
    /// </summary>
    public Registration Document { get; }

    /// <summary>
    /// Gets отримує момент архівування заявки.
    /// </summary>
    public DateTimeOffset ArchivedAt { get; }
}
