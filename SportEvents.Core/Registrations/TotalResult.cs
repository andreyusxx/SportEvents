using SportEvents.Core.Domain;

namespace SportEvents.Core.Registrations;

/// <summary>
/// Інкапсулює результат розрахунку вартості та оновленого стану заявки.
/// </summary>
/// <param name="Total">Підсумкова сума до сплати.</param>
/// <param name="NewStatus">Оновлений стан заявки після обробки.</param>
public sealed record TotalResult(
    decimal Total,
    RegistrationStatus NewStatus);
