namespace SportEvents.Core.Results;

/// <summary>
/// Представляє результат операції на межі системи без використання винятків для очікуваних відмов.
/// </summary>
/// <typeparam name="T">Тип значення у разі успішного виконання операції.</typeparam>
public sealed class Result<T>
{
    private readonly T? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class.
    /// Ініціалізує новий екземпляр результату операції.
    /// </summary>
    /// <param name="isSuccess">Ознака успіху операції.</param>
    /// <param name="value">Значення результату.</param>
    /// <param name="error">Текст помилки користувача.</param>
    internal Result(bool isSuccess, T? value, string error)
    {
        IsSuccess = isSuccess;
        _value = value;
        Error = error;
    }

    /// <summary>
    /// Gets a value indicating whether операція завершилася успішно.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets текст діагностичного повідомлення для користувача у разі помилки.
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// Gets значення успішного результату або генерує виняток, якщо операція завершилася помилкою.
    /// </summary>
    /// <exception cref="InvalidOperationException">Спроба звернутися до значення невдалого результату.</exception>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException($"Неможливо отримати значення невдалого результату: {Error}");
}
