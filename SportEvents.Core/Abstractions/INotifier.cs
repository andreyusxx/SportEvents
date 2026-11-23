using SportEvents.Core.Domain;

namespace SportEvents.Core.Abstractions;

/// <summary>
/// Визначає контракт сповіщення учасника про оформлення або зміну стану заявки.
/// </summary>
public interface INotifier
{
    /// <summary>
    /// Надсилає сповіщення за вказаною заявкою та обчисленою сумою.
    /// </summary>
    /// <param name="registration">Заявка на змагання.</param>
    /// <param name="total">Підсумкова сума внеску.</param>
    void Notify(Registration registration, decimal total);
}
