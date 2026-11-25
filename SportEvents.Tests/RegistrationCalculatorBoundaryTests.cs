using SportEvents.Core.Registrations;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести класів еквівалентності та граничних значень для RegistrationCalculator.
/// </summary>
public sealed class RegistrationCalculatorBoundaryTests
{
    /// <summary>
    /// Перевіряє нижню межу класу 0,00 грн у розрахунку обсягової знижки.
    /// </summary>
    [Fact]
    public void VolumeDiscountOf_ZeroAmount_ReturnsZero()
    {
        // Arrange
        decimal amount = 0.00m;

        // Act
        decimal actual = RegistrationCalculator.VolumeDiscountOf(amount);

        // Assert
        Assert.Equal(0.00m, actual);
    }

    /// <summary>
    /// Перевіряє верхню межу базового класу: рівно 2000,00 грн ще не дає знижки (0,00 грн).
    /// </summary>
    [Fact]
    public void VolumeDiscountOf_Exact2000_ReturnsZero()
    {
        // Arrange
        decimal amount = 2000.00m;

        // Act
        decimal actual = RegistrationCalculator.VolumeDiscountOf(amount);

        // Assert
        Assert.Equal(0.00m, actual);
    }

    /// <summary>
    /// Перевіряє нижню межу знижки 5 %: 2000,01 грн дає знижку 100,0005 грн.
    /// </summary>
    [Fact]
    public void VolumeDiscountOf_JustAbove2000_ReturnsFivePercent()
    {
        // Arrange
        decimal amount = 2000.01m;

        // Act
        decimal actual = RegistrationCalculator.VolumeDiscountOf(amount);

        // Assert
        Assert.Equal(100.0005m, actual);
    }

    /// <summary>
    /// Перевіряє верхню межу класу знижки 5 %: рівно 5000,00 грн дає 5 % (250,00 грн).
    /// </summary>
    [Fact]
    public void VolumeDiscountOf_Exact5000_ReturnsFivePercent()
    {
        // Arrange
        decimal amount = 5000.00m;

        // Act
        decimal actual = RegistrationCalculator.VolumeDiscountOf(amount);

        // Assert
        Assert.Equal(250.00m, actual);
    }

    /// <summary>
    /// Перевіряє нижню межу знижки 10 %: 5000,01 грн дає знижку 500,001 грн.
    /// </summary>
    [Fact]
    public void VolumeDiscountOf_JustAbove5000_ReturnsTenPercent()
    {
        // Arrange
        decimal amount = 5000.01m;

        // Act
        decimal actual = RegistrationCalculator.VolumeDiscountOf(amount);

        // Assert
        Assert.Equal(500.001m, actual);
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
