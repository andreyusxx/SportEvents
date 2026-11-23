using SportEvents.Core.Domain;
using SportEvents.Core.Legacy;
using SportEvents.Core.Registrations;
using SportEvents.Core.Reporting;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести формування та виведення текстового звіту за заявкою.
/// </summary>
public sealed class RegistrationReportTests
{
    /// <summary>
    /// Перевіряє детерміноване формування тексту звіту чистою функцією TextOf.
    /// </summary>
    [Fact]
    public void TextOfValidResultFormatsExpectedReport()
    {
        TotalRequest request = BuildSampleRequest();
        TotalResult result = RegistrationCalculator.Calculate(request);

        string report = RegistrationReport.TextOf(request, result);

        Assert.Contains("Документ № 1", report, StringComparison.Ordinal);
        Assert.Contains("Клієнт: Іваненко", report, StringComparison.Ordinal);
        Assert.Contains("RUN-10K x 12", report, StringComparison.Ordinal);
        Assert.Contains("Разом: 1292.70", report, StringComparison.Ordinal);
        Assert.Contains("Стан: Confirmed", report, StringComparison.Ordinal);
    }

    /// <summary>
    /// Перевіряє запис сформованого звіту у переданий текстовий потік StringWriter.
    /// </summary>
    [Fact]
    public void PrintWritesReportToOutput()
    {
        TotalRequest request = BuildSampleRequest();
        TotalResult result = RegistrationCalculator.Calculate(request);
        string report = RegistrationReport.TextOf(request, result);

        using StringWriter writer = new();
        RegistrationReportPrinter printer = new(writer);

        printer.Print(report);

        Assert.Equal(report, writer.ToString());
    }

    private static TotalRequest BuildSampleRequest()
        => new(
            RegistrationId: 1,
            Participant: new ParticipantInfo("Іваненко", "i@ex.com", IsLicensed: true),
            Lines:
            [
                new LegacyRegistrationLine { EventCode = "RUN-10K", Quantity = 12, UnitPrice = 100m },
                new LegacyRegistrationLine { EventCode = "MEDAL-PRO", Quantity = 1, UnitPrice = 250m },
            ],
            CreatedAt: new DateOnly(2026, 3, 10),
            PromoCode: string.Empty,
            Status: RegistrationStatus.New,
            DeliveryPrice: 60m);
}
