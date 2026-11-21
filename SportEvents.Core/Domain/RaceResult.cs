using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace SportEvents.Core.Domain;

/// <summary>Представляє зафіксований результат проходження дистанції учасником.</summary>
public sealed class RaceResult
{
    private const long TicksPerHundredthSecond = TimeSpan.TicksPerMillisecond * 10;

    /// <summary>Initializes a new instance of the <see cref="RaceResult"/> class.Ініціалізує екземпляр результату забігу з перевіркою часових меж.</summary>
    /// <param name="chipStartAt">Момент перетину стартового килима.</param>
    /// <param name="finishAt">Момент перетину фінішної лінії.</param>
    /// <param name="penaltyDuration">Нарахований штрафний час.</param>
    /// <exception cref="ArgumentException">
    /// Виникає, якщо <paramref name="finishAt"/> передує <paramref name="chipStartAt"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Виникає, якщо <paramref name="penaltyDuration"/> менший за <see cref="TimeSpan.Zero"/>.
    /// </exception>
    public RaceResult(
        DateTimeOffset chipStartAt,
        DateTimeOffset finishAt,
        TimeSpan penaltyDuration)
    {
        if (finishAt < chipStartAt)
        {
            throw new ArgumentException(
                "Час фінішу не може передувати часу старту.",
                nameof(finishAt));
        }

        if (penaltyDuration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(penaltyDuration),
                "Штрафний час не може бути від'ємним.");
        }

        ChipStartAt = chipStartAt;
        FinishAt = finishAt;
        PenaltyDuration = penaltyDuration;
    }

    /// <summary>Gets отримує момент перетину стартового килима.</summary>
    public DateTimeOffset ChipStartAt { get; }

    /// <summary>Gets отримує момент перетину фінішної лінії.</summary>
    public DateTimeOffset FinishAt { get; }

    /// <summary>Gets отримує нарахований штрафний час.</summary>
    public TimeSpan PenaltyDuration { get; }

    /// <summary>Gets отримує чистий час проходження дистанції без урахування штрафів.</summary>
    public TimeSpan NetDuration => FinishAt - ChipStartAt;

    /// <summary>Gets отримує підсумковий протокольний час із урахуванням штрафів.</summary>
    public TimeSpan OfficialDuration => NetDuration + PenaltyDuration;

    /// <summary>
    /// Нормалізує офіційний час до сотих часток секунди (0,01 с) для ранжування в протоколі.
    /// </summary>
    /// <returns>Нормалізований інтервал <see cref="TimeSpan"/>.</returns>
    [SuppressMessage(
        "Design",
        "CA1024:Use properties where appropriate",
        Justification = "Метод виконує округлення тактів, а не читання поля (правило N-03).")]
    public TimeSpan GetNormalizedOfficialDuration()
    {
        long normalizedTicks = (long)Math.Round(
            (double)OfficialDuration.Ticks / TicksPerHundredthSecond,
            MidpointRounding.AwayFromZero) * TicksPerHundredthSecond;

        return TimeSpan.FromTicks(normalizedTicks);
    }

    /// <summary>
    /// Форматує підсумковий час у стандарті протоколу (hh:mm:ss.ff) без зміни стану.
    /// </summary>
    /// <returns>Рядок часу у форматі <c>hh:mm:ss.ff</c>.</returns>
    public string FormatOfficialDuration()
    {
        return GetNormalizedOfficialDuration()
            .ToString(@"hh\:mm\:ss\.ff", CultureInfo.InvariantCulture);
    }
}
