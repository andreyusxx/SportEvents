using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;
using SportEvents.Core.Infrastructure;
using SportEvents.Core.Pricing;
using SportEvents.Core.Services;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести прикладного сервісу RegistrationService, сховища та політик цін.
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

        Assert.Throws<ArgumentNullException>(() => new RegistrationService(null!, pricing));
        Assert.Throws<ArgumentNullException>(() => new RegistrationService(repository, null!));
    }

    /// <summary>
    /// Перевіряє підміну політики ціноутворення без зміни коду сервісу.
    /// </summary>
    [Fact]
    public void TotalOfDifferentPricingPoliciesCalculatesExpectedTotal()
    {
        Registration registration = new(1, 10, new DateOnly(2026, 5, 10));
        registration.AddItem(new RegistrationItem("RUN-10K", 2, 250m));

        RegistrationService regularService = new(
            new InMemoryRegistrationRepository(),
            new RegularPricingPolicy());
        RegistrationService discountService = new(
            new InMemoryRegistrationRepository(),
            new DiscountPricingPolicy(0.05m));

        Assert.Equal(500.00m, regularService.TotalOf(registration));
        Assert.Equal(475.00m, discountService.TotalOf(registration));
    }

    /// <summary>
    /// Перевіряє збереження заявки у сховищі в пам'яті та захист від дублікатів ідентифікаторів.
    /// </summary>
    [Fact]
    public void PlaceDuplicateRegistrationIdThrowsInvalidOperationException()
    {
        InMemoryRegistrationRepository repository = new();
        RegistrationService service = new(repository, new RegularPricingPolicy());

        Registration first = new(1, 10, new DateOnly(2026, 5, 10));
        first.AddItem(new RegistrationItem("RUN-10K", 1, 300m));
        service.Place(first);

        Registration duplicate = new(1, 20, new DateOnly(2026, 5, 11));
        duplicate.AddItem(new RegistrationItem("RUN-21K", 1, 500m));

        Assert.Throws<InvalidOperationException>(() => service.Place(duplicate));
        Assert.Single(repository.GetAll());
    }
}
