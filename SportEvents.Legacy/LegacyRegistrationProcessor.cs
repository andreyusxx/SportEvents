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
    private const int LoyalMinDoneCount = 10;

    private readonly List<string> _log = new List<string>();
    private readonly RegistrationReportBuilder _reportBuilder = new();
    private readonly LegacyPricingPolicy _pricing = new();

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

        decimal sum = _pricing.CalculateSubtotal(items);
        decimal disc = _pricing.CalculateDiscount(athlete, sum) + manualBonus;
        decimal afterDisc = sum - disc;
        decimal ship = _pricing.CalculateShipping(afterDisc, urgent);
        grandTotal = afterDisc + ship;

        if (sendMail)
        {
            _log.Add("MAIL->" + clientMail + ":" + grandTotal.ToString("0.00"));
        }

        _log.Add("OK#" + docId);
        return _reportBuilder.Build(docId, athlete, items, currency, disc, ship, grandTotal);
    }

    public string Preview(int docId, Athlete athlete, List<EventLineItem> items)
    {
        if (items == null || items.Count == 0)
        {
            return "EMPTY";
        }

        decimal sum = _pricing.CalculateSubtotal(items);
        decimal disc = _pricing.CalculateDiscount(athlete, sum);
        decimal afterDisc = sum - disc;
        decimal ship = _pricing.CalculateShipping(afterDisc, urgent: false);
        decimal total = afterDisc + ship;

        return _reportBuilder.Build(docId, athlete, items, "UAH", disc, ship, total);
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
}