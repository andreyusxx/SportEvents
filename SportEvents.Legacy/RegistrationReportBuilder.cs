using System.Globalization;
using System.Text;

namespace SportEvents.Legacy;

/// <summary>
/// Формує багаторядковий текстовий звіт за результатами розрахунку заявки.
/// </summary>
public sealed class RegistrationReportBuilder
{
    /// <summary>
    /// Будує текстовий документ заявки з переліком позицій, знижкою, доставкою та підсумком.
    /// </summary>
    /// <param name="docId">Ідентифікатор документа.</param>
    /// <param name="athlete">Учасник змагань.</param>
    /// <param name="items">Позиції заявки.</param>
    /// <param name="currency">Валюта документа.</param>
    /// <param name="discount">Загальна сума знижки.</param>
    /// <param name="shipping">Вартість доставки.</param>
    /// <param name="total">Підсумкова сума до сплати.</param>
    /// <returns>Сформований текст звіту.</returns>
    public static string Build(
        int docId,
        Athlete athlete,
        IReadOnlyList<EventLineItem> items,
        string currency,
        decimal discount,
        decimal shipping,
        decimal total)
    {
        ArgumentNullException.ThrowIfNull(athlete);
        ArgumentNullException.ThrowIfNull(items);

        StringBuilder sb = new();
        sb.Append("Документ #").Append(docId).Append('\n');
        sb.Append("Клієнт: ").Append(athlete.Name).Append('\n');

        for (int i = 0; i < items.Count; i++)
        {
            decimal lineTotal = items[i].Qty * items[i].Price;
            sb.Append(items[i].Code)
              .Append(" x")
              .Append(items[i].Qty)
              .Append(" = ")
              .Append(lineTotal.ToString("0.00", CultureInfo.InvariantCulture))
              .Append(' ')
              .Append(currency)
              .Append('\n');
        }

        sb.Append("Знижка: ").Append(discount.ToString("0.00", CultureInfo.InvariantCulture)).Append(' ').Append(currency).Append('\n');
        sb.Append("Доставка: ").Append(shipping.ToString("0.00", CultureInfo.InvariantCulture)).Append(' ').Append(currency).Append('\n');
        sb.Append("Разом: ").Append(total.ToString("0.00", CultureInfo.InvariantCulture)).Append(' ').Append(currency).Append('\n');
        return sb.ToString();
    }
}
