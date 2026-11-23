namespace SportEvents.Core.Legacy;

/// <summary>
/// Перевантажений інтерфейс сховища на 7 методів, що порушує ISP.
/// </summary>
public interface IRegistrationStore
{
    /// <summary>Зберігає заявку.</summary>
    /// <param name="registration">Заявка.</param>
    void Save(LegacyRegistration registration);

    /// <summary>Повертає заявку за номером.</summary>
    /// <param name="id">Ідентифікатор.</param>
    /// <returns>Знайдена заявка.</returns>
    LegacyRegistration? Load(int id);

    /// <summary>Видаляє заявку.</summary>
    /// <param name="id">Ідентифікатор.</param>
    void Delete(int id);

    /// <summary>Експортує заявки у CSV.</summary>
    /// <param name="path">Шлях до файлу.</param>
    void ExportCsv(string path);

    /// <summary>Імпортує заявки з CSV.</summary>
    /// <param name="path">Шлях до файлу.</param>
    void ImportCsv(string path);

    /// <summary>Надсилає лист підтвердження.</summary>
    /// <param name="email">Пошта.</param>
    /// <param name="body">Текст.</param>
    void SendEmail(string email, string body);

    /// <summary>Очищає кеш.</summary>
    void ClearCache();
}
