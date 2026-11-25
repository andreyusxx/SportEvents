namespace SportEvents.Core.Errors;

/// <summary>
/// Виняток, що сигналізує про порушення бізнес-правила предметної області SportEvents.
/// </summary>
public sealed class DomainRuleException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainRuleException"/> class.
    /// Ініціалізує новий екземпляр доменного винятку із кодом правила та описом.
    /// </summary>
    /// <param name="rule">Машиночитний код порушеного правила (наприклад, registration.exists).</param>
    /// <param name="message">Зрозуміле повідомлення українською мовою.</param>
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rule);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        Rule = rule;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainRuleException"/> class.
    /// Ініціалізує новий екземпляр доменного винятку із кодом правила, описом та внутрішнім винятком.
    /// </summary>
    /// <param name="rule">Машиночитний код порушеного правила.</param>
    /// <param name="message">Зрозуміле повідомлення українською мовою.</param>
    /// <param name="innerException">Первинний виняток, що спричинив збій.</param>
    public DomainRuleException(string rule, string message, Exception innerException)
        : base(message, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rule);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        ArgumentNullException.ThrowIfNull(innerException);
        Rule = rule;
    }

    /// <summary>
    /// Gets машиночитний ідентифікатор порушеного доменного правила.
    /// </summary>
    public string Rule { get; }
}
