using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using SportEvents.Core;
using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;
using SportEvents.Core.Errors;
using SportEvents.Core.Infrastructure;
using SportEvents.Core.Pricing;
using SportEvents.Core.Services;

namespace SportEvents.App;

/// <summary>
/// Точка входу консольного застосунку, що демонструє обробку помилок, логування та IDisposable.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Збирає залежності та демонструє роботу журналу і гарантоване звільнення ресурсів через using.
    /// </summary>
    private static void Main()
    {
        Console.WriteLine($"SportEvents v{VersionInfo.Current()}");
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

        string auditPath = Path.Combine(Path.GetTempPath(), "sportevents_audit.log");
        using FileAuditLog auditLog = new(auditPath);

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
        auditLog.Write("Place", registration.Id);
        Console.WriteLine($"Підсумкова сума: {total.ToString("0.00", CultureInfo.InvariantCulture)} грн");

        registration.Confirm();
        auditLog.Write("Confirm", registration.Id);
        try
        {
            service.Cancel(101);
        }
        catch (DomainRuleException ex)
        {
            auditLog.Write("CancelRejected", registration.Id);
            Console.WriteLine($"Очікувана відмова бізнес-правила [{ex.Rule}]: {ex.Message}");
        }
    }
}
