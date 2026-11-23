using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;

namespace SportEvents.Core.Infrastructure;

/// <summary>
/// Реалізує сховище заявок у оперативній пам'яті на основі словника.
/// </summary>
public sealed class InMemoryRegistrationRepository : IRegistrationRepository
{
    private readonly Dictionary<int, Registration> _storage = new();

    /// <inheritdoc />
    public void Add(Registration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        if (!_storage.TryAdd(registration.Id, registration))
        {
            throw new InvalidOperationException(
                $"Заявку з ідентифікатором {registration.Id} вже зареєстровано у сховищі.");
        }
    }

    /// <inheritdoc />
    public Registration? GetById(int id)
        => _storage.GetValueOrDefault(id);

    /// <inheritdoc />
    public IReadOnlyList<Registration> GetAll()
        => new List<Registration>(_storage.Values);
}
