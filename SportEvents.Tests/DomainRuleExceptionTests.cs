using SportEvents.Core.Domain;
using SportEvents.Core.Errors;
using SportEvents.Core.Infrastructure;
using SportEvents.Core.Pricing;
using SportEvents.Core.Services;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести спеціалізованого доменного винятку DomainRuleException.
/// </summary>
public sealed class DomainRuleExceptionTests
{
    /// <summary>
    /// Перевіряє генерацію DomainRuleException з кодом registration.exists при спробі скасувати неіснуючу заявку.
    /// </summary>
    [Fact]
    public void CancelMissingRegistrationThrowsDomainRuleExceptionWithExistsRule()
    {
        RegistrationService service = new(
            new InMemoryRegistrationRepository(),
            new RegularPricingPolicy(),
            new ConsoleNotifier(TextWriter.Null));

        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => service.Cancel(999));

        Assert.Equal("registration.exists", ex.Rule);
        Assert.Contains("999", ex.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Перевіряє генерацію DomainRuleException з кодом registration.cancellable для підтвердженої заявки.
    /// </summary>
    [Fact]
    public void CancelConfirmedRegistrationThrowsDomainRuleExceptionWithCancellableRule()
    {
        InMemoryRegistrationRepository repository = new();
        RegistrationService service = new(
            repository,
            new RegularPricingPolicy(),
            new ConsoleNotifier(TextWriter.Null));

        Registration registration = new(7, 10, new DateOnly(2026, 5, 10));
        registration.AddItem(new RegistrationItem("RUN-10K", 1, 300m));
        service.Place(registration);
        registration.Confirm();

        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => service.Cancel(7));

        Assert.Equal("registration.cancellable", ex.Rule);
    }

    /// <summary>
    /// Перевіряє збереження внутрішнього винятку у конструкторі DomainRuleException.
    /// </summary>
    [Fact]
    public void ConstructorWithInnerExceptionPreservesRuleAndInnerException()
    {
        InvalidOperationException inner = new("Внутрішній збій");
        DomainRuleException ex = new("storage.consistency", "Помилка узгодженості", inner);

        Assert.Equal("storage.consistency", ex.Rule);
        Assert.Same(inner, ex.InnerException);
    }
}
