using SportEvents.Core.Legacy;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Характеризаційні тести для фіксації поточної поведінки успадкованого методу розрахунку.
/// </summary>
public sealed class LegacyProcessorTests
{
    /// <summary>
    /// Перевіряє розрахунок заявки для ліцензованого учасника з гуртовою позицією та переходом у стан Paid.
    /// </summary>
    [Fact]
    public void ProcessLicensedParticipantReturns1292()
    {
        List<LegacyRegistrationLine> lines =
        [
            new LegacyRegistrationLine { EventCode = "RUN-10K", Quantity = 12, UnitPrice = 100m },
            new LegacyRegistrationLine { EventCode = "MEDAL-PRO", Quantity = 1, UnitPrice = 250m },
        ];

        decimal total = LegacyProcessor.Process(
            1,
            "Іваненко",
            "i@ex.com",
            true,
            lines,
            new DateOnly(2026, 3, 10),
            string.Empty,
            "New",
            false,
            60m,
            out string next);

        Assert.Equal(1292.70m, total);
        Assert.Equal("Paid", next);
    }

    /// <summary>
    /// Перевіряє застосування промокоду SALE10, додавання вартості доставки та перехід у стан Shipped.
    /// </summary>
    [Fact]
    public void ProcessSmallRegistrationAddsDelivery()
    {
        List<LegacyRegistrationLine> lines =
        [
            new LegacyRegistrationLine { EventCode = "RUN-10K", Quantity = 2, UnitPrice = 200m },
        ];

        decimal total = LegacyProcessor.Process(
            2,
            "Петренко",
            "p@ex.com",
            false,
            lines,
            new DateOnly(2026, 3, 10),
            "SALE10",
            "Paid",
            false,
            60m,
            out string next);

        Assert.Equal(420m, total);
        Assert.Equal("Shipped", next);
    }

    /// <summary>
    /// Перевіряє повернення коду помилки -1 за порожнього переліку позицій заявки.
    /// </summary>
    [Fact]
    public void ProcessNoLinesReturnsMinusOne()
    {
        decimal total = LegacyProcessor.Process(
            3,
            "Коваль",
            "k@ex.com",
            false,
            [],
            new DateOnly(2026, 3, 10),
            string.Empty,
            "New",
            false,
            60m,
            out string _);

        Assert.Equal(-1m, total);
    }
}