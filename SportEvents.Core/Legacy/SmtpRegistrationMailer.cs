namespace SportEvents.Core.Legacy;

/// <summary>
/// Конкретний поштовий відправник, створений безпосередньо в класі-Бозі.
/// </summary>
public sealed class SmtpRegistrationMailer
{
    private readonly string _host = "smtp.sportevents.local";

    /// <summary>
    /// Надсилає електронний лист.
    /// </summary>
    /// <param name="email">Адреса отримувача.</param>
    /// <param name="subject">Тема листа.</param>
    /// <param name="body">Тіло листа.</param>
    public void Send(string email, string subject, string body)
    {
        _ = $"{_host}:{email}:{subject}:{body}";
    }
}
