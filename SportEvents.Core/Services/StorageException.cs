namespace SportEvents.Core.Errors;

/// <summary>
/// Виняток, що сигналізує про збій середовища під час роботи зі сховищем даних.
/// </summary>
public sealed class StorageException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StorageException"/> class.
    /// Ініціалізує новий екземпляр винятку сховища з контекстом операції.
    /// </summary>
    /// <param name="operation">Назва операції (наприклад, Save або Load).</param>
    /// <param name="target">Шлях або ідентифікатор ресурсу сховища.</param>
    /// <param name="message">Зрозуміле повідомлення українською мовою.</param>
    /// <param name="innerException">Первинний виняток середовища (наприклад, IOException).</param>
    public StorageException(string operation, string target, string message, Exception innerException)
        : base(message, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        ArgumentException.ThrowIfNullOrWhiteSpace(target);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        ArgumentNullException.ThrowIfNull(innerException);

        Operation = operation;
        Target = target;
    }

    /// <summary>
    /// Gets назву операції сховища, під час якої стався збій.
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Gets шлях до файлу або ім'я ресурсу сховища.
    /// </summary>
    public string Target { get; }
}
