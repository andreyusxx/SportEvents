using System.Globalization;
using System.Text;

namespace SportEvents.Legacy;

/// <summary>
/// Координує валідацію, розрахунок вартості та формування звіту за заявкою на спортивну подію.
/// </summary>
public class LegacyRegistrationProcessor
{
    private readonly List<string> _log = new();
    private readonly LegacyPricingPolicy _pricing = new();

    /// <summary>
    /// Повертає текстовий опис учасника, делегуючи виклик сутності Athlete.
    /// </summary>
    /// <param name="athlete">Учасник змагань.</param>
    /// <returns>Форматований опис учасника.</returns>
    public static string DescribeClient(Athlete athlete)
    {
        ArgumentNullException.ThrowIfNull(athlete);
        return athlete.Describe();
    }

    /// <summary>
    /// Обробляє заявку за структурованим об'єктом-запитом.
    /// </summary>
    /// <param name="request">Параметри обробки заявки.</param>
    /// <returns>Результат обробки із текстом звіту та підсумковою сумою.</returns>
    public RegistrationHandleResult Handle(RegistrationHandleRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        string? validationError = Validate(request.Items, request.State, request.ClientMail);
        if (validationError != null)
        {
            _log.Add(validationError);
            return new RegistrationHandleResult(validationError, 0m);
        }

        decimal subtotal = LegacyPricingPolicy.CalculateSubtotal(request.Items!);
        decimal discount = _pricing.CalculateDiscount(request.Athlete, subtotal) + request.ManualBonus;
        decimal afterDiscount = subtotal - discount;
        decimal shipping = LegacyPricingPolicy.CalculateShipping(afterDiscount, request.Urgent);
        decimal grandTotal = afterDiscount + shipping;

        if (request.SendMail)
        {
            _log.Add("MAIL->" + request.ClientMail + ":" + grandTotal.ToString("0.00", CultureInfo.InvariantCulture));
        }

        _log.Add("OK#" + request.DocId);
        string report = RegistrationReportBuilder.Build(
            request.DocId,
            request.Athlete,
            request.Items!,
            request.Currency,
            discount,
            shipping,
            grandTotal);

        return new RegistrationHandleResult(report, grandTotal);
    }

    /// <summary>
    /// Забезпечує зворотну сумісність зі старою сигнатурою виклику обробки заявки.
    /// </summary>
    /// <param name="docId">Ідентифікатор документа.</param>
    /// <param name="athlete">Учасник змагань.</param>
    /// <param name="items">Позиції заявки.</param>
    /// <param name="state">Стан документа.</param>
    /// <param name="currency">Валюта розрахунку.</param>
    /// <param name="sendMail">Ознака надсилання поштового повідомлення.</param>
    /// <param name="clientMail">Електронна адреса учасника.</param>
    /// <param name="note">Застаріла примітка (не використовується).</param>
    /// <param name="urgent">Ознака термінової доставки.</param>
    /// <param name="manualBonus">Ручний бонус.</param>
    /// <param name="grandTotal">Вихідна підсумкова сума.</param>
    /// <returns>Сформований звіт або код помилки.</returns>
    public string Handle(
        int docId,
        Athlete athlete,
        List<EventLineItem> items,
        string state,
        string currency,
        bool sendMail,
        string clientMail,
        string note,
        bool urgent,
        decimal manualBonus,
        out decimal grandTotal)
    {
        _ = note;
        RegistrationHandleResult result = Handle(new RegistrationHandleRequest(
            docId,
            athlete,
            items,
            state,
            currency,
            sendMail,
            clientMail,
            urgent,
            manualBonus));
        grandTotal = result.GrandTotal;
        return result.Report;
    }

    /// <summary>
    /// Формує попередній перегляд розрахунку заявки без запису в журнал.
    /// </summary>
    /// <param name="docId">Ідентифікатор документа.</param>
    /// <param name="athlete">Учасник змагань.</param>
    /// <param name="items">Позиції заявки.</param>
    /// <returns>Текст попереднього звіту або EMPTY для порожнього списку.</returns>
    public string Preview(int docId, Athlete athlete, List<EventLineItem> items)
    {
        if (items == null || items.Count == 0)
        {
            return "EMPTY";
        }

        decimal subtotal = LegacyPricingPolicy.CalculateSubtotal(items);
        decimal discount = _pricing.CalculateDiscount(athlete, subtotal);
        decimal afterDiscount = subtotal - discount;
        decimal shipping = LegacyPricingPolicy.CalculateShipping(afterDiscount, urgent: false);
        decimal total = afterDiscount + shipping;

        return RegistrationReportBuilder.Build(docId, athlete, items, "UAH", discount, shipping, total);
    }

    /// <summary>
    /// Повертає накопичений журнал подій обробника у вигляді тексту.
    /// </summary>
    /// <returns>Рядки внутрішнього журналу.</returns>
    public string DumpLog()
    {
        StringBuilder sb = new();
        for (int i = 0; i < _log.Count; i++)
        {
            sb.Append(_log[i]).Append('\n');
        }

        return sb.ToString();
    }

    private static string? Validate(IReadOnlyList<EventLineItem>? items, string state, string? clientMail)
    {
        if (items == null)
        {
            return "ERR: null";
        }

        if (items.Count == 0)
        {
            return "ERR: empty";
        }

        if (state != "new" && state != "draft")
        {
            return "ERR: state";
        }

        if (clientMail == null || !clientMail.Contains('@', StringComparison.Ordinal))
        {
            return "ERR: mail";
        }

        return null;
    }
}
