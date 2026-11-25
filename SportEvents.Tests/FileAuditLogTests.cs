using SportEvents.Core.Infrastructure;
using Xunit;

namespace SportEvents.Tests;

/// <summary>
/// Модульні тести керування ресурсами через IDisposable у класі FileAuditLog.
/// </summary>
public sealed class FileAuditLogTests
{
    /// <summary>
    /// Перевіряє, що блок using закриває файл і зберігає буфер навіть при виникненні винятку.
    /// </summary>
    [Fact]
    public void UsingBlockWhenExceptionThrownFlushesAndReleasesFileHandle()
    {
        string path = Path.Combine(Path.GetTempPath(), $"sportevents_audit_{Guid.NewGuid():N}.log");
        try
        {
            void Act()
            {
                using FileAuditLog audit = new(path);
                audit.Write("Place", 42);
                throw new InvalidOperationException("Імітація збою після запису");
            }

            Assert.Throws<InvalidOperationException>((Action)Act);

            // Файл має бути розблокованим і містити записаний рядок
            string content = File.ReadAllText(path);
            Assert.Contains(";Place;42", content, StringComparison.Ordinal);
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
    /// Перевіряє генерацію ObjectDisposedException при зверненні після Dispose та ідемпотентність Dispose.
    /// </summary>
    [Fact]
    public void WriteAfterDisposeThrowsObjectDisposedException()
    {
        string path = Path.Combine(Path.GetTempPath(), $"sportevents_disposed_{Guid.NewGuid():N}.log");
        try
        {
            FileAuditLog audit = new(path);
            audit.Write("Confirm", 15);
            audit.Dispose();
            audit.Dispose();

            Assert.Throws<ObjectDisposedException>(() => audit.Write("Cancel", 15));
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
