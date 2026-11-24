namespace SportEvents.Core.Parsing;

/// <summary>
/// Представляє неперевірені вхідні дані позиції заявки із зовнішнього джерела (консоль або файл).
/// </summary>
/// <param name="ParticipantId">Сирий рядок ідентифікатора учасника.</param>
/// <param name="EventCode">Код спортивної дисципліни.</param>
/// <param name="Quantity">Сирий рядок кількості слотів.</param>
public sealed record RegistrationInputRequest(
    string ParticipantId,
    string EventCode,
    string Quantity);
