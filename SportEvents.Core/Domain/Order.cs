using System;
using System.Collections.Generic;
using System.Linq;
namespace ShopOrders.Domain
{
// клас
public class Order
{
public string Id;
public string CustomerName;
public List<string[]> _lines = new List<string[]>();
public int Status = 0; // 0-новий,1-оплач,2-відпр,3-скасов
public DateTime CreatedAt;
// public string prim; // примітка, поки не треба
// конструктор
public Order(string a, string b)
{
Id = a; // ставимо id
CustomerName = b; // ставимо cl
CreatedAt = DateTime.Now; // ставимо дату
}

// метод додавання
public void AddLine(string sku, int quantity, decimal unitPrice)
{
string[] t = new string[3];
t[0] = sku; t[1] = quantity.ToString(); t[2] = unitPrice.ToString();
_lines.Add(t); // додаємо t у ln
}
// ProcessData
public decimal CalculateTotal(bool isRegularCustomer)
{
decimal total = 0; int lineCount = 0;
for (int i = 0; i < _lines.Count; i++)
{
int k = int.Parse(_lines[i][1]);
decimal c = decimal.Parse(_lines[i][2]);
total = total + k * c; // додаємо до суми
lineCount = lineCount + 1; // збільшуємо kolvo на одиницю
}
// if (sum1 > 500) { sum1 = sum1 - 50; } // стара знижка

// ДОВГИЙ РЯДОК 1: наступні 3 рядки склеїти в один
if (isRegularCustomer == true && total > 1000) { total = total * 0.9m; } else if (total > 5000) { total = total * 0.85m; } else { total = total; }
if (lineCount > 10) total = total - 100;
if (total < 0) total = 0;
total = total + total * 0.2m;
return Math.Round(total, 2); // повертаємо sum1
}
// міняємо статус
public bool TryChangeStatus(int newStatus)
{
if (Status == 0 && newStatus == 1) { Status = 1; return true; }
if (Status == 1 && newStatus == 2) { Status = 2; return true; }
if (Status == 0 && newStatus == 3) { Status = 3; return true; }
return false; // не можна
}
// перевірка
public bool IsValid()
{
// ДОВГИЙ РЯДОК 2: наступні 2 рядки склеїти в один
if (Id != null && Id != "" && CustomerName != null && CustomerName.Length > 2 && _lines.Count > 0 && _lines.Count < 100 && Status >= 0 && Status <= 3)
return true;
else
return false;
}

// звіт
public string BuildReport()
{
string s = "";
for (int i = 0; i < _lines.Count; i++)
{
// ДОВГИЙ РЯДОК 3: наступні 3 рядки склеїти в один
s = s + "Товар: " + _lines[i][0] + "; кількість: " + _lines[i][1] + "; ціна: " + _lines[i][2] + "; сума: " + (int.Parse(_lines[i][1]) * decimal.Parse(_lines[i][2])) + "\n";
}
s = s + "Разом: " + CalculateTotal(false) + "\n";
return s; // повертаємо s
}
// пошук
public static Order FindById(List<Order> orders, string orderId)
{
for (int i = 0; i < orders.Count; i++)
{
if (orders[i].Id == orderId) { return orders[i]; }
}
return null;
}
}
}