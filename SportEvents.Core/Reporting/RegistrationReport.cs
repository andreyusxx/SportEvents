using System.Globalization;
using System.Text;

using SportEvents.Core.Registrations;

namespace SportEvents.Core.Reporting;

/// <summary>
/// Формує текстове подання звіту за заявкою на участь у змаганнях без побічних ефектів.
/// </summary>
public static class RegistrationReport
{
    /// <summary>
    /// Створює форматований текст звіту для заданого запиту та результату розрахунку.
    /// </summary>
    /// <param name="request">Вхідний запит із даними заявки та учасника.</param>
    /// <param name="result">Обчислений підсумок і оновлений стан заявки.</param>
    /// <returns>Детермінований багаторядковий текст звіту.</returns>
    public static string TextOf(TotalRequest request, TotalResult result)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(result);

        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine(string.Format(culture, "Документ № {0}", request.RegistrationId));
        text.AppendLine("Клієнт: " + request.Participant.Name);
        foreach (RegistrationLine line in request.Lines)
        {
            text.AppendLine(string.Format(culture, "{0} x {1}", line.EventCode, line.Quantity));
        }

        text.AppendLine(string.Format(culture, "Разом: {0:F2}", result.Total));
        text.AppendLine("Стан: " + result.NewStatus);
        return text.ToString();
    }
}
