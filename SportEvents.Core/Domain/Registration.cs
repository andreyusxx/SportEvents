namespace SportEvents.Core.Domain;

/// <summary>
/// Представляє агрегат заявки учасника на участь у спортивних змаганнях.
/// </summary>
public sealed class Registration
{
    private readonly List<RegistrationItem> _items = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Registration"/> class.
    /// Ініціалізує новий екземпляр заявки на реєстрацію з перевіркою інваріантів.
    /// </summary>
    /// <param name="id">Унікальний ідентифікатор заявки.</param>
    /// <param name="participantId">Ідентифікатор зареєстрованого учасника.</param>
    /// <param name="createdAt">Дата створення заявки.</param>
    /// <exception cref="ArgumentOutOfRangeException">Ідентифікатор заявки або учасника менший або дорівнює нулю.</exception>
    public Registration(int id, int participantId, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(participantId);

        Id = id;
        ParticipantId = participantId;
        CreatedAt = createdAt;
        Status = RegistrationStatus.New;
    }

    /// <summary>
    /// Gets отримує унікальний ідентифікатор заявки.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets отримує ідентифікатор учасника, якому належить заявка.
    /// </summary>
    public int ParticipantId { get; }

    /// <summary>
    /// Gets отримує дату оформлення заявки.
    /// </summary>
    public DateOnly CreatedAt { get; }

    /// <summary>
    /// Gets отримує поточний стан заявки в життєвому циклі змагань.
    /// </summary>
    public RegistrationStatus Status { get; private set; }

    /// <summary>
    /// Gets отримує перелік позицій заявки лише для читання.
    /// </summary>
    public IReadOnlyList<RegistrationItem> Items => _items;

    /// <summary>
    /// Додає нову позицію до заявки, якщо вона перебуває у стані <see cref="RegistrationStatus.New"/>.
    /// </summary>
    /// <param name="item">Незмінна позиція заявки.</param>
    /// <exception cref="ArgumentNullException">Передано null замість позиції.</exception>
    /// <exception cref="InvalidOperationException">Заявка вже підтверджена, завершена або скасована.</exception>
    public void AddItem(RegistrationItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (Status != RegistrationStatus.New)
        {
            throw new InvalidOperationException("Позиції дозволено додавати лише до нової заявки.");
        }

        _items.Add(item);
    }

    /// <summary>
    /// Переводить непорожню нову заявку у стан <see cref="RegistrationStatus.Confirmed"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Стан не дорівнює New або у заявці відсутні позиції.</exception>
    public void Confirm()
    {
        if (Status != RegistrationStatus.New)
        {
            throw new InvalidOperationException($"Перехід зі стану {Status} у Confirmed заборонено.");
        }

        if (_items.Count == 0)
        {
            throw new InvalidOperationException("Неможливо підтвердити порожню заявку без позицій.");
        }

        Status = RegistrationStatus.Confirmed;
    }

    /// <summary>
    /// Скасовує нову заявку, переводячи її у стан <see cref="RegistrationStatus.Cancelled"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Стан заявки не дорівнює New.</exception>
    public void Cancel()
    {
        if (Status != RegistrationStatus.New)
        {
            throw new InvalidOperationException($"Перехід зі стану {Status} у Cancelled заборонено.");
        }

        Status = RegistrationStatus.Cancelled;
    }

    /// <summary>
    /// Фіксує завершення участі у змаганнях переведенням у стан <see cref="RegistrationStatus.Finished"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Заявка не перебуває у стані Confirmed.</exception>
    public void Finish()
    {
        if (Status != RegistrationStatus.Confirmed)
        {
            throw new InvalidOperationException($"Перехід зі стану {Status} у Finished заборонено.");
        }

        Status = RegistrationStatus.Finished;
    }

    /// <summary>
    /// Дискваліфікує підтверджену заявку переведенням у стан <see cref="RegistrationStatus.Disqualified"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Заявка не перебуває у стані Confirmed.</exception>
    public void Disqualify()
    {
        if (Status != RegistrationStatus.Confirmed)
        {
            throw new InvalidOperationException($"Перехід зі стану {Status} у Disqualified заборонено.");
        }

        Status = RegistrationStatus.Disqualified;
    }

    /// <summary>
    /// Обчислює базову суму всіх позицій заявки без урахування знижок політики.
    /// </summary>
    /// <returns>Сума вартості позицій у гривнях.</returns>
    public decimal Subtotal()
    {
        decimal sum = 0m;
        foreach (RegistrationItem item in _items)
        {
            sum += item.LineTotal;
        }

        return sum;
    }
}
