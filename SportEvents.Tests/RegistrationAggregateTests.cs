using SportEvents.Core.Domain;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести інкапсуляції та машини станів агрегату Registration.
/// </summary>
public sealed class RegistrationAggregateTests
{
    /// <summary>
    /// Перевіряє заборону підтвердження порожньої заявки без жодної позиції.
    /// </summary>
    [Fact]
    public void ConfirmEmptyRegistrationThrowsInvalidOperationException()
    {
        Registration registration = new(1, 10, new DateOnly(2026, 5, 10));

        Assert.Throws<InvalidOperationException>(() => registration.Confirm());
    }

    /// <summary>
    /// Перевіряє заборону додавання нових позицій після підтвердження заявки.
    /// </summary>
    [Fact]
    public void AddItemAfterConfirmThrowsInvalidOperationException()
    {
        Registration registration = new(1, 10, new DateOnly(2026, 5, 10));
        registration.AddItem(new RegistrationItem("RUN-10K", 1, 500m));
        registration.Confirm();

        Assert.Throws<InvalidOperationException>(() =>
            registration.AddItem(new RegistrationItem("MEDAL", 1, 150m)));
    }

    /// <summary>
    /// Перевіряє успішний життєвий цикл New -> Confirmed -> Finished та заборону повернення у New чи Cancel.
    /// </summary>
    [Fact]
    public void LifecycleNewToConfirmedToFinishedForbidsReverseTransitions()
    {
        Registration registration = new(1, 10, new DateOnly(2026, 5, 10));
        registration.AddItem(new RegistrationItem("RUN-21K", 2, 700m));

        registration.Confirm();
        Assert.Equal(RegistrationStatus.Confirmed, registration.Status);

        registration.Finish();
        Assert.Equal(RegistrationStatus.Finished, registration.Status);

        Assert.Throws<InvalidOperationException>(() => registration.Cancel());
        Assert.Throws<InvalidOperationException>(() => registration.Disqualify());
    }
}
