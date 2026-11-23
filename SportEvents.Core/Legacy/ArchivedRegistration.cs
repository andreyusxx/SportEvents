namespace SportEvents.Core.Legacy;

/// <summary>
/// Успадкований клас архівної заявки, що порушує принцип підстановки Лісков.
/// </summary>
public sealed class ArchivedRegistration : LegacyRegistration
{
    /// <inheritdoc />
    public override void AddLine(LegacyRegistrationLine line)
    {
        throw new NotSupportedException("Архівну заявку заборонено змінювати.");
    }
}
