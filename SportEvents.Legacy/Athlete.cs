using System.Globalization;

namespace SportEvents.Legacy;

/// <summary>
/// Подає дані учасника спортивних змагань у модулі SportEvents.Legacy.
/// </summary>
public class Athlete
{
    private const int LoyalMinDoneCount = 10;

    /// <summary>Gets or sets унікальний ідентифікатор учасника.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets повне ім'я учасника.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets електронну пошту учасника.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets категорію участі (regular, vip, partner).</summary>
    public string Kind { get; set; } = "regular";

    /// <summary>Gets or sets кількість завершених стартів учасника.</summary>
    public int DoneCount { get; set; }

    /// <summary>Gets or sets дату реєстрації учасника в системі (UTC).</summary>
    public DateTime SinceUtc { get; set; }

    /// <summary>
    /// Формує текстовий рядок опису учасника з його статусом, поштою та стажем.
    /// </summary>
    /// <returns>Форматований опис учасника.</returns>
    public string Describe()
    {
        string s = Name.Trim().ToUpper(CultureInfo.InvariantCulture);
        if (Kind == "vip")
        {
            s += " [VIP]";
        }

        if (DoneCount > LoyalMinDoneCount)
        {
            s += " [ЛОЯЛЬНИЙ]";
        }

        s += " <" + Email.ToLower(CultureInfo.InvariantCulture) + ">";
        int years = DateTime.Now.Year - SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }
}
