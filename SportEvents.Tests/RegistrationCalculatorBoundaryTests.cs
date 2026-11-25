using SportEvents.Core.Registrations;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Параметризовані та межові модульні тести для RegistrationCalculator.
/// </summary>
public sealed class RegistrationCalculatorBoundaryTests
{
    /// <summary>
    /// Перевіряє розрахунок обсягової знижки на всіх класах еквівалентності та граничних значеннях.
    /// </summary>
    /// <param name="amount">Вхідна сума заявки у гривнях.</param>
    /// <param name="expected">Очікувана сума знижки у гривнях.</param>
    [Theory]
    [InlineData(0.00, 0.00)]
    [InlineData(1500.00, 0.00)]
    [InlineData(2000.00, 0.00)]
    [InlineData(2000.01, 100.0005)]
    [InlineData(5000.00, 250.00)]
    [InlineData(5000.01, 500.001)]
    public void VolumeDiscountOf_Boundaries_MatchesTable(double amount, double expected)
    {
        // Arrange
        decimal decimalAmount = (decimal)amount;
        decimal expectedDiscount = (decimal)expected;

        // Act
        decimal actual = RegistrationCalculator.VolumeDiscountOf(decimalAmount);

        // Assert
        Assert.Equal(expectedDiscount, actual);
    }

    /// <summary>
    /// Перевіряє викидання винятку ArgumentOutOfRangeException із коректним ParamName при від'ємній сумі.
    /// </summary>
    /// <param name="amount">Недопустима від'ємна сума.</param>
    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1000.00)]
    public void VolumeDiscountOf_NegativeAmount_ThrowsOutOfRange(double amount)
    {
        // Arrange
        decimal negativeAmount = (decimal)amount;

        // Act
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => RegistrationCalculator.VolumeDiscountOf(negativeAmount));

        // Assert
        Assert.Equal("amount", ex.ParamName);
    }

    /// <summary>
    /// Перевіряє точну межу гуртової кількості: 10 слотів у позиції отримують знижку 5 % (2375,00 грн).
    /// </summary>
    [Fact]
    public void LineSubtotalOf_ExactBulkThreshold_AppliesBulkDiscount()
    {
        // Arrange
        RegistrationLine line = new() { EventCode = "RUN-10K", Quantity = 10, UnitPrice = 250.00m };

        // Act
        decimal actual = RegistrationCalculator.LineSubtotalOf(line);

        // Assert
        Assert.Equal(2375.00m, actual);
    }
}
