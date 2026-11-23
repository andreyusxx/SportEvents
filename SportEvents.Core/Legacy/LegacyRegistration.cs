namespace SportEvents.Core.Legacy;

/// <summary>
/// Успадкована модель заявки з відкритими полями та віртуальним методом додавання позицій.
/// </summary>
public class LegacyRegistration
{
    /// <summary>
    /// Gets or sets отримує або задає ідентифікатор заявки.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets отримує або задає тип клієнта (Regular, Licensed, Vip).
    /// </summary>
    public string ClientType { get; set; } = "Regular";

    /// <summary>
    /// Gets or sets отримує або задає електронну пошту клієнта.
    /// </summary>
    public string ClientEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets отримує або задає перелік позицій заявки.
    /// </summary>
    public List<LegacyRegistrationLine> Lines { get; set; } = new();

    /// <summary>
    /// Додає позицію до заявки.
    /// </summary>
    /// <param name="line">Позиція заявки.</param>
    public virtual void AddLine(LegacyRegistrationLine line)
    {
        Lines.Add(line);
    }
}
