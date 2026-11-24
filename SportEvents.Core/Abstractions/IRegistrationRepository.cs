using SportEvents.Core.Domain;

namespace SportEvents.Core.Abstractions;

/// <summary>
/// Визначає контракт сховища для збереження та отримання заявок на участь у змаганнях.
/// </summary>
public interface IRegistrationRepository
{
    /// <summary>
    /// Додає нову заявку до сховища.
    /// </summary>
    /// <param name="registration">Агрегат заявки для збереження.</param>
    void Add(Registration registration);

    /// <summary>
    /// Повертає заявку за її унікальним ідентифікатором або null, якщо заявку не знайдено.
    /// </summary>
    /// <param name="id">Ідентифікатор заявки.</param>
    /// <returns>Знайдена заявка або null.</returns>
    Registration? GetById(int id);

    /// <summary>
    /// Повертає копію переліку всіх збережених заявок.
    /// </summary>
    /// <returns>Список заявок лише для читання.</returns>
    IReadOnlyList<Registration> GetAll();
}
