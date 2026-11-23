using SportEvents.Core.Legacy;

namespace SportEvents.Core.Registrations;

/// <summary>
/// Надає методи захисного програмування для перевірки передумов заявок на участь у змаганнях.
/// </summary>
public static class RegistrationGuards
{
    /// <summary>
    /// Перевіряє коректність заголовка заявки та контактних даних учасника.
    /// </summary>
    /// <param name="registrationId">Ідентифікатор заявки.</param>
    /// <param name="participantName">Прізвище та ім'я учасника.</param>
    /// <param name="participantEmail">Електронна пошта учасника.</param>
    /// <exception cref="ArgumentOutOfRangeException">Ідентифікатор заявки менший або дорівнює нулю.</exception>
    /// <exception cref="ArgumentException">Ім'я або пошта порожні, або пошта не містить символ @.</exception>
    public static void EnsureHeaderValid(
        int registrationId,
        string participantName,
        string participantEmail)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(registrationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(participantName);
        ArgumentException.ThrowIfNullOrWhiteSpace(participantEmail);
        if (!participantEmail.Contains('@'))
        {
            throw new ArgumentException(
                "Пошта учасника має містити символ @.",
                nameof(participantEmail));
        }
    }

    /// <summary>
    /// Перевіряє коректність окремої позиції заявки на змагання.
    /// </summary>
    /// <param name="line">Позиція заявки.</param>
    /// <exception cref="ArgumentNullException">Позиція дорівнює null.</exception>
    /// <exception cref="ArgumentException">Код дисципліни порожній.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Кількість менша або дорівнює нулю, або ціна від'ємна.</exception>
    public static void EnsureLineValid(LegacyRegistrationLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException.ThrowIfNullOrWhiteSpace(line.EventCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(line.Quantity, nameof(line.Quantity));
        ArgumentOutOfRangeException.ThrowIfNegative(line.UnitPrice, nameof(line.UnitPrice));
    }

    /// <summary>
    /// Перевіряє непорожність переліку позицій та валідність кожної позиції заявки.
    /// </summary>
    /// <param name="lines">Перелік позицій заявки.</param>
    /// <exception cref="ArgumentNullException">Колекція позицій дорівнює null.</exception>
    /// <exception cref="ArgumentException">Заявка не містить жодної позиції або містить некоректні дані.</exception>
    public static void EnsureLinesValid(IReadOnlyList<LegacyRegistrationLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (lines.Count == 0)
        {
            throw new ArgumentException(
                "Заявка не містить жодної позиції.",
                nameof(lines));
        }

        foreach (LegacyRegistrationLine line in lines)
        {
            EnsureLineValid(line);
        }
    }
}
