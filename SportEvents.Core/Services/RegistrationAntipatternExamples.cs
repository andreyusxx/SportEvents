#pragma warning disable CS0168, CA1031, CA2200, CA1806, CA1062
using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;

namespace SportEvents.Core.Services;

/// <summary>
/// Навчальний клас із чотирма антипатернами обробки винятків до рефакторингу.
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
        _repository = repository;
    }

    /// <summary>
    /// АНТИПАТЕРН 1: проковтнутий виняток (порожній catch) і повернення null.
    /// </summary>
    /// <param name="registrationId">Ідентифікатор заявки.</param>
    /// <returns>Знайдена заявка або null.</returns>
    public Registration? LoadUnsafe(int registrationId)
    {
        try
        {
            return _repository.GetById(registrationId);
        }
        catch (Exception)
        {
        }

        return null;
    }

    /// <summary>
    /// АНТИПАТЕРНИ 2 і 3: широке перехоплення catch (Exception) та втрата стека через throw ex.
    /// </summary>
    /// <param name="registrationId">Ідентифікатор заявки.</param>
    public void CancelUnsafe(int registrationId)
    {
        try
        {
            Registration? registration = _repository.GetById(registrationId);
            registration!.Cancel();
        }
        catch (Exception ex)
        {
            _ = "Щось пішло не так";
            throw ex;
        }
    }

    /// <summary>
    /// АНТИПАТЕРН 4: винятки як керування потоком (try/catch замість int.TryParse).
    /// </summary>
    /// <param name="rows">Масив рядків із кількістю слотів.</param>
    /// <returns>Кількість коректних додатних значень.</returns>
    public int CountValidQuantitiesUnsafe(string[] rows)
    {
        _ = _repository;
        int valid = 0;
        foreach (string row in rows)
        {
            try
            {
                int q = int.Parse(row, System.Globalization.CultureInfo.InvariantCulture);
                if (q > 0)
                {
                    valid++;
                }
            }
            catch (FormatException)
            {
                // не число — пропускаємо
            }
        }

        return valid;
    }
}
