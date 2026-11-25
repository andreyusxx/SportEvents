using System.Globalization;
using SportEvents.Core.Domain;
using SportEvents.Core.Results;

namespace SportEvents.Core.Parsing;

/// <summary>
/// Захисна барикада на межі введення, що перетворює сирі рядки на перевірені доменні типи.
/// </summary>
public static class RegistrationRequestBarricade
{
    private const int MinSlotsQuantity = 1;
    private const int MaxSlotsQuantity = 100;

    /// <summary>
    /// Перевіряє сирий запит на реєстрацію та будує чистий об'єкт <see cref="RegistrationItem"/>.
    /// </summary>
    /// <param name="request">Неперевірений зовнішній запит.</param>
    /// <param name="eventPrices">Довідник вартості реєстрації за кодами спортивних подій.</param>
    /// <returns>Успішний результат із <see cref="RegistrationItem"/> або опис помилки користувача.</returns>
    public static Result<RegistrationItem> ToRegistrationItem(
        RegistrationInputRequest? request,
        IReadOnlyDictionary<string, decimal> eventPrices)
    {
        ArgumentNullException.ThrowIfNull(eventPrices);

        if (request is null)
        {
            return ResultFactory.Fail<RegistrationItem>("Запит на реєстрацію не може бути порожнім.");
        }

        if (string.IsNullOrWhiteSpace(request.EventCode))
        {
            return ResultFactory.Fail<RegistrationItem>("Не вказано код спортивної дисципліни.");
        }

        string normalizedCode = request.EventCode.Trim();
        if (!eventPrices.TryGetValue(normalizedCode, out decimal unitPrice))
        {
            return ResultFactory.Fail<RegistrationItem>($"Невідому спортивну подію '{normalizedCode}' не знайдено в каталозі.");
        }

        if (!int.TryParse(
                request.Quantity,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int quantity))
        {
            return ResultFactory.Fail<RegistrationItem>("Кількість слотів має бути цілим числом.");
        }

        if (quantity < MinSlotsQuantity || quantity > MaxSlotsQuantity)
        {
            return ResultFactory.Fail<RegistrationItem>(
                $"Кількість слотів має належати діапазону від {MinSlotsQuantity} до {MaxSlotsQuantity}.");
        }

        return ResultFactory.Ok(
            new RegistrationItem(normalizedCode, quantity, unitPrice));
    }

    /// <summary>
    /// Безпечно розбирає грошовий внесок із рядка з використанням крапки як десяткового роздільника.
    /// </summary>
    /// <param name="rawPrice">Рядок із сумою внеску.</param>
    /// <returns>Успішний результат із числом <see cref="decimal"/> або повідомлення про помилку.</returns>
    public static Result<decimal> ParseEntryFee(string? rawPrice)
    {
        if (string.IsNullOrWhiteSpace(rawPrice))
        {
            return ResultFactory.Fail<decimal>("Не вказано суму реєстраційного внеску.");
        }

        if (!decimal.TryParse(
                rawPrice.Trim(),
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out decimal fee))
        {
            return ResultFactory.Fail<decimal>("Некоректний формат суми внеску (очікується число з крапкою).");
        }

        if (fee < 0m)
        {
            return ResultFactory.Fail<decimal>("Реєстраційний внесок не може бути від'ємним.");
        }

        return ResultFactory.Ok(fee);
    }
}
