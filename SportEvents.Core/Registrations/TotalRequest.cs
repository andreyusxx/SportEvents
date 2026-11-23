using SportEvents.Core.Domain;

namespace SportEvents.Core.Registrations;

/// <summary>
/// Інкапсулює вхідні параметри запиту на розрахунок вартості та стану заявки.
/// </summary>
/// <param name="RegistrationId">Ідентифікатор заявки на реєстрацію.</param>
/// <param name="Participant">Відомості про учасника змагань.</param>
/// <param name="Lines">Перелік обраних дисциплін і послуг.</param>
/// <param name="CreatedAt">Дата створення заявки.</param>
/// <param name="PromoCode">Промокод на знижку.</param>
/// <param name="Status">Поточний стан заявки.</param>
/// <param name="DeliveryPrice">Вартість доставки стартового пакета.</param>
public sealed record TotalRequest(
    int RegistrationId,
    ParticipantInfo Participant,
    IReadOnlyList<RegistrationLine> Lines,
    DateOnly CreatedAt,
    string? PromoCode,
    RegistrationStatus Status,
    decimal DeliveryPrice);
