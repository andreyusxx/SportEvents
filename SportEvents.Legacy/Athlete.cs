#pragma warning disable
using System;

namespace SportEvents.Legacy;

public class Athlete
{
    private const int LoyalMinDoneCount = 10;

    public int Id;
    public string Name = "";
    public string Email = "";
    public string Kind = "regular";
    public int DoneCount;
    public DateTime SinceUtc;

    public string Describe()
    {
        string s = Name.Trim().ToUpper();
        if (Kind == "vip")
        {
            s += " [VIP]";
        }

        if (DoneCount > LoyalMinDoneCount)
        {
            s += " [ЛОЯЛЬНИЙ]";
        }

        s += " <" + Email.ToLower() + ">";
        int years = DateTime.Now.Year - SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }
}