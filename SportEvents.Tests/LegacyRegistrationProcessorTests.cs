using System.Globalization;
using SportEvents.Legacy;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Характеризаційні тести успадкованого модуля LegacyRegistrationProcessor (фіксують поточну поведінку до рефакторингу).
/// </summary>
public sealed class LegacyRegistrationProcessorTests
{
    /// <summary>
    /// Перевіряє всі чотири повідомлення про помилку при некоректних вхідних даних у Handle.
    /// </summary>
    /// <param name="scenario">Назва сценарію помилки.</param>
    /// <param name="expectedError">Очікуваний код помилки.</param>
    [Theory]
    [InlineData("null_items", "ERR: null")]
    [InlineData("empty_items", "ERR: empty")]
    [InlineData("invalid_state", "ERR: state")]
    [InlineData("invalid_mail", "ERR: mail")]
    public void HandleInvalidInputsReturnsExactLegacyErrorCodes(string scenario, string expectedError)
    {
        LegacyRegistrationProcessor processor = new();
        Athlete athlete = new() { Id = 1, Name = "Іван Петренко", Email = "ivan@sport.ua" };
        List<EventLineItem>? items = scenario switch
        {
            "null_items" => null,
            "empty_items" => new List<EventLineItem>(),
            _ => new List<EventLineItem> { new() { Code = "RUN-10K", Qty = 1, Price = 300m } },
        };
        string state = scenario == "invalid_state" ? "confirmed" : "new";
        string mail = scenario == "invalid_mail" ? "ivan.sport.ua" : "ivan@sport.ua";

        string result = processor.Handle(
            10,
            athlete,
            items!,
            state,
            "UAH",
            false,
            mail,
            string.Empty,
            false,
            0m,
            out decimal grandTotal);

        Assert.Equal(expectedError, result);
        Assert.Equal(0m, grandTotal);
        Assert.Contains(expectedError, processor.DumpLog(), StringComparison.Ordinal);
    }

    /// <summary>
    /// Тест «золотого зразка» (Golden Master): фіксує точний багаторядковий текст звіту Handle та запис у журнал.
    /// </summary>
    [Fact]
    public void HandleValidVipUrgentOrderMatchesGoldenMasterReport()
    {
        CultureInfo previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            LegacyRegistrationProcessor processor = new();
            Athlete athlete = new()
            {
                Id = 7,
                Name = "Олена Коваль",
                Email = "olena@sport.ua",
                Kind = "vip",
                DoneCount = 15,
            };
            List<EventLineItem> items = new()
            {
                new() { Code = "RUN-21K", Qty = 2, Price = 300.00m },
                new() { Code = "MEDAL", Qty = 1, Price = 100.00m },
            };

            // Сума = 700.00 (> 500 -> знижка VIP 15% = 105.00 + бонус 20.00 = 125.00; після знижки 575.00 < 1000 -> термінова доставка 150.00; разом = 725.00)
            string report = processor.Handle(
                42,
                athlete,
                items,
                "new",
                "UAH",
                true,
                "olena@sport.ua",
                "Примітка",
                true,
                20.00m,
                out decimal grandTotal);

            string expectedReport =
                "Документ #42\n" +
                "Клієнт: Олена Коваль\n" +
                "RUN-21K x2 = 600.00 UAH\n" +
                "MEDAL x1 = 100.00 UAH\n" +
                "Знижка: 125.00 UAH\n" +
                "Доставка: 150.00 UAH\n" +
                "Разом: 725.00 UAH\n";

            Assert.Equal(expectedReport, report);
            Assert.Equal(725.00m, grandTotal);
            Assert.Equal("MAIL->olena@sport.ua:725.00\nOK#42\n", processor.DumpLog());
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    /// <summary>
    /// Перевіряє всі три види знижок (vip, partner, лояльний DoneCount &gt; 10) та відсутність знижки.
    /// </summary>
    /// <param name="kind">Тип учасника.</param>
    /// <param name="doneCount">Кількість завершених стартів.</param>
    /// <param name="unitPrice">Ціна одиниці (подвійне значення для InlineData).</param>
    /// <param name="expectedTotal">Очікувана підсумкова сума з урахуванням стандартної доставки 50 грн.</param>
    [Theory]
    [InlineData("vip", 0, 600.0, 560.0)]       // 600 - 15% (90) + 50 доставка = 560
    [InlineData("partner", 0, 400.0, 430.0)]   // 400 - 5% (20) + 50 доставка = 430
    [InlineData("regular", 12, 600.0, 608.0)]  // 600 - 7% (42) + 50 доставка = 608
    [InlineData("regular", 3, 600.0, 650.0)]   // 600 - 0 + 50 доставка = 650
    public void HandleDiscountTiersMatchesLegacyCalculation(
        string kind,
        int doneCount,
        double unitPrice,
        double expectedTotal)
    {
        LegacyRegistrationProcessor processor = new();
        Athlete athlete = new()
        {
            Id = 5,
            Name = "Тарас Шевченко",
            Email = "taras@sport.ua",
            Kind = kind,
            DoneCount = doneCount,
        };
        List<EventLineItem> items = new()
        {
            new() { Code = "RUN-10K", Qty = 1, Price = (decimal)unitPrice },
        };

        _ = processor.Handle(
            1,
            athlete,
            items,
            "draft",
            "UAH",
            false,
            "taras@sport.ua",
            string.Empty,
            false,
            0m,
            out decimal grandTotal);

        Assert.Equal((decimal)expectedTotal, grandTotal);
    }

    /// <summary>
    /// Перевіряє межу безкоштовної доставки: коли сума після знижки дорівнює рівно 1000.00 грн, доставка 0.00.
    /// </summary>
    [Fact]
    public void HandleFreeShippingBoundaryAtExactly1000ChargesZeroShipping()
    {
        LegacyRegistrationProcessor processor = new();
        Athlete athlete = new() { Id = 2, Name = "Марія Бойко", Email = "maria@sport.ua", Kind = "regular", DoneCount = 0 };
        List<EventLineItem> items = new()
        {
            new() { Code = "MARATHON", Qty = 1, Price = 1000.00m },
        };

        _ = processor.Handle(
            2,
            athlete,
            items,
            "new",
            "UAH",
            false,
            "maria@sport.ua",
            string.Empty,
            true,
            0m,
            out decimal grandTotal);

        Assert.Equal(1000.00m, grandTotal);
    }

    /// <summary>
    /// Фіксує поведінку методів Preview та DescribeClient до рефакторингу.
    /// </summary>
    [Fact]
    public void PreviewAndDescribeClientMatchCurrentBehavior()
    {
        CultureInfo previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            LegacyRegistrationProcessor processor = new();
            Athlete athlete = new()
            {
                Id = 9,
                Name = "  андрій шарагін  ",
                Email = "ANDRII@SPORT.UA",
                Kind = "vip",
                DoneCount = 11,
                SinceUtc = new DateTime(DateTime.Now.Year - 3, 1, 1),
            };

            Assert.Equal("EMPTY", processor.Preview(1, athlete, new List<EventLineItem>()));

            string preview = processor.Preview(
                1,
                athlete,
                new List<EventLineItem> { new() { Code = "RUN-10K", Qty = 1, Price = 400.00m } });

            Assert.Contains("Знижка: 40.00 UAH\nДоставка: 50.00 UAH\nРазом: 410.00 UAH\n", preview, StringComparison.Ordinal);
            Assert.Equal("АНДРІЙ ШАРАГІН [VIP] [ЛОЯЛЬНИЙ] <andrii@sport.ua> стаж 3", LegacyRegistrationProcessor.DescribeClient(athlete));
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }
}

