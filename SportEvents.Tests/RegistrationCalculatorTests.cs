using SportEvents.Core.Domain;
using SportEvents.Core.Legacy;
using SportEvents.Core.Registrations;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести чистих підпрограм розрахунку вартості та стану заявки.
/// </summary>
public sealed class RegistrationCalculatorTests
{
    /// <summary>
    /// Перевіряє застосування гуртової знижки 5 % за кількості 10 і більше слотів.
    /// </summary>
    [Fact]
    public void LineSubtotalOfBulkQuantityAppliesDiscount()
    {
        LegacyRegistrationLine line = new() { EventCode = "RUN-10K", Quantity = 12, UnitPrice = 100m };

        decimal subtotal = RegistrationCalculator.LineSubtotalOf(line);

        Assert.Equal(1140m, subtotal);
    }

    /// <summary>
    /// Перевіряє надання підвищеної та базової знижки за обсяг заявки.
    /// </summary>
    [Theory]
    [InlineData(6000, 600)]
    [InlineData(3000, 150)]
    [InlineData(1500, 0)]
    public void VolumeDiscountOfVariousAmountsReturnsExpectedDiscount(decimal amount, decimal expected)
    {
        decimal discount = RegistrationCalculator.VolumeDiscountOf(amount);

        Assert.Equal(expected, discount);
    }

    /// <summary>
    /// Перевіряє обчислення знижки за промокодами SALE10 та MINUS200.
    /// </summary>
    [Theory]
    [InlineData(500, "SALE10", 50)]
    [InlineData(1200, "MINUS200", 200)]
    [InlineData(800, "MINUS200", 0)]
    [InlineData(1000, "", 0)]
    public void PromoDiscountOfPromoCodesReturnsExpectedDiscount(
        decimal amount,
        string promoCode,
        decimal expected)
    {
        decimal discount = RegistrationCalculator.PromoDiscountOf(amount, promoCode);

        Assert.Equal(expected, discount);
    }

    /// <summary>
    /// Перевіряє надання недільної знижки 2 % та її відсутність у будні дні.
    /// </summary>
    [Fact]
    public void WeekendDiscountOfSundayAppliesTwoPercent()
    {
        DateOnly sunday = new(2026, 3, 15);
        DateOnly tuesday = new(2026, 3, 10);

        Assert.Equal(20m, RegistrationCalculator.WeekendDiscountOf(1000m, sunday));
        Assert.Equal(0m, RegistrationCalculator.WeekendDiscountOf(1000m, tuesday));
    }

    /// <summary>
    /// Перевіряє переходи станів заявки у життєвому циклі RegistrationStatus.
    /// </summary>
    [Theory]
    [InlineData(RegistrationStatus.New, 500, RegistrationStatus.Confirmed)]
    [InlineData(RegistrationStatus.Confirmed, 500, RegistrationStatus.Finished)]
    [InlineData(RegistrationStatus.Cancelled, 500, RegistrationStatus.Cancelled)]
    public void NextStatusOfValidTransitionsReturnsExpectedStatus(
        RegistrationStatus current,
        decimal total,
        RegistrationStatus expected)
    {
        RegistrationStatus next = RegistrationCalculator.NextStatusOf(current, total);

        Assert.Equal(expected, next);
    }
}
