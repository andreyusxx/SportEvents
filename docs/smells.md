# Реєстр дефектів проєктування коду (Code Smells) — SportEvents.Legacy

Аналізований файл: `SportEvents.Legacy/LegacyRegistrationProcessor.cs`.

| № | Дефект | Місце | Прояв | Рефакторинг |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Довгий список параметрів (Long Parameter List) | `Handle`, рядки 29–40 | 11 параметрів у сигнатурі (`docId`, `athlete`, `items`, `state`, `currency`, `sendMail`, `clientMail`, `note`, `urgent`, `manualBonus`, `out grandTotal`) | Introduce Parameter Object |
| 2 | Вкладені умови (Nested Conditionals) | `Handle`, рядки 43–129 | 4 рівні вкладеності `if` усередині `if`; основне тіло методу зсунуте на 24 пробіли вправо | Decompose Conditional, заміна на охоронні умови (Guard Clauses) |
| 3 | Тимчасове поле (Temporary Field) | Клас `LegacyRegistrationProcessor`, рядки 25–27 | 3 приватні поля (`_lastDiscount`, `_lastShipping`, `_lastTotal`), які записуються та читаються виключно під час одного виклику `Handle` | Remove Temporary Field |
| 4 | Магічне число (Magic Number) | `Handle`, рядки 57–83; `Preview`, рядки 145–160 | 7 неіменованих числових літералів (`500m`, `0.15m`, `0.10m`, `0.12m`, `0.05m`, `0.07m`, `1000m`, `150m`, `50m`) | Replace Magic Number with Constant |
| 5 | Дублювання коду (Duplicated Code) | `Handle`, рядки 51–101 та `Preview`, рядки 138–173 | Дубльовано 28 рядків розрахунку суми позицій, трирівневої шкали знижок, доставки та текстового форматування звіту | Extract Method |
| 6 | Довгий метод (Long Method) | `Handle`, рядки 29–130 | 102 рядки коду (80 LOC без дужок), цикломатична складність $CC = 17$, змішування 5 рівнів абстракції в одному тілі | Extract Method |
| 7 | Заздрість до чужих даних (Feature Envy) | `DescribeClient`, рядки 176–185 | Метод читає виключно поля класу `Athlete` (`Name`, `Kind`, `DoneCount`, `Email`, `SinceUtc`) і жодного разу не звертається до полів `LegacyRegistrationProcessor` | Move Method (перенести метод у клас `Athlete`) |
| 8 | Великий клас (Large Class) | `LegacyRegistrationProcessor`, рядки 22–192 | 171 рядок у межах класу, 6 різних відповідальностей: валідація, арифметика та знижки, розрахунок доставки, форматування звіту, журналювання, опис учасника | Extract Class |
| 9 | Одержимість елементарними типами та мертвий параметр | `Handle`, рядки 32, 36, 47 (`state`, `note`, `currency`); `Athlete.Kind`, рядок 11 | Стан заявки `state` та категорія учасника `Kind` подані рядками (`"new"`, `"vip"`, `"partner"`), а параметр `note` взагалі не використовується в тілі методу | Replace Primitive with Object (`enum`), видалення невикористаного параметра |