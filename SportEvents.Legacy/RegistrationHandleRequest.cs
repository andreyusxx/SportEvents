namespace SportEvents.Legacy;

/// <summary>
/// Об'єкт-параметр із вхідними даними для обробки заявки учасника.
/// </summary>
/// <param name="DocId">Ідентифікатор документа заявки.</param>
/// <param name="Athlete">Дані учасника змагань.</param>
/// <param name="Items">Перелік позицій заявки.</param>
/// <param name="State">Поточний стан документа (new або draft).</param>
/// <param name="Currency">Код валюти розрахунку.</param>
/// <param name="SendMail">Ознака потреби надіслати сповіщення на пошту.</param>
/// <param name="ClientMail">Електронна адреса отримувача.</param>
/// <param name="Urgent">Ознака термінової доставки стартового пакета.</param>
/// <param name="ManualBonus">Ручна бонусна знижка у валюті розрахунку.</param>
public sealed record RegistrationHandleRequest(
    int DocId,
    Athlete Athlete,
    IReadOnlyList<EventLineItem>? Items,
    string State,
    string Currency,
    bool SendMail,
    string ClientMail,
    bool Urgent,
    decimal ManualBonus);
