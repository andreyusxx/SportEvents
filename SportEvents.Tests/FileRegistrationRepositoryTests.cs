using SportEvents.Core.Domain;
using SportEvents.Core.Errors;
using SportEvents.Core.Infrastructure;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести файлового сховища FileRegistrationRepository: повторні спроби та атомарність.
/// </summary>
public sealed class FileRegistrationRepositoryTests
{
    /// <summary>
    /// Перевіряє атомарний запис заявки у файл та збереження даних.
    /// </summary>
    [Fact]
    public void AddValidRegistrationWritesAtomicallyToFile()
    {
        string path = Path.Combine(Path.GetTempPath(), $"sportevents_{Guid.NewGuid():N}.csv");
        try
        {
            FileRegistrationRepository repository = new(path);
            Registration registration = new(1, 10, new DateOnly(2026, 5, 10));
            registration.AddItem(new RegistrationItem("RUN-10K", 2, 250m));

            repository.Add(registration);

            Assert.True(File.Exists(path));
            string content = File.ReadAllText(path);
            Assert.Contains("1;10;New;2026-05-10;500.00", content, StringComparison.Ordinal);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    /// <summary>
    /// Перевіряє генерацію StorageException з InnerException та відкат стану при заблокованому файлі.
    /// </summary>
    [Fact]
    public void AddWhenFileLockedThrowsStorageExceptionAndRollsBackState()
    {
        string path = Path.Combine(Path.GetTempPath(), $"sportevents_lock_{Guid.NewGuid():N}.csv");
        string tempPath = $"{path}.tmp";
        try
        {
            // Блокуємо тимчасовий файл ексклюзивним потоком, імітуючи помилку середовища IOException
            using FileStream lockStream = new(tempPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            FileRegistrationRepository repository = new(path);
            Registration registration = new(2, 20, new DateOnly(2026, 5, 11));
            registration.AddItem(new RegistrationItem("RUN-21K", 1, 700m));

            StorageException ex = Assert.Throws<StorageException>(() => repository.Add(registration));

            Assert.Equal("Save", ex.Operation);
            Assert.Equal(path, ex.Target);
            Assert.IsType<IOException>(ex.InnerException);
            Assert.Empty(repository.GetAll());
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
