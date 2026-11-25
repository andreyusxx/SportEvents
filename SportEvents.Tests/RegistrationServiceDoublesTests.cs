using SportEvents.Core.Domain;
using SportEvents.Core.Infrastructure;
using SportEvents.Core.Services;
using SportEvents.Tests.Doubles;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести прикладного сервісу RegistrationService із ручними тестовими дублерами.
/// </summary>
public sealed class RegistrationServiceDoublesTests
{
    /// <summary>
    /// Перевіряє, що TotalOf використовує значення, повернуте стабом політики цін, без реальних розрахунків.
    /// </summary>
    [Fact]
    public void TotalOf_WithFixedPricingStub_ReturnsStubbedAmount()
    {
        // Arrange
        InMemoryRegistrationRepositoryFake fakeRepository = new();
        FixedPricingPolicyStub pricingStub = new(420.00m);
        RegistrationService service = new(fakeRepository, pricingStub, new ConsoleNotifier(TextWriter.Null));
        Registration registration = new(101, 12, new DateOnly(2026, 5, 12));
        registration.AddItem(new RegistrationItem("RUN-10K", 2, 500.00m));

        // Act
        decimal actual = service.TotalOf(registration);

        // Assert
        Assert.Equal(420.00m, actual);
    }

    /// <summary>
    /// Перевіряє, що Place зберігає заявку у фейковому сховищі та повертає суму зі стабу.
    /// </summary>
    [Fact]
    public void Place_ValidRegistration_SavesInFakeAndReturnsStubbedTotal()
    {
        // Arrange
        InMemoryRegistrationRepositoryFake fakeRepository = new();
        FixedPricingPolicyStub pricingStub = new(750.00m);
        using StringWriter output = new();
        RegistrationService service = new(fakeRepository, pricingStub, new ConsoleNotifier(output));
        Registration registration = new(102, 15, new DateOnly(2026, 5, 13));
        registration.AddItem(new RegistrationItem("RUN-21K", 1, 900.00m));

        // Act
        decimal actual = service.Place(registration);

        // Assert
        Assert.Equal(750.00m, actual);
        Assert.Equal(1, fakeRepository.Count);
        Assert.Same(registration, fakeRepository.GetById(102));
    }
}
