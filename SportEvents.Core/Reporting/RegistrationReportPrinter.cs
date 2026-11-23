namespace SportEvents.Core.Reporting;

/// <summary>
/// Виконує команду запису сформованого звіту у вказаний текстовий потік виведення.
/// </summary>
public sealed class RegistrationReportPrinter
{
    private readonly TextWriter _output;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistrationReportPrinter"/> class.
    /// Ініціалізує новий екземпляр принтера звітів для заданого потоку виведення.
    /// </summary>
    /// <param name="output">Текстовий потік призначення (консоль, файл або буфер пам'яті).</param>
    public RegistrationReportPrinter(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>
    /// Записує текст звіту у потік виведення без повернення значення.
    /// </summary>
    /// <param name="report">Сформований текст звіту.</param>
    public void Print(string report)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(report);
        _output.Write(report);
    }
}
