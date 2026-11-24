# Картка класової моделі проєкту SportEvents (Варіант № 20)

## 1. Сутність та інваріанти

| Сутність | Роль | Незмінний | Інваріант |
| :--- | :--- | :--- | :--- |
| `RegistrationItem` | позиція заявки на змагання | так, `record` | `!string.IsNullOrWhiteSpace(Code)`, `Quantity > 0`, `UnitPrice >= 0m` |
| `Registration` | документ заявки зі станом | ні, агрегат | `Id > 0`, `ParticipantId > 0`; додавання позицій дозволено лише у стані `New` |
| `Participant` | довідник учасників змагань | так, `record` | `Id > 0`, `!string.IsNullOrWhiteSpace(Name)`, `Email.Contains('@')` |
| `SportEvent` | довідник спортивних подій / забігів | ні (`AvailableSlots` змінюється) | `Id > 0`, `!string.IsNullOrWhiteSpace(Title)`, `AvailableSlots >= 0`, `EntryFee >= 0m`, `StartDate < EndDateExclusive` |

## 2. Стани документа `Registration` та дозволені переходи

| Зі стану | До стану | Умова переходу |
| :--- | :--- | :--- |
| `New` | `Confirmed` | `Items.Count > 0` (у заявці є хоча б одна позиція) |
| `New` | `Cancelled` | завжди (учасник або організатор скасовує неоформлену заявку) |
| `Confirmed` | `Finished` | завжди (після фіксації фінішу учасника на дистанції) |
| `Confirmed` | `Disqualified` | завжди (за порушення регламенту змагань під час старту) |