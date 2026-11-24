using SportEvents.Core.Domain;
using SportEvents.Core.Parsing;
using SportEvents.Core.Results;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести барикади введення даних RegistrationRequestBarricade та типу Result.
/// </summary>
public sealed class RegistrationRequestBarricadeTests
{
    private static readonly Dictionary<string, decimal> _sampleCatalog = new(StringComparer.Ordinal)
    {
        ["RUN-10K"] = 450.00m,
        ["RUN-21K"] = 700.00m,
    };

    /// <summary>
    /// Перевіряє повернення Result.Fail для нечислової кількості слотів без генерації винятку.
    /// </summary>
    [Fact]
    public void ToRegistrationItemNonNumericQuantityReturnsFailResult()
    {
        RegistrationInputRequest request = new("10", "RUN-10K", "дві");

        Result<RegistrationItem> result =
            RegistrationRequestBarricade.ToRegistrationItem(request, _sampleCatalog);

        Assert.False(result.IsSuccess);
        Assert.Contains("цілим числом", result.Error, StringComparison.Ordinal);
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    /// <summary>
    /// Перевіряє успішне перетворення коректного зовнішнього запиту на незмінний RegistrationItem.
    /// </summary>
    [Fact]
    public void ToRegistrationItemValidInputReturnsCleanDomainItem()
    {
        RegistrationInputRequest request = new("10", "RUN-21K", "3");

        Result<RegistrationItem> result =
            RegistrationRequestBarricade.ToRegistrationItem(request, _sampleCatalog);

        Assert.True(result.IsSuccess);
        Assert.Equal("RUN-21K", result.Value.Code);
        Assert.Equal(3, result.Value.Quantity);
        Assert.Equal(2100.00m, result.Value.LineTotal);
    }

    /// <summary>
    /// Перевіряє розбір десяткового внеску з крапкою як роздільником дробової частини.
    /// </summary>
    [Fact]
    public void ParseEntryFeeInvariantDotSeparatorParsesSuccessfully()
    {
        Result<decimal> valid = RegistrationRequestBarricade.ParseEntryFee("450.75");
        Result<decimal> invalidComma = RegistrationRequestBarricade.ParseEntryFee("450,75");

        Assert.True(valid.IsSuccess);
        Assert.Equal(450.75m, valid.Value);
        Assert.False(invalidComma.IsSuccess);
    }
}
