using System.Globalization;
using SportEvents.Core.Domain;

Registration registration = new()
{
    Id = 1,
    ParticipantId = 10,
    CreatedAt = DateTimeOffset.Now,
};

registration.AddItem(new RegistrationItem
{
    Code = "DISC-001",
    Quantity = 2,
    UnitPrice = 250.00m,
});

registration.AddItem(new RegistrationItem
{
    Code = "DISC-002",
    Quantity = 1,
    UnitPrice = 150.50m,
});

string total = registration.Total()
    .ToString("F2", CultureInfo.InvariantCulture);

Console.WriteLine($"Реєстрація #{registration.Id}");
Console.WriteLine($"Стан: {registration.Status}");
Console.WriteLine($"Позицій: {registration.Items.Count}");
Console.WriteLine($"Сума: {total}");
