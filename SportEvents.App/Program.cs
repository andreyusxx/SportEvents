using System.Text;
using SportEvents.Core.Domain;
using SportEvents.Core.Legacy;
using SportEvents.Core.Registrations;
using SportEvents.Core.Reporting;

Console.OutputEncoding = Encoding.UTF8;

TotalRequest request = new(
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

TotalResult result = RegistrationCalculator.Calculate(request);
string reportText = RegistrationReport.TextOf(request, result);

RegistrationReportPrinter printer = new(Console.Out);
printer.Print(reportText);
