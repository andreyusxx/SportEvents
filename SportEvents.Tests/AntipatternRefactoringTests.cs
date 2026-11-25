using SportEvents.Core.Domain;
using SportEvents.Core.Errors;
using SportEvents.Core.Infrastructure;
using SportEvents.Core.Services;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести перевірки виправлених антипатернів обробки винятків.
/// </summary>
public sealed class AntipatternRefactoringTests
{
    /// <summary>
    /// Перевіряє підрахунок коректних кількостей слотів без генерації винятків (int.TryParse).
    /// </summary>
    [Fact]
    public void CountValidQuantitiesSafeFiltersInvalidAndNonPositiveStrings()
    {
        string[] rows = ["2", "дві", "0", "-5", "10", ""];

        int count = RegistrationAntipatternExamples.CountValidQuantitiesSafe(rows);

        Assert.Equal(2, count);
    }

    /// <summary>
    /// Перевіряє збереження початкового стека викликів при повторному прокиданні DomainRuleException через throw.
    /// </summary>
    [Fact]
    public void CancelSafePreservesOriginalStackTraceOnDomainRuleException()
    {
        InMemoryRegistrationRepository repository = new();
        RegistrationAntipatternExamples service = new(repository);

        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => service.CancelSafe(404));

        Assert.Equal("registration.exists", ex.Rule);
        Assert.Contains(nameof(RegistrationAntipatternExamples.LoadSafe), ex.StackTrace, StringComparison.Ordinal);
    }
}
