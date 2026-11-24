using System.Globalization;
using SportEvents.Core.Domain;

namespace SportEvents.Core.Reporting;

/// <summary>
/// Формує рядкове подання заявки у форматі CSV без побічних ефектів запису на диск.
/// </summary>
public static class RegistrationCsvReport
{
    /// <summary>
    /// Будує рядок CSV для переданої заявки та її підсумкової вартості.
    /// </summary>
    /// <param name="registration">Заявка на змагання.</param>
    /// <param name="total">Підсумкова вартість заявки.</param>
    /// <returns>Рядок у форматі CSV з роздільником крапка з комою.</returns>
    public static string BuildCsv(Registration registration, decimal total)
    {
        ArgumentNullException.ThrowIfNull(registration);
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0};{1};{2};{3:F2}",
            registration.Id,
            registration.ParticipantId,
            registration.Status,
            total);
    }
}
