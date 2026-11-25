using System.Globalization;
using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;
using SportEvents.Core.Errors;

namespace SportEvents.Core.Services;

/// <summary>
/// Демонструє коректну обробку винятків і перевірок після усунення чотирьох антипатернів.
/// </summary>
public sealed class RegistrationAntipatternExamples
{
    private readonly IRegistrationRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistrationAntipatternExamples"/> class.
    /// Ініціалізує екземпляр із посиланням на сховище заявок.
    /// </summary>
    /// <param name="repository">Сховище заявок.</param>
    public RegistrationAntipatternExamples(IRegistrationRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Виправлення антипатерну 4: використання int.TryParse замість try/catch для керування потоком.
    /// </summary>
    /// <param name="rows">Масив рядків із кількістю слотів.</param>
    /// <returns>Кількість коректних додатних значень.</returns>
    public static int CountValidQuantitiesSafe(IReadOnlyList<string> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        int valid = 0;
        foreach (string row in rows)
        {
            if (int.TryParse(row, NumberStyles.Integer, CultureInfo.InvariantCulture, out int quantity)
                && quantity > 0)
            {
                valid++;
            }
        }

        return valid;
    }

    /// <summary>
    /// Виправлення антипатерну 1: не перехоплюємо невідомі збої, а повертаємо результат або кидаємо доменний виняток.
    /// </summary>
    /// <param name="registrationId">Ідентифікатор заявки.</param>
    /// <returns>Знайдена заявка.</returns>
    /// <exception cref="DomainRuleException">Заявку не знайдено у сховищі.</exception>
    public Registration LoadSafe(int registrationId)
    {
        return _repository.GetById(registrationId)
            ?? throw new DomainRuleException(
                "registration.exists",
                $"Заявку #{registrationId} не знайдено.");
    }

    /// <summary>
    /// Виправлення антипатернів 2 і 3: перевірка передумов, конкретний тип винятку та збереження стека через throw.
    /// </summary>
    /// <param name="registrationId">Ідентифікатор заявки.</param>
    /// <exception cref="DomainRuleException">Заявку не знайдено або її неможливо скасувати.</exception>
    /// <exception cref="StorageException">Помилка середовища під час звернення до сховища.</exception>
    public void CancelSafe(int registrationId)
    {
        try
        {
            Registration registration = LoadSafe(registrationId);
            if (registration.Status != RegistrationStatus.New)
            {
                throw new DomainRuleException(
                    "registration.cancellable",
                    $"Заявку #{registrationId} у стані {registration.Status} неможливо скасувати.");
            }

            registration.Cancel();
        }
        catch (IOException ex)
        {
            throw new StorageException(
                "Cancel",
                $"registration:{registrationId}",
                $"Збій введення-виведення під час скасування заявки #{registrationId}.",
                ex);
        }
        catch (DomainRuleException)
        {
            // Зберігаємо початковий стек викликів доменного винятку без зміни його типу
            throw;
        }
    }
}
