using System.Globalization;
using SportEvents.Core.Abstractions;
using SportEvents.Core.Domain;

namespace SportEvents.Core.Infrastructure;

/// <summary>
/// Реалізує надсилання текстових сповіщень про реєстрацію у вказаний потік виведення.
/// </summary>
public sealed class ConsoleNotifier : INotifier
{
    private readonly TextWriter _output;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleNotifier"/> class.
    /// Ініціалізує новий екземпляр сповіщувача для заданого потоку.
    /// </summary>
    /// <param name="output">Текстовий потік виведення.</param>
    public ConsoleNotifier(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <inheritdoc />
    public void Notify(Registration registration, decimal total)
    {
        ArgumentNullException.ThrowIfNull(registration);
        string message = string.Format(
            CultureInfo.InvariantCulture,
            "Заявку #{0} для учасника #{1} оформлено. Сума: {2:F2}",
            registration.Id,
            registration.ParticipantId,
            total);
        _output.WriteLine(message);
    }
}
