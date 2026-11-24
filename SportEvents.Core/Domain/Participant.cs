namespace SportEvents.Core.Domain;

/// <summary>
/// Представляє зареєстрованого учасника спортивних змагань.
/// </summary>
public sealed record Participant
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Participant"/> class.
    /// Ініціалізує новий екземпляр учасника змагань з перевіркою інваріантів.
    /// </summary>
    /// <param name="id">Унікальний ідентифікатор учасника.</param>
    /// <param name="name">Прізвище та ім'я спортсмена.</param>
    /// <param name="email">Електронна пошта учасника.</param>
    /// <exception cref="ArgumentOutOfRangeException">Ідентифікатор менший або дорівнює нулю.</exception>
    /// <exception cref="ArgumentException">Ім'я порожнє або електронна пошта не містить символ @.</exception>
    public Participant(int id, string name, string email)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (!email.Contains('@'))
        {
            throw new ArgumentException("Електронна пошта має містити символ @.", nameof(email));
        }

        Id = id;
        Name = name;
        Email = email;
    }

    /// <summary>
    /// Gets отримує унікальний ідентифікатор учасника.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets отримує прізвище та ім'я учасника.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets отримує контактну електронну пошту учасника.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Gets a value indicating whether отримує ознаку наявності чинної спортивної ліцензії.
    /// </summary>
    public bool IsLicensed { get; init; }
}
