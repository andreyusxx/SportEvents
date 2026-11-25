using System.Globalization;
using System.Text;

namespace SportEvents.Core.Infrastructure;

/// <summary>
/// Записує журнал аудиту операцій із заявками у файл із гарантованим звільненням ресурсів через IDisposable.
/// </summary>
public sealed class FileAuditLog : IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileAuditLog"/> class.
    /// Ініціалізує новий екземпляр журналу аудиту з дозаписом у вказаний файл.
    /// </summary>
    /// <param name="path">Шлях до файлу журналу аудиту.</param>
    public FileAuditLog(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _writer = new StreamWriter(path, append: true, Encoding.UTF8);
    }

    /// <summary>
    /// Дописує один структурований рядок аудиту до потоку.
    /// </summary>
    /// <param name="operation">Назва виконаної операції.</param>
    /// <param name="documentId">Ідентифікатор документа заявки.</param>
    /// <exception cref="ObjectDisposedException">Спроба запису після звільнення ресурсу.</exception>
    public void Write(string operation, int documentId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);

        string line = string.Format(
            CultureInfo.InvariantCulture,
            "{0:O};{1};{2}",
            DateTimeOffset.Now,
            operation,
            documentId);
        _writer.WriteLine(line);
    }

    /// <summary>
    /// Скидає буфер на диск та звільняє файловий потік.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _writer.Flush();
        _writer.Dispose();
        _disposed = true;
    }
}
