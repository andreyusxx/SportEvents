using SportEvents.Core.Legacy;
using SportEvents.Core.Registrations;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести перевірок передумов заявок на участь у змаганнях.
/// </summary>
public sealed class RegistrationGuardsTests
{
    /// <summary>
    /// Перевіряє генерацію ArgumentException у разі передавання порожнього списку позицій заявки.
    /// </summary>
    [Fact]
    public void EnsureLinesValidNoLinesThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            RegistrationGuards.EnsureLinesValid([]));
    }

    /// <summary>
    /// Перевіряє генерацію ArgumentOutOfRangeException у разі нульової кількості слотів у позиції.
    /// </summary>
    [Fact]
    public void EnsureLineValidZeroQuantityThrowsArgumentOutOfRangeException()
    {
        LegacyRegistrationLine line = new()
        {
            EventCode = "RUN-10K",
            Quantity = 0,
            UnitPrice = 10m,
        };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            RegistrationGuards.EnsureLineValid(line));
    }
}
