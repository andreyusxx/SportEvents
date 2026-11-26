namespace SportEvents.Core.Parsing;

/// <summary>
/// Валідований рядок заявки на спортивний захід.
/// </summary>
/// <param name="EventCode">Код спортивного заходу (1–20 символів).</param>
/// <param name="ParticipantsCount">Кількість учасників (від 1 до 500).</param>
/// <param name="EntryFee">Вартість участі за одного учасника (невід'ємна).</param>
public sealed record ParsedRegistrationLine(string EventCode, int ParticipantsCount, decimal EntryFee);
