using System.Diagnostics.CodeAnalysis;

namespace SportEvents.Core.Legacy;

/// <summary>
/// Успадкований монолітний обробник розрахунку підсумкової суми заявки на змагання.
/// </summary>
public static class LegacyProcessor
{
    /// <summary>
    /// Виконує перевірку, розрахунок знижок, зміну стану та друк заявки у консоль.
    /// </summary>
    /// <param name="registrationId">Ідентифікатор заявки на реєстрацію.</param>
    /// <param name="participantName">Прізвище та ім'я учасника.</param>
    /// <param name="participantEmail">Електронна пошта учасника.</param>
    /// <param name="isLicensed">Ознака наявності чинної спортивної ліцензії.</param>
    /// <param name="lines">Перелік обраних дисциплін та послуг.</param>
    /// <param name="createdAt">Дата створення заявки.</param>
    /// <param name="promoCode">Промокод на знижку.</param>
    /// <param name="status">Поточний стан заявки.</param>
    /// <param name="printToConsole">Ознака необхідності виведення деталей у консоль.</param>
    /// <param name="deliveryPrice">Вартість доставки стартового пакета.</param>
    /// <param name="newStatus">Оновлений стан заявки після обробки.</param>
    /// <returns>Підсумкова сума до сплати або -1 у разі помилки вхідних даних.</returns>
    [SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Успадкований метод перед рефакторингом у ЛР № 3.")]
    public static decimal Process(
        int registrationId,
        string participantName,
        string participantEmail,
        bool isLicensed,
        List<LegacyRegistrationLine> lines,
        DateOnly createdAt,
        string promoCode,
        string status,
        bool printToConsole,
        decimal deliveryPrice,
        out string newStatus)
    {
        newStatus = status;
        decimal total = 0;
        if (registrationId <= 0)
        {
            Console.WriteLine("Помилка: номер документа");
            return -1;
        }

        if (participantName == null || participantName == string.Empty)
        {
            Console.WriteLine("Помилка: немає клієнта");
            return -1;
        }

        if (participantEmail == null || !participantEmail.Contains('@'))
        {
            Console.WriteLine("Помилка: пошта клієнта");
            return -1;
        }

        if (lines == null || lines.Count == 0)
        {
            Console.WriteLine("Помилка: немає позицій");
            return -1;
        }

        if (status != "New" && status != "Paid" &&
            status != "Shipped" && status != "Cancelled")
        {
            Console.WriteLine("Помилка: невідомий стан");
            return -1;
        }

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Quantity <= 0)
            {
                Console.WriteLine("Помилка: кількість");
                return -1;
            }

            if (lines[i].UnitPrice < 0)
            {
                Console.WriteLine("Помилка: ціна");
                return -1;
            }

            decimal sum = lines[i].Quantity * lines[i].UnitPrice;
            if (lines[i].Quantity >= 10)
            {
                sum = sum * 0.95m;
            }

            total = total + sum;
        }

        if (isLicensed)
        {
            total = total * 0.93m;
        }

        if (total > 5000)
        {
            total = total * 0.9m;
        }
        else if (total > 2000)
        {
            total = total * 0.95m;
        }

        if (promoCode == "SALE10")
        {
            total = total - (total * 0.1m);
        }
        else if (promoCode == "MINUS200" && total > 1000)
        {
            total = total - 200;
        }

        if (createdAt.DayOfWeek == DayOfWeek.Sunday)
        {
            total = total * 0.98m;
        }

        if (total < 1000)
        {
            total = total + deliveryPrice;
        }

        if (total < 0)
        {
            total = 0;
        }

        total = Math.Round(total, 2);

        if (status == "New" && total > 0)
        {
            newStatus = "Paid";
        }
        else if (status == "Paid")
        {
            newStatus = "Shipped";
        }
        else if (status == "Cancelled")
        {
            newStatus = "Cancelled";
            total = 0;
        }

        if (printToConsole)
        {
            Console.WriteLine("Документ № " + registrationId);
            Console.WriteLine("Клієнт: " + participantName);
            Console.WriteLine("Пошта: " + participantEmail);
            for (int i = 0; i < lines.Count; i++)
            {
                Console.WriteLine(lines[i].EventCode + " x " + lines[i].Quantity);
            }

            Console.WriteLine("Разом: " + total);
            Console.WriteLine("Стан: " + newStatus);
        }

        return total;
    }
}
