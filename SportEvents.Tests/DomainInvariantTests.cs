using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;
using SportEvents.Core.Infrastructure;
using SportEvents.Core.Pricing;
using SportEvents.Core.Services;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести перевірки ключових інваріантів моделі та політик ціноутворення SportEvents.
/// </summary>
public sealed class DomainInvariantTests
{
    /// <summary>
    /// Перевіряє відхилення створення позиції заявки з нульовою кількістю слотів.
    /// </summary>
    [Fact]
    public void RegistrationItemWithZeroQuantityThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new RegistrationItem("RUN-10K", 0, 100m));
    }

    /// <summary>
    /// Перевіряє заборону додавання нових позицій до заявки після її підтвердження.
    /// </summary>
    [Fact]
    public void RegistrationAfterConfirmRejectsNewItems()
    {
        Registration registration = new(1, 10, new DateOnly(2026, 5, 10));
        registration.AddItem(new RegistrationItem("RUN-10K", 1, 300m));
        registration.Confirm();

        Assert.Throws<InvalidOperationException>(
            () => registration.AddItem(new RegistrationItem("RUN-21K", 1, 500m)));
    }

    /// <summary>
    /// Перевіряє розрахунок підсумкової суми заявки з підставленою політикою знижок у пам'яті.
    /// </summary>
    [Fact]
    public void RegistrationServiceWithDiscountPolicyCalculatesDiscountedTotal()
    {
        IRegistrationRepository repository = new InMemoryRegistrationRepository();
        IPricingPolicy pricing = new DiscountPricingPolicy(0.10m);
        INotifier notifier = new ConsoleNotifier(TextWriter.Null);
        RegistrationService service = new(repository, pricing, notifier);

        Registration registration = new(1, 10, new DateOnly(2026, 5, 10));
        registration.AddItem(new RegistrationItem("RUN-10K", 2, 250m));

        decimal total = service.TotalOf(registration);

        Assert.Equal(450.00m, total);
    }
}
