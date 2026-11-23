using SportEvents.Core.Domain;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести незмінних доменних типів RegistrationItem та Participant.
/// </summary>
public sealed class ImmutableDomainTypesTests
{
    /// <summary>
    /// Перевіряє відхилення некоректних параметрів у конструкторі RegistrationItem.
    /// </summary>
    [Theory]
    [InlineData("", 1, 100)]
    [InlineData("RUN-10K", 0, 100)]
    [InlineData("RUN-10K", 2, -5)]
    public void RegistrationItemInvalidArgumentsThrowsException(string code, int quantity, decimal unitPrice)
    {
        Assert.ThrowsAny<ArgumentException>(() => new RegistrationItem(code, quantity, unitPrice));
    }

    /// <summary>
    /// Перевіряє коректне обчислення LineTotal та порівняння за значенням для RegistrationItem.
    /// </summary>
    [Fact]
    public void RegistrationItemEqualValuesAreEqualByValue()
    {
        RegistrationItem first = new("RUN-10K", 2, 250m);
        RegistrationItem second = new("RUN-10K", 2, 250m);
        RegistrationItem different = new("RUN-10K", 5, 250m);

        Assert.Equal(500m, first.LineTotal);
        Assert.Equal(first, second);
        Assert.NotEqual(first, different);
    }

    /// <summary>
    /// Перевіряє відхилення некоректних даних учасника у конструкторі Participant.
    /// </summary>
    [Theory]
    [InlineData(0, "Іваненко", "i@ex.com")]
    [InlineData(1, "", "i@ex.com")]
    [InlineData(1, "Іваненко", "invalid-email")]
    public void ParticipantInvalidArgumentsThrowsException(int id, string name, string email)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Participant(id, name, email));
    }
}
