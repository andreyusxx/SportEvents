using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;
using SportEvents.Core.Errors;
using SportEvents.Core.Infrastructure;
using SportEvents.Core.Pricing;
using SportEvents.Core.Services;

namespace SportEvents.App;

/// <summary>
/// Точка входу консольного застосунку, що демонструє обробку помилок та структуроване логування.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Збирає залежності та демонструє роботу журналу на рівнях Debug, Information, Warning і Error.
    /// </summary>
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        using ILoggerFactory factory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.TimestampFormat = "HH:mm:ss ";
                options.IncludeScopes = true;
            });
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        IRegistrationRepository repository = new InMemoryRegistrationRepository();
        IPricingPolicy pricing = new DiscountPricingPolicy(0.05m);
        INotifier notifier = new ConsoleNotifier(Console.Out);
        RegistrationService service = new(
            repository,
            pricing,
            notifier,
            factory.CreateLogger<RegistrationService>());

        Registration registration = new(101, 10, new DateOnly(2026, 5, 10));
        registration.AddItem(new RegistrationItem("RUN-10K", 1, 500.00m));

        decimal total = service.Place(registration);
        Console.WriteLine($"Підсумкова сума: {total.ToString("0.00", CultureInfo.InvariantCulture)} грн");

        registration.Confirm();
        try
        {
            service.Cancel(101);
        }
        catch (DomainRuleException ex)
        {
            Console.WriteLine($"Очікувана відмова бізнес-правила [{ex.Rule}]: {ex.Message}");
        }
    }
}
