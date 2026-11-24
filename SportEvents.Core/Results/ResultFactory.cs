namespace SportEvents.Core.Results;

/// <summary>
/// Надає фабричні методи для створення успішних та невдалих результатів <see cref="Result{T}"/>.
/// </summary>
public static class ResultFactory
{
    /// <summary>
    /// Створює успішний результат із переданим значенням.
    /// </summary>
    /// <typeparam name="T">Тип успішного значення.</typeparam>
    /// <param name="value">Валідне значення.</param>
    /// <returns>Успішний екземпляр <see cref="Result{T}"/>.</returns>
    public static Result<T> Ok<T>(T value)
        => new(true, value, string.Empty);

    /// <summary>
    /// Створює результат із помилкою користувача.
    /// </summary>
    /// <typeparam name="T">Очікуваний тип значення.</typeparam>
    /// <param name="error">Зрозумілий текст помилки для користувача.</param>
    /// <returns>Невдалий екземпляр <see cref="Result{T}"/>.</returns>
    public static Result<T> Fail<T>(string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);
        return new(false, default, error);
    }
}
