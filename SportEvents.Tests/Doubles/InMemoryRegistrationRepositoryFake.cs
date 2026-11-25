using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;

namespace SportEvents.Tests.Doubles;

/// <summary>
/// Ручний фейк сховища заявок у пам'яті для ізольованого тестування сервісу без зовнішньої БД чи файлів.
/// </summary>
public sealed class InMemoryRegistrationRepositoryFake : IRegistrationRepository
{
    private readonly Dictionary<int, Registration> _storage = [];

    /// <summary>
    /// Повертає кількість збережених у пам'яті заявок.
    /// </summary>
    public int Count => _storage.Count;

    /// <inheritdoc />
    public void Add(Registration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        if (_storage.ContainsKey(registration.Id))
        {
            throw new InvalidOperationException($"Заявку #{registration.Id} вже збережено.");
        }

        _storage[registration.Id] = registration;
    }

    /// <inheritdoc />
    public Registration? GetById(int id)
        => _storage.TryGetValue(id, out Registration? found) ? found : null;

    /// <inheritdoc />
    public IReadOnlyList<Registration> GetAll()
        => [.. _storage.Values];
}
