namespace SportEvents.Core.Legacy;

/// <summary>
/// Конкретна файлова реалізація перевантаженого сховища.
/// </summary>
public sealed class FileRegistrationStore : IRegistrationStore
{
    /// <inheritdoc />
    public void Save(LegacyRegistration registration)
    {
    }

    /// <inheritdoc />
    public LegacyRegistration? Load(int id) => null;

    /// <inheritdoc />
    public void Delete(int id)
    {
    }

    /// <inheritdoc />
    public void ExportCsv(string path)
    {
    }

    /// <inheritdoc />
    public void ImportCsv(string path)
    {
    }

    /// <inheritdoc />
    public void SendEmail(string email, string body)
    {
    }

    /// <inheritdoc />
    public void ClearCache()
    {
    }
}
