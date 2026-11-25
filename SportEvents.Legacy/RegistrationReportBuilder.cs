#pragma warning disable
using System.Collections.Generic;
using System.Text;

namespace SportEvents.Legacy;

public sealed class RegistrationReportBuilder
{
    public string Build(
        int docId,
        Athlete athlete,
        IReadOnlyList<EventLineItem> items,
        string currency,
        decimal discount,
        decimal shipping,
        decimal total)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Документ #").Append(docId).Append('\n');
        sb.Append("Клієнт: ").Append(athlete.Name).Append('\n');

        for (int i = 0; i < items.Count; i++)
        {
            decimal lineTotal = items[i].Qty * items[i].Price;
            sb.Append(items[i].Code)
              .Append(" x")
              .Append(items[i].Qty)
              .Append(" = ")
              .Append(lineTotal.ToString("0.00"))
              .Append(' ')
              .Append(currency)
              .Append('\n');
        }

        sb.Append("Знижка: ").Append(discount.ToString("0.00")).Append(' ').Append(currency).Append('\n');
        sb.Append("Доставка: ").Append(shipping.ToString("0.00")).Append(' ').Append(currency).Append('\n');
        sb.Append("Разом: ").Append(total.ToString("0.00")).Append(' ').Append(currency).Append('\n');
        return sb.ToString();
    }
}