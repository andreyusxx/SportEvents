using System.Globalization;
using System.Text;
using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;
using SportEvents.Core.Infrastructure;
using SportEvents.Core.Pricing;
using SportEvents.Core.Reporting;
using SportEvents.Core.Services;

namespace SportEvents.App;

/// <summary>
/// Точка входу консольного застосунку, що виконує роль композиційного кореня.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Збирає граф залежностей та виконує демонстраційний сценарій реєстрації на змагання.
    /// </summary>
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        IRegistrationRepository repository = new InMemoryRegistrationRepository();
        IPricingPolicy pricing = new DiscountPricingPolicy(0.05m);
        INotifier notifier = new ConsoleNotifier(Console.Out);
        RegistrationService service = new(repository, pricing, notifier);

        Registration registration = new(1, 100, DateOnly.FromDateTime(DateTime.Today));
        registration.AddItem(new RegistrationItem("RUN-10K", 2, 250m));

        decimal total = service.Place(registration);
        string formattedTotal = total.ToString("0.00", CultureInfo.CurrentCulture);
        Console.WriteLine($"Сума: {formattedTotal}");
        Console.WriteLine($"CSV: {RegistrationCsvReport.BuildCsv(registration, total)}");
    }
}
