using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;
using SportEvents.Core.Infrastructure;
using SportEvents.Core.Pricing;
using SportEvents.Core.Reporting;
using SportEvents.Core.Services;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести прикладного сервісу RegistrationService, сховища, політик цін та принципів SOLID.
/// </summary>
public sealed class RegistrationServiceTests
{
    /// <summary>
    /// Перевіряє відхилення null-залежностей у конструкторі RegistrationService.
    /// </summary>
    [Fact]
    public void ConstructorNullDependenciesThrowsArgumentNullException()
    {
        IRegistrationRepository repository = new InMemoryRegistrationRepository();
        IPricingPolicy pricing = new RegularPricingPolicy();
        INotifier notifier = new ConsoleNotifier(TextWriter.Null);

        Assert.Throws<ArgumentNullException>(() => new RegistrationService(null!, pricing, notifier));
        Assert.Throws<ArgumentNullException>(() => new RegistrationService(repository, null!, notifier));
        Assert.Throws<ArgumentNullException>(() => new RegistrationService(repository, pricing, null!));
    }

    /// <summary>
    /// Перевіряє підміну політики ціноутворення без зміни коду сервісу (OCP).
    /// </summary>
    [Fact]
    public void TotalOfDifferentPricingPoliciesCalculatesExpectedTotal()
    {
        Registration registration = new(1, 10, new DateOnly(2026, 5, 10));
        registration.AddItem(new RegistrationItem("RUN-10K", 2, 250m));

        RegistrationService regularService = new(
            new InMemoryRegistrationRepository(),
            new RegularPricingPolicy(),
            new ConsoleNotifier(TextWriter.Null));
        RegistrationService discountService = new(
            new InMemoryRegistrationRepository(),
            new DiscountPricingPolicy(0.05m),
            new ConsoleNotifier(TextWriter.Null));

        Assert.Equal(500.00m, regularService.TotalOf(registration));
        Assert.Equal(475.00m, discountService.TotalOf(registration));
    }

    /// <summary>
    /// Перевіряє збереження заявки, надсилання сповіщення та захист від дублікатів (SRP, ISP, DIP).
    /// </summary>
    [Fact]
    public void PlaceValidRegistrationSavesNotifiesAndRejectsDuplicate()
    {
        InMemoryRegistrationRepository repository = new();
        using StringWriter output = new();
        RegistrationService service = new(
            repository,
            new RegularPricingPolicy(),
            new ConsoleNotifier(output));

        Registration first = new(1, 10, new DateOnly(2026, 5, 10));
        first.AddItem(new RegistrationItem("RUN-10K", 1, 300m));
        decimal total = service.Place(first);

        Assert.Equal(300.00m, total);
        Assert.Contains("300.00", output.ToString(), StringComparison.Ordinal);

        Registration duplicate = new(1, 20, new DateOnly(2026, 5, 11));
        duplicate.AddItem(new RegistrationItem("RUN-21K", 1, 500m));

        Assert.Throws<InvalidOperationException>(() => service.Place(duplicate));
        Assert.Single(repository.GetAll());
    }

    /// <summary>
    /// Перевіряє архівування підтвердженої заявки через композицію та формування CSV (LSP, SRP).
    /// </summary>
    [Fact]
    public void ArchivedRegistrationWrapsConfirmedDocumentAndBuildsCsv()
    {
        Registration registration = new(5, 42, new DateOnly(2026, 5, 10));
        registration.AddItem(new RegistrationItem("RUN-21K", 1, 600m));
        registration.Confirm();

        ArchivedRegistration archived = new(registration, DateTimeOffset.UtcNow);
        string csv = RegistrationCsvReport.BuildCsv(archived.Document, 600m);

        Assert.Equal(5, archived.Document.Id);
        Assert.Equal("5;42;Confirmed;600.00", csv);
    }
}
