using System.Globalization;
using System.Text;
using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;
using SportEvents.Core.Errors;

namespace SportEvents.Core.Infrastructure;

/// <summary>
/// Реалізує файлове сховище заявок із захистом від блокування файлу та атомарним перезаписом.
/// </summary>
public sealed class FileRegistrationRepository : IRegistrationRepository
{
    private const int MaxRetryAttempts = 3;
    private const int RetryDelayMilliseconds = 50;

    private readonly string _filePath;
    private readonly Dictionary<int, Registration> _cache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FileRegistrationRepository"/> class.
    /// Ініціалізує новий екземпляр файлового сховища за вказаним шляхом.
    /// </summary>
    /// <param name="filePath">Шлях до файлу збереження заявок.</param>
    public FileRegistrationRepository(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
    }

    /// <inheritdoc />
    public void Add(Registration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        if (_cache.ContainsKey(registration.Id))
        {
            throw new DomainRuleException(
                "registration.unique",
                $"Заявку з ідентифікатором {registration.Id} вже зареєстровано.");
        }

        _cache.Add(registration.Id, registration);
        try
        {
            PersistAllWithRetry();
        }
        catch
        {
            // Відкочуємо зміну в оперативній пам'яті, щоб стан кешу не розійшовся з диском
            _cache.Remove(registration.Id);
            throw;
        }
    }

    /// <inheritdoc />
    public Registration? GetById(int id)
        => _cache.GetValueOrDefault(id);

    /// <inheritdoc />
    public IReadOnlyList<Registration> GetAll()
        => new List<Registration>(_cache.Values);

    private void PersistAllWithRetry()
    {
        IOException? lastError = null;

        for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
        {
            try
            {
                WriteAtomically();
                return;
            }
            catch (IOException ex)
            {
                lastError = ex;
                if (attempt < MaxRetryAttempts)
                {
                    Thread.Sleep(RetryDelayMilliseconds * attempt);
                }
            }
        }

        throw new StorageException(
            "Save",
            _filePath,
            $"Не вдалося зберегти заявки у файл '{_filePath}' після {MaxRetryAttempts} спроб.",
            lastError!);
    }

    private void WriteAtomically()
    {
        string tempPath = $"{_filePath}.tmp";
        using (FileStream stream = new(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
        using (StreamWriter writer = new(stream, Encoding.UTF8))
        {
            foreach (Registration registration in _cache.Values)
            {
                string line = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0};{1};{2};{3:yyyy-MM-dd};{4:F2}",
                    registration.Id,
                    registration.ParticipantId,
                    registration.Status,
                    registration.CreatedAt,
                    registration.Subtotal());
                writer.WriteLine(line);
            }
        }

        File.Move(tempPath, _filePath, overwrite: true);
    }
}
