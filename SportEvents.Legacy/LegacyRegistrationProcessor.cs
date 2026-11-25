#pragma warning disable
using System;
using System.Collections.Generic;

namespace SportEvents.Legacy;

public class EventLineItem
{
    public string Code = "";
    public int Qty;
    public decimal Price;
}

public class LegacyRegistrationProcessor
{
    private readonly List<string> _log = new List<string>();
    private readonly RegistrationReportBuilder _reportBuilder = new();
    private readonly LegacyPricingPolicy _pricing = new();

    public RegistrationHandleResult Handle(RegistrationHandleRequest request)
    {
        string? validationError = Validate(request.Items, request.State, request.ClientMail);
        if (validationError != null)
        {
            _log.Add(validationError);
            return new RegistrationHandleResult(validationError, 0m);
        }

        decimal subtotal = _pricing.CalculateSubtotal(request.Items!);
        decimal discount = _pricing.CalculateDiscount(request.Athlete, subtotal) + request.ManualBonus;
        decimal afterDiscount = subtotal - discount;
        decimal shipping = _pricing.CalculateShipping(afterDiscount, request.Urgent);
        decimal grandTotal = afterDiscount + shipping;

        if (request.SendMail)
        {
            _log.Add("MAIL->" + request.ClientMail + ":" + grandTotal.ToString("0.00"));
        }

        _log.Add("OK#" + request.DocId);
        string report = _reportBuilder.Build(
            request.DocId,
            request.Athlete,
            request.Items!,
            request.Currency,
            discount,
            shipping,
            grandTotal);

        return new RegistrationHandleResult(report, grandTotal);
    }

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

    public string Preview(int docId, Athlete athlete, List<EventLineItem> items)
    {
        if (items == null || items.Count == 0)
        {
            return "EMPTY";
        }

        decimal subtotal = _pricing.CalculateSubtotal(items);
        decimal discount = _pricing.CalculateDiscount(athlete, subtotal);
        decimal afterDiscount = subtotal - discount;
        decimal shipping = _pricing.CalculateShipping(afterDiscount, urgent: false);
        decimal total = afterDiscount + shipping;

        return _reportBuilder.Build(docId, athlete, items, "UAH", discount, shipping, total);
    }

    public string DescribeClient(Athlete a) => a.Describe();

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log)
        {
            r += l + "\n";
        }

        return r;
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

        if (clientMail == null || !clientMail.Contains("@"))
        {
            return "ERR: mail";
        }

        return null;
    }
}