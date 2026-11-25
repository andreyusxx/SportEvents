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
    private readonly List<string> _log = new List<string>();
    private decimal _lastDiscount;
    private decimal _lastShipping;
    private decimal _lastTotal;

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
        if (items != null)
        {
            if (items.Count > 0)
            {
                if (state == "new" || state == "draft")
                {
                    if (clientMail != null && clientMail.Contains("@"))
                    {
                        decimal sum = 0m;
                        for (int i = 0; i < items.Count; i++)
                        {
                            sum += items[i].Qty * items[i].Price;
                        }

                        decimal disc = 0m;
                        if (athlete.Kind == "vip")
                        {
                            if (sum > 500m)
                            {
                                disc = sum * 0.15m;
                            }
                            else
                            {
                                disc = sum * 0.10m;
                            }
                        }
                        else if (athlete.Kind == "partner")
                        {
                            if (sum > 500m)
                            {
                                disc = sum * 0.12m;
                            }
                            else
                            {
                                disc = sum * 0.05m;
                            }
                        }
                        else if (athlete.DoneCount > 10)
                        {
                            if (sum > 500m)
                            {
                                disc = sum * 0.07m;
                            }
                        }

                        disc += manualBonus;
                        decimal afterDisc = sum - disc;
                        decimal ship = 0m;
                        if (afterDisc < 1000m)
                        {
                            ship = urgent ? 150m : 50m;
                        }

                        grandTotal = afterDisc + ship;
                        _lastDiscount = disc;
                        _lastShipping = ship;
                        _lastTotal = grandTotal;

                        string rep = "Документ #" + docId + "\n";
                        rep += "Клієнт: " + athlete.Name + "\n";
                        for (int i = 0; i < items.Count; i++)
                        {
                            rep += items[i].Code + " x" + items[i].Qty + " = " + (items[i].Qty * items[i].Price).ToString("0.00") + " " + currency + "\n";
                        }

                        rep += "Знижка: " + _lastDiscount.ToString("0.00") + " " + currency + "\n";
                        rep += "Доставка: " + _lastShipping.ToString("0.00") + " " + currency + "\n";
                        rep += "Разом: " + _lastTotal.ToString("0.00") + " " + currency + "\n";

                        if (sendMail)
                        {
                            _log.Add("MAIL->" + clientMail + ":" + grandTotal.ToString("0.00"));
                        }

                        _log.Add("OK#" + docId);
                        return rep;
                    }
                    else
                    {
                        _log.Add("ERR: mail");
                        return "ERR: mail";
                    }
                }
                else
                {
                    _log.Add("ERR: state");
                    return "ERR: state";
                }
            }
            else
            {
                _log.Add("ERR: empty");
                return "ERR: empty";
            }
        }
        else
        {
            _log.Add("ERR: null");
            return "ERR: null";
        }
    }

    public string Preview(int docId, Athlete athlete, List<EventLineItem> items)
    {
        if (items == null || items.Count == 0)
        {
            return "EMPTY";
        }

        decimal sum = 0m;
        for (int i = 0; i < items.Count; i++)
        {
            sum += items[i].Qty * items[i].Price;
        }

        decimal disc = 0m;
        if (athlete.Kind == "vip")
        {
            disc = sum > 500m ? sum * 0.15m : sum * 0.10m;
        }
        else if (athlete.Kind == "partner")
        {
            disc = sum > 500m ? sum * 0.12m : sum * 0.05m;
        }
        else if (athlete.DoneCount > 10 && sum > 500m)
        {
            disc = sum * 0.07m;
        }

        decimal afterDisc = sum - disc;
        decimal ship = afterDisc < 1000m ? 50m : 0m;
        decimal total = afterDisc + ship;

        string rep = "Документ #" + docId + "\n";
        rep += "Клієнт: " + athlete.Name + "\n";
        for (int i = 0; i < items.Count; i++)
        {
            rep += items[i].Code + " x" + items[i].Qty + " = " + (items[i].Qty * items[i].Price).ToString("0.00") + " UAH\n";
        }

        rep += "Знижка: " + disc.ToString("0.00") + " UAH\n";
        rep += "Доставка: " + ship.ToString("0.00") + " UAH\n";
        rep += "Разом: " + total.ToString("0.00") + " UAH\n";
        return rep;
    }

    public string DescribeClient(Athlete a)
    {
        string s = a.Name.Trim().ToUpper();
        if (a.Kind == "vip") s += " [VIP]";
        if (a.DoneCount > 10) s += " [ЛОЯЛЬНИЙ]";
        s += " <" + a.Email.ToLower() + ">";
        int years = DateTime.Now.Year - a.SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }
}