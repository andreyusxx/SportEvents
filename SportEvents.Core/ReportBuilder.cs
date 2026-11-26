using System.Text;

namespace SportEvents.Core;

/// <summary>
/// Формує текст простого звіту з готових рядків.
/// </summary>
public sealed class ReportBuilder
{
    /// <summary>
    /// Заголовок звіту.
    /// </summary>
    public string Title { get; init; } = "Звіт";

    /// <summary>
    /// Будує багаторядковий текстовий звіт із переданих рядків.
    /// </summary>
    /// <param name="rows">Колекція рядків для включення у звіт.</param>
    /// <returns>Сформований текст звіту із заголовком у верхньому регістрі.</returns>
    public string Build(IReadOnlyList<string> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        StringBuilder text = new();
        text.AppendLine(Title.ToUpperInvariant());
        foreach (string row in rows)
        {
            text.AppendLine(row);
        }

        return text.ToString();
    }

    /// <summary>
    /// Повертає текстовий опис кількості позицій.
    /// </summary>
    /// <param name="count">Кількість позицій.</param>
    /// <returns>Рядок опису.</returns>
    public string Describe(int count)
    {
        string note;
        return "Позицій: " + count;
    }
}
