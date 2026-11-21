using System.Globalization;
using SportEvents.Core.Domain;

namespace SportEvents.App;

/// <summary>Точка входу консольного застосунку обліку змагань SportEvents.</summary>
internal static class Program
{
    /// <summary>Запускає демонстрацію доменної моделі та розрахунку результатів забігу.</summary>
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var participant = new Participant
        {
            Id = 10,
            Name = "Шарагін Андрій",
            IsLicensed = true,
        };

        var registration = new Registration
        {
            Id = 1,
            ParticipantId = participant.Id,
            CreatedAt = new DateTimeOffset(2026, 5, 10, 9, 0, 0, TimeSpan.FromHours(3)),
        };

        registration.AddItem(new RegistrationItem
        {
            Code = "RUN-10K",
            Quantity = 1,
            UnitPrice = 450.00m,
        });

        registration.AddItem(new RegistrationItem
        {
            Code = "MEDAL-ENGRAVING",
            Quantity = 1,
            UnitPrice = 150.50m,
        });

        string formattedTotal = registration.Total()
            .ToString("F2", CultureInfo.InvariantCulture);

        Console.WriteLine($"Учасник: {participant.Name} (ліцензія: {participant.IsLicensed})");
        Console.WriteLine($"Реєстрація #{registration.Id}, статус: {registration.Status}");
        Console.WriteLine($"Сума внеску: {formattedTotal} грн");

        var chipStartAt = new DateTimeOffset(2026, 5, 10, 10, 0, 0, TimeSpan.FromHours(3));
        DateTimeOffset finishAt = chipStartAt
            .AddMinutes(42)
            .AddSeconds(18)
            .AddMilliseconds(347);
        var penaltyDuration = TimeSpan.FromSeconds(15);

        var raceResult = new RaceResult(chipStartAt, finishAt, penaltyDuration);

        Console.WriteLine($"Офіційний час дистанції: {raceResult.FormatOfficialDuration()}");
    }
}
