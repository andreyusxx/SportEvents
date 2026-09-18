using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopOrders.Domain;

/// <summary>
/// Представляє замовлення із переліком позицій та логікою розрахунку вартості.
/// </summary>
public class Order
{
    private readonly List<string[]> _lines = new List<string[]>();

    /// <summary>
    /// Ініціалізує новий екземпляр замовлення.
    /// </summary>
    /// <param name="a">Унікальний ідентифікатор замовлення.</param>
    /// <param name="b">Прізвище або ім'я клієнта.</param>
    public Order(string a, string b)
    {
        Id = a;
        CustomerName = b;
        CreatedAt = DateTime.Now;
    }

    /// <summary>
    /// Отримує ідентифікатор замовлення.
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// Отримує ім'я клієнта.
    /// </summary>
    public string CustomerName { get; private set; }

    /// <summary>
    /// Отримує поточний стан замовлення.
    /// </summary>
    public int Status { get; private set; } 

    /// <summary>
    /// Отримує дату та час створення замовлення.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Виконує пошук замовлення за його ідентифікатором у наданій колекції.
    /// </summary>
    /// <param name="orders">Колекція замовлень для пошуку.</param>
    /// <param name="orderId">Цільовий ідентифікатор замовлення.</param>
    /// <returns>Знайдений екземпляр замовлення або null, якщо запис не знайдено.</returns>
    public static Order? FindById(List<Order> orders, string orderId)
    {
        for (int i = 0; i < orders.Count; i++)
        {
            if (orders[i].Id == orderId)
            {
                return orders[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Додає до замовлення новий рядок із даними про товар.
    /// </summary>
    /// <param name="sku">Артикул позиції.</param>
    /// <param name="quantity">Кількість одиниць.</param>
    /// <param name="unitPrice">Ціна за одиницю товару.</param>
    public void AddLine(string sku, int quantity, decimal unitPrice)
    {
        string[] line = new string[3];
        line[0] = sku;
        line[1] = quantity.ToString();
        line[2] = unitPrice.ToString();
        _lines.Add(line);
    }

    /// <summary>
    /// Обчислює підсумкову вартість замовлення з урахуванням знижок та ПДВ.
    /// </summary>
    /// <param name="isRegularCustomer">Ознака наявності статусу постійного покупця.</param>
    /// <returns>Підсумкова сума до сплати, округлена до двох знаків.</returns>
    public decimal CalculateTotal(bool isRegularCustomer)
    {
        decimal total = 0;
        int lineCount = 0;
        for (int i = 0; i < _lines.Count; i++)
        {
            int quantity = int.Parse(_lines[i][1]);
            decimal unitPrice = decimal.Parse(_lines[i][2]);
            total += quantity * unitPrice;
            lineCount++;
        }

        // Застосування знижки постійного клієнта або знижки на велике замовлення
        if (isRegularCustomer == true && total > 1000)
        {
            total *= 0.9m;
        }
        else if (total > 5000)
        {
            total *= 0.85m;
        }

        // Гуртова знижка від 10 позицій у чеку
        if (lineCount > 10)
        {
            total -= 100;
        }

        if (total < 0)
        {
            total = 0;
        }

        // Нарахування ПДВ 20% на підсумкову вартість після врахування знижок
        total += total * 0.2m;
        return Math.Round(total, 2); 
    }

    /// <summary>
    /// Змінює стан замовлення за правилами допустимих переходів предметної області.
    /// </summary>
    /// <param name="newStatus">Новий стан, на який виконується спроба переходу.</param>
    /// <returns>true, якщо перехід успішно здійснено; false, якщо перехід заборонений правилами.</returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == 0 && newStatus == 1)
        {
            Status = 1;
            return true;
        }

        if (Status == 1 && newStatus == 2)
        {
            Status = 2;
            return true;
        }

        if (Status == 0 && newStatus == 3)
        {
            Status = 3;
            return true;
        }

        return false; 
    }

    /// <summary>
    /// Перевіряє коректність заповнення та цілісність даних замовлення.
    /// </summary>
    /// <returns>true, якщо поля замовлення задовольняють бізнес-правилам; інакше false.</returns>
    public bool IsValid()
    {
        if (Id != null
            && Id != string.Empty
            && CustomerName != null
            && CustomerName.Length > 2
            && _lines.Count > 0
            && _lines.Count < 100
            && Status >= 0
            && Status <= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Формує детальний текстовий звіт за позиціями замовлення та фінальною сумою.
    /// </summary>
    /// <returns>Рядок із роздрукованим вмістом замовлення.</returns>
    public string BuildReport()
    {
        string report = string.Empty;

        for (int i = 0; i < _lines.Count; i++)
        {
            int quantity = int.Parse(_lines[i][1]);
            decimal unitPrice = decimal.Parse(_lines[i][2]);
            decimal itemTotal = quantity * unitPrice;

            report += "Товар: " + _lines[i][0]
                + "; кількість: " + _lines[i][1]
                + "; ціна: " + _lines[i][2]
                + "; сума: " + itemTotal + "\n";
        }

        report += "Разом: " + CalculateTotal(false) + "\n";
        return report;
    }
}
