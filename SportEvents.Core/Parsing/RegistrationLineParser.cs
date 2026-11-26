using System.Globalization;

namespace SportEvents.Core.Parsing;

/// <summary>
/// Межа між зовнішніми неперевіреними даними та доменними сутностями.
/// </summary>
public static class RegistrationLineParser
{
    /// <summary>
    /// Безпечно розбирає вхідний рядок формату "КодПодії;Кількість;Вартість".
    /// </summary>
    /// <param name="raw">Сирий вхідний рядок із зовнішнього джерела.</param>
    /// <param name="line">Розпізнана та валідована сутність або null у разі помилки.</param>
    /// <returns>true, якщо рядок коректний; інакше false.</returns>
    public static bool TryParse(string? raw, out ParsedRegistrationLine? line)
    {
        line = null;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        string[] parts = raw.Split(';');
        if (parts.Length != 3 || parts[0].Trim().Length is 0 or > 20)
        {
            return false;
        }

        if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int count)
            || count is < 1 or > 500)
        {
            return false;
        }

        if (!decimal.TryParse(parts[2], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal fee)
            || fee < 0m)
        {
            return false;
        }

        line = new ParsedRegistrationLine(parts[0].Trim(), count, fee);
        return true;
    }
}
