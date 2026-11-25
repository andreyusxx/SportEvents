namespace SportEvents.Legacy;

/// <summary>
/// Результат обробки заявки: сформований звіт і підсумкова сума до сплати.
/// </summary>
/// <param name="Report">Текстовий звіт або код помилки валідації.</param>
/// <param name="GrandTotal">Підсумкова сума до сплати з урахуванням знижок і доставки.</param>
public sealed record RegistrationHandleResult(
    string Report,
    decimal GrandTotal);
