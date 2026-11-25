#pragma warning disable
using System;
using System.Collections.Generic;

namespace SportEvents.Legacy;

public class Athlete
{
    public int Id;
    public string Name = "";
    public string Email = "";
    public string Kind = "regular";
    public int DoneCount;
    public DateTime SinceUtc;
}

public class EventLineItem
{
    public string Code = "";
    public int Qty;
    public decimal Price;
}

public class LegacyRegistrationProcessor
{
    private const decimal DiscountThreshold = 500m;
    private const decimal VipHighRate = 0.15m;
    private const decimal VipBaseRate = 0.10m;
    private const decimal PartnerHighRate = 0.12m;
    private const decimal PartnerBaseRate = 0.05m;
    private const decimal LoyalRate = 0.07m;
    private const int LoyalMinDoneCount = 10;
    private const decimal FreeShippingThreshold = 1000m;
    private const decimal UrgentShippingFee = 150m;
    private const decimal StandardShippingFee = 50m;

    private readonly List<string> _log = new List<string>();

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
        grandTotal = 0m;
        string? validationError = Validate(items, state, clientMail);
        if (validationError != null)
        {
            _log.Add(validationError);
            return validationError;
        }

        decimal sum = CalculateSubtotal(items);
        decimal disc = CalculateDiscount(athlete, sum) + manualBonus;
        decimal afterDisc = sum - disc;
        decimal ship = CalculateShipping(afterDisc, urgent);
        grandTotal = afterDisc + ship;

        if (sendMail)
        {
            _log.Add("MAIL->" + clientMail + ":" + grandTotal.ToString("0.00"));
        }

        _log.Add("OK#" + docId);
        return BuildReportText(docId, athlete, items, currency, disc, ship, grandTotal);
    }

    public string Preview(int docId, Athlete athlete, List<EventLineItem> items)
    {
        if (items == null || items.Count == 0)
        {
            return "EMPTY";
        }

        decimal sum = CalculateSubtotal(items);
        decimal disc = CalculateDiscount(athlete, sum);
        decimal afterDisc = sum - disc;
        decimal ship = CalculateShipping(afterDisc, urgent: false);
        decimal total = afterDisc + ship;

        return BuildReportText(docId, athlete, items, "UAH", disc, ship, total);
    }

    public string DescribeClient(Athlete a)
    {
        string s = a.Name.Trim().ToUpper();
        if (a.Kind == "vip")
        {
            s += " [VIP]";
        }

        if (a.DoneCount > LoyalMinDoneCount)
        {
            s += " [ЛОЯЛЬНИЙ]";
        }

        s += " <" + a.Email.ToLower() + ">";
        int years = DateTime.Now.Year - a.SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log)
        {
            r += l + "\n";
        }

        return r;
    }

    private static string? Validate(List<EventLineItem>? items, string state, string? clientMail)
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

        if (clientMail == null || !clientMail.Contains("@"))
        {
            return "ERR: mail";
        }

        return null;
    }

    private static decimal CalculateSubtotal(IReadOnlyList<EventLineItem> items)
    {
        decimal sum = 0m;
        for (int i = 0; i < items.Count; i++)
        {
            sum += items[i].Qty * items[i].Price;
        }

        return sum;
    }

    private static decimal CalculateDiscount(Athlete athlete, decimal sum)
    {
        if (athlete.Kind == "vip")
        {
            return sum > DiscountThreshold ? sum * VipHighRate : sum * VipBaseRate;
        }

        if (athlete.Kind == "partner")
        {
            return sum > DiscountThreshold ? sum * PartnerHighRate : sum * PartnerBaseRate;
        }

        if (athlete.DoneCount > LoyalMinDoneCount && sum > DiscountThreshold)
        {
            return sum * LoyalRate;
        }

        return 0m;
    }

    private static decimal CalculateShipping(decimal amountAfterDiscount, bool urgent)
    {
        if (amountAfterDiscount >= FreeShippingThreshold)
        {
            return 0m;
        }

        return urgent ? UrgentShippingFee : StandardShippingFee;
    }

    private static string BuildReportText(
        int docId,
        Athlete athlete,
        IReadOnlyList<EventLineItem> items,
        string currency,
        decimal discount,
        decimal shipping,
        decimal total)
    {
        string rep = "Документ #" + docId + "\n";
        rep += "Клієнт: " + athlete.Name + "\n";
        for (int i = 0; i < items.Count; i++)
        {
            rep += items[i].Code + " x" + items[i].Qty + " = " + (items[i].Qty * items[i].Price).ToString("0.00") + " " + currency + "\n";
        }

        rep += "Знижка: " + discount.ToString("0.00") + " " + currency + "\n";
        rep += "Доставка: " + shipping.ToString("0.00") + " " + currency + "\n";
        rep += "Разом: " + total.ToString("0.00") + " " + currency + "\n";
        return rep;
    }
}