# Стандарт кодування та довідник конструктивних практик команди UtilityBills

Версія 1.0.0 · 2026-11-02 · автор: Римарцов Володимир Володимирович, група 471 · статус: чинний

## 1. Вступ і правила користування

### 1.1. Призначення

Цей документ — спільна домовленість команди проєкту **UtilityBills** (облік і нарахування комунальних платежів, варіант № 14) про те, як ми пишемо код. Його читає кожен, хто пише код і хто його рецензує, — і до першого коміту, і під час кожного огляду. Кожне правило має рівень, обґрунтування і спосіб перевірки: правило, яке нічим не перевіряється, — лише побажання.

### 1.2. Сфера дії

Стандарт діє на код проєктів `UtilityBills.Core`, `UtilityBills.App` і `UtilityBills.Tests`. Він **не діє** на теку `Legacy/` (навмисно поганий код із прикладів, підлягає видаленню) та на згенерований код у `bin/` і `obj/`. Наявний код, який стандарту не відповідає, переводять за правилом бойскаута: кожен, хто змінює файл, приводить його до стандарту; відомі відхилення записано в розділі 7.5.

### 1.3. Мови

Текст стандарту, коментарі в коді та повідомлення комітів — українською. Ідентифікатори (імена типів, методів, змінних) — англійською. Англійськими лишаються лише службові формули StyleCop у XML-документації (правило C-09).

### 1.4. Рівні вимог

| Рівень | Що означає | Наслідок порушення |
| --- | --- | --- |
| ОБОВ'ЯЗКОВО | Виконується завжди | збірка падає (`error`) або зміни не приймають на огляді |
| РЕКОМЕНДОВАНО | Виконується, якщо немає вагомої причини | зауваження на огляді; відхилення пояснюють у коментарі |
| ДОПУСКАЄТЬСЯ | Обидва варіанти правильні | нічого; обираєте за смаком |
| ЗАБОРОНЕНО | Конструкція в проєкті не вживається | збірка падає, якщо є аналізатор, або зміни не приймають |

Рівні в `.editorconfig` відповідають рівням тексту (правило A-03): ОБОВ'ЯЗКОВО і ЗАБОРОНЕНО — `error`; РЕКОМЕНДОВАНО — `suggestion`; ДОПУСКАЄТЬСЯ — `silent`.

### 1.5. Глосарій предметної області

Одне поняття — одне слово (правило N-16). Усі імена в коді беруть із цієї таблиці.

| Поняття | Ім'я в коді |
| --- | --- |
| Рахунок (документ нарахування) | `UtilityBill` |
| Рядок рахунку | `UtilityBillLine` |
| Абонент | `Subscriber` |
| Комунальна послуга | `UtilityService` |
| Тариф | `Tariff` |
| Обсяг споживання | `Quantity` |
| Сума рядка | `Amount` |
| Показник лічильника | `MeterReading` |
| Попередній показник | `PreviousReading` |
| Поточний показник | `CurrentReading` |
| Споживання за період | `Consumption` |
| Період нарахування | `BillingPeriod` |
| Крайня дата оплати | `DueDate` |
| Стан рахунку | `BillStatus` |
| Виставити рахунок | `Issue` |
| Оплатити | `Pay` |
| Скасувати | `Cancel` |
| Прострочення | `Overdue` |
| Пільговик | `IsPrivileged` |
| Знижка | `Discount` |
| Політика ціноутворення | `PricingPolicy` |
| Сховище рахунків | `BillRepository` |
| Сума до сплати | `AmountDue` |
| Архівний рахунок | `ArchivedBill` |
| Одиниця виміру | `MeasurementUnit` |

### 1.6. Шаблон запису правила

Кожне правило записано рядком таблиці з полями: **ID** (літера розділу й номер: N — іменування, F — форматування, C — коментарі, D — дані, X — особливості варіанта, S — керування виконанням, A — автоматизація), **Рівень**, **Правило** (одне речення), **Обґрунтування**, **Перевірка**. Приклад до правила, якщо потрібен, стоїть у кінці розділу з номером Е-NN у форматі «Погано / Добре».

| ID | Рівень | Правило | Обґрунтування | Перевірка |
| --- | --- | --- | --- | --- |
| N-03 | ОБОВ'ЯЗКОВО | Інтерфейс — PascalCase з префіксом `I`. | З імені видно контракт. | IDE1006 (error) |


## 2. Іменування

### 2.1. Правила

| ID | Рівень | Правило | Обґрунтування | Перевірка |
| --- | --- | --- | --- | --- |
| N-01 | ОБОВ'ЯЗКОВО | Простір імен починається з `UtilityBills`, має 3–4 сегменти, його шлях збігається зі шляхом теки: `UtilityBills.Core.Billing` ↔ `UtilityBills.Core/Billing`. | Тип знаходять за назвою без пошуку, а глибокі дерева імен не ростуть. | IDE0130 (error); огляд |
| N-02 | ОБОВ'ЯЗКОВО | Клас, запис і структура — PascalCase, іменник або іменна група: `UtilityBill`, `BillCalculator`. | Тип називає річ, а не дію; це відрізняє його від методів. | IDE1006, правило types (error) |
| N-03 | ОБОВ'ЯЗКОВО | Інтерфейс — PascalCase з префіксом `I`: `IBillRepository`, `IPricingPolicy`. | З імені одразу видно, що це контракт, а не реалізація. | IDE1006, правило interfaces (error) |
| N-04 | ОБОВ'ЯЗКОВО | Приватне й внутрішнє поле, зокрема статичне, — `_camelCase`; префікс `s_` не вживаємо. | Поле відрізняється від параметра без `this.`; один префікс простіший за три (див. відхилення 1 у 7.3). | IDE1006, правило private_fields (error); SA1309 вимкнено, бо вимагає протилежного |
| N-05 | ОБОВ'ЯЗКОВО | Константа (поле `const`) — PascalCase: `MaxServiceId`, а не `MAX_ID`. | Константа — звичайне значення з назвою, а не макрос; крикливий регістр не потрібен. | IDE1006, правило constants (error) |
| N-06 | ОБОВ'ЯЗКОВО | Метод, властивість і подія — PascalCase. | Єдиний регістр для публічного API, як у бібліотеці класів .NET. | IDE1006, правило members (error); у тестах правило вимкнено (N-14) |
| N-07 | РЕКОМЕНДОВАНО | Метод-команда починається з дієслова (`Place`, `Cancel`), метод-запит називає результат (`TotalOf`, `RateFor`, `IsOverdue`). | Читач із назви розуміє, чи метод змінює стан. | огляд (чекліст п. 3) |
| N-08 | ОБОВ'ЯЗКОВО | Асинхронний метод має суфікс `Async`: `SaveAsync`. | Викликач бачить, що метод треба чекати (`await`). | IDE1006, правило async_methods (error) |
| N-09 | РЕКОМЕНДОВАНО | Булеві члени мають форму питання: `Is…`, `Has…`, `Can…` — `IsPrivileged`, `HasValidEmail`. | Умова `if (bill.IsOverdue)` читається як речення. | огляд (чекліст п. 3) |
| N-10 | РЕКОМЕНДОВАНО | Параметр і локальна змінна — camelCase з осмисленим іменем: `billId`, `subscriber`. | Назва пояснює зміст, а не тип. | IDE1006, правило locals_and_params (suggestion) |
| N-11 | ОБОВ'ЯЗКОВО | Перелічення — іменник в однині (`BillStatus`), члени без префікса типу (`Paid`, а не `BillStatusPaid`); набір прапорців (`[Flags]`) — множина. | Запис `BillStatus.Paid` не повторює слова. | IDE1006 (error); огляд |
| N-12 | ОБОВ'ЯЗКОВО | Параметр типу починається з `T`: `T`, `TResult`, `TKey`. | Параметр типу відрізняється від звичайного типу. | IDE1006, правило type_parameters (error) |
| N-13 | РЕКОМЕНДОВАНО | Колекція має ім'я у множині: `Lines`, `Bills`; слова `List`, `Array` в імені не вживаємо. | Тип колекції може змінитись, а зміст — ні. | огляд (чекліст п. 3) |
| N-14 | РЕКОМЕНДОВАНО | Тестовий метод — `Метод_Умова_Результат` (`ChargeFor_NegativeAmount_ThrowsOutOfRange`), клас тестів — `ІмяТипуTests`. | Назва тесту, що впав, одразу описує, що зламалося. | огляд (чекліст п. 14) |
| N-15 | ОБОВ'ЯЗКОВО | Один файл — один тип, ім'я файла збігається з іменем типу: `UtilityBill.cs`. | Файл знаходять за назвою типу; файли не розростаються. | SA1402, SA1649 (error) |
| N-16 | РЕКОМЕНДОВАНО | Одне поняття — одне слово: імена беруть із глосарія розділу 1 (`Subscriber`, а не `Client`, `Customer`, `User`). | Розбіжні назви однієї речі збивають читача й ускладнюють пошук. | огляд (чекліст п. 2) |
| N-17 | ДОПУСКАЄТЬСЯ | Загальноприйняті скорочення допускаються: `Id`, `Url`, `Csv`, `Xml`; скорочення з трьох літер і більше пишуть як слово — `Csv`, а не `CSV`. | Короткі стандартні скорочення читаються краще за повні слова; крикливий регістр псує PascalCase. | огляд |

### 2.2. Заборонені антипатерни

| ID | Рівень | Заборонено | Чому | Погано | Добре |
| --- | --- | --- | --- | --- | --- |
| N-18 | ЗАБОРОНЕНО | Імена-затички: `data`, `info`, `temp`, `obj`, `item1`. | Не кажуть, що саме зберігає змінна. | `var data = Load();` | `var bill = Load(billId);` |
| N-19 | ЗАБОРОНЕНО | Транслітерація: `Rakhunok`, `Kilkist`, `Pokaznyk`. | Змішує мови, і пошук за глосарієм не працює. | `class Rakhunok { decimal Kilkist; }` | `class UtilityBill { decimal Quantity; }` |
| N-20 | ЗАБОРОНЕНО | Угорська нотація: `strName`, `decTotal`, `iCount`. | Тип видно зі змінної та підказки середовища. | `decimal decTotal = 0m;` | `decimal total = 0m;` |
| N-21 | ЗАБОРОНЕНО | Нестандартні скорочення: `calcBillTot`, `UtlSvc`, `cnt`. | Скорочення читає лише автор. | `var tot = CalcBillTot(l);` | `var total = CalculateBillTotal(lines);` |
| N-22 | ЗАБОРОНЕНО | Імена з `And` чи `Or`: `SaveAndNotify`. | Метод, що робить дві речі, треба розділити. | `void SaveAndNotify(UtilityBill b)` | `void Place(UtilityBill bill)  // складає кроки` |
| N-23 | ЗАБОРОНЕНО | Беззмістовні суфікси `Manager`, `Helper`, `Utils`, `Common`: `BillManager`. | Клас-смітник збирає все й не має єдиної відповідальності. Правило не діє на теку Legacy/. | `class BillHelper { ... }` | `class BillCalculator { ... }` |
| N-24 | ЗАБОРОНЕНО | Числові й оціночні суфікси: `CalculateTotal2`, `BillFinal`, `BillNew`. | Версію зберігає Git, а не ім'я. | `decimal CalculateTotal2(...)` | `decimal TotalOf(...)` |
| N-25 | ЗАБОРОНЕНО | Ім'я, що вводить в оману: `GetTotal`, який ще й зберігає; `IsPaid`, що змінює стан. | Побічний ефект, не названий в імені, — джерело помилок. | `decimal GetTotal() { Save(); ... }` | `decimal TotalOf(bill)  // лише запит` |

### 2.3. Приклади

**Е-01** (правило N-02, N-18) — Тип і метод із говірними іменами

Погано:

```csharp
class Calc {
  decimal Do(List<UtilityBillLine> l,
    bool f) { ... }
}
```

Добре:

```csharp
class BillCalculator {
  decimal ChargeFor(decimal amount,
    bool isPrivileged) { ... }
}
```

Імена пояснюють зміст без коментарів.

**Е-02** (правило N-09) — Булева ознака

Погано:

```csharp
bool flag = bill.Status == 2;
if (flag) { ... }
```

Добре:

```csharp
bool isPaid = bill.Status == BillStatus.Paid;
if (isPaid) { ... }
```

Назва-питання читається як речення.

**Е-03** (правило N-04) — Приватне поле

Погано:

```csharp
private IBillRepository repo;
private decimal Rate;
```

Добре:

```csharp
private readonly IBillRepository _repository;
private decimal _rate;
```

Префікс `_` відрізняє поле від параметра без `this.`.

**Е-04** (правило N-05) — Константа

Погано:

```csharp
const int MAX_ID = 9999;
```

Добре:

```csharp
public const int MaxServiceId = 9999;
```

**Е-05** (правило N-03) — Інтерфейс

Погано:

```csharp
interface BillRepository { ... }
```

Добре:

```csharp
interface IBillRepository { ... }
```

**Е-06** (правило N-11) — Перелічення

Погано:

```csharp
enum BillStatuses { BS_PAID, BS_NEW }
```

Добре:

```csharp
enum BillStatus { Draft, Issued, Paid }
```

**Е-07** (правило N-01) — Простір імен

Погано:

```csharp
namespace utilitybills.core.Data.Stuff;
```

Добре:

```csharp
namespace UtilityBills.Core.Billing;
```

**Е-08** (правило N-13) — Колекція

Погано:

```csharp
List<UtilityBillLine> LineList;
```

Добре:

```csharp
IReadOnlyList<UtilityBillLine> Lines;
```

**Е-09** (правило N-14) — Назва тесту

Погано:

```csharp
[Fact] void Test1() { ... }
```

Добре:

```csharp
[Fact] void ChargeFor_ZeroAmount_ReturnsZero()
{ ... }
```

**Е-10** (правило N-19) — Транслітерація

Погано:

```csharp
decimal Kilkist; // кількість
var rakhunok = Znayty(id);
```

Добре:

```csharp
decimal Quantity;
var bill = Find(billId);
```

**Е-11** (правило N-22) — Метод із `And`

Погано:

```csharp
void SaveAndNotify(UtilityBill b)
{ _repo.Add(b); _mail.Send(b); }
```

Добре:

```csharp
void Place(UtilityBill bill)
{ _repository.Add(bill);
  _notifier.Notify(...); }
```

Кроки складає сценарій `Place`; кожен має власний клас.

**Е-12** (правило N-25) — Ім'я, що бреше

Погано:

```csharp
decimal GetTotal(UtilityBill b)
{ Save(b); return Sum(b); }
```

Добре:

```csharp
decimal TotalOf(UtilityBill bill)
{ return Sum(bill); }
```

Запит не змінює стан.


## 3. Форматування і коментарі

### 3.1. Форматування

| ID | Рівень | Правило | Обґрунтування | Перевірка |
| --- | --- | --- | --- | --- |
| F-01 | ОБОВ'ЯЗКОВО | Кодування файлів C# — UTF-8 без BOM. | Українські коментарі не псуються на іншому комп'ютері. | charset = utf-8; dotnet format |
| F-02 | ОБОВ'ЯЗКОВО | Кінці рядків C# — CRLF на всіх машинах; закріплено файлом `.gitattributes`. | Однакові кінці рядків дають чисту різницю в Git і зелений `dotnet format` на Linux-ранері. | end_of_line = crlf; .gitattributes; dotnet format |
| F-03 | ОБОВ'ЯЗКОВО | Файл закінчується одним порожнім рядком. | Без нього Git показує зайвий рядок у різниці. | insert_final_newline = true; dotnet format |
| F-04 | ОБОВ'ЯЗКОВО | Пробілів наприкінці рядка немає. | Невидимі зміни засмічують різницю. | trim_trailing_whitespace; SA1028 (error) |
| F-05 | ОБОВ'ЯЗКОВО | Відступ — чотири пробіли, табуляції заборонені. | Код однаково виглядає в будь-якому редакторі. | indent_style, indent_size; IDE0055 (error) |
| F-06 | ОБОВ'ЯЗКОВО | Довжина рядка не більша за 100 символів. | Два файли поряд на екрані 1920 пікселів без горизонтального прокручування. | max_line_length; команда A-06 |
| F-07 | ОБОВ'ЯЗКОВО | Відкривальна фігурна дужка — на новому рядку (стиль Allman). | Початок і кінець блока на одній вертикалі. | csharp_new_line_before_open_brace; IDE0055; SA1500 (error) |
| F-08 | ОБОВ'ЯЗКОВО | Фігурні дужки обов'язкові навіть для одного оператора. | Додавання другого рядка не ламає логіку (клас помилок «goto fail»). | csharp_prefer_braces (IDE0011); SA1503 (error) |
| F-09 | ОБОВ'ЯЗКОВО | Простір імен оголошується в стилі файла: `namespace UtilityBills.Core.Billing;`. | На один рівень відступу менше в усьому файлі. | csharp_style_namespace_declarations (IDE0161, error) |
| F-10 | ОБОВ'ЯЗКОВО | Директиви `using` — поза простором імен, `System.*` першими, далі за алфавітом. | Передбачуваний порядок і чистий початок файла. | IDE0065; SA1208, SA1210 (error) |
| F-11 | ОБОВ'ЯЗКОВО | Порядок членів: поля, конструктори, властивості, методи; публічні перед приватними; статичні перед екземплярними. | Файл читається згори вниз, як зміст. | SA1201, SA1202, SA1204 (error) |
| F-12 | ОБОВ'ЯЗКОВО | Між членами один порожній рядок; два порожні рядки поспіль заборонені. | Рівномірний ритм файла. | SA1516, SA1507 (error) |
| F-13 | ОБОВ'ЯЗКОВО | Один оператор — один рядок; параметри методу — в одному рядку або кожен в окремому. | Рядок із кількома діями не читається й не відлагоджується. | SA1107, SA1117 (error) |
| F-14 | ЗАБОРОНЕНО | `#region` заборонено. | Регіон ховає надто великий клас замість того, щоб його розділити. | SA1124 (error) |
| F-15 | РЕКОМЕНДОВАНО | Довгий вираз переносять після оператора, продовження зсувають на один відступ. | Оператор на кінці рядка показує, що вираз триває. | огляд (чекліст п. 17) |
| F-16 | РЕКОМЕНДОВАНО | Модифікатор видимості вказують явно, навіть коли він типовий: `private readonly …`. | З коду одразу видно рівень доступу. | IDE0040 (suggestion) |
| F-17 | ДОПУСКАЄТЬСЯ | Тіло методу чи властивості з одного виразу допускається записувати через `=>`; обидва записи правильні. | Обидва стилі однаково читаються, вибір лишається за автором. | csharp_style_expression_bodied_* (silent) |

### 3.2. Коментарі та XML-документація

| ID | Рівень | Правило | Обґрунтування | Перевірка |
| --- | --- | --- | --- | --- |
| C-01 | ЗАБОРОНЕНО | Закоментований код у гілці, що йде на злиття. | Старий код зберігає Git; мертві рядки лише плутають. | огляд (чекліст п. 9) |
| C-02 | ОБОВ'ЯЗКОВО | Коментар пояснює «чому»: рішення, обмеження, посилання на вимогу; «що» читається з коду. | Причину рішення з коду не відновити. | огляд (чекліст п. 10) |
| C-03 | ЗАБОРОНЕНО | Коментар, що переказує код або виправдовує погане ім'я. | Краще перейменувати, ніж пояснювати. | огляд (чекліст п. 10) |
| C-04 | РЕКОМЕНДОВАНО | Застарілий коментар видаляють у тому самому коміті, де змінено код. | Коментар, що суперечить коду, гірший за відсутність коментаря. | огляд (чекліст п. 10) |
| C-05 | РЕКОМЕНДОВАНО | Позначка завдання має формат `// TODO(UB-12): що зробити` з номером завдання з трекера. | Безіменні TODO не мають власника й не закриваються. | огляд (чекліст п. 11) |
| C-06 | ОБОВ'ЯЗКОВО | Кожен публічний член бібліотеки `UtilityBills.Core` має XML-документацію. | Публічний API без пояснень не придатний до повторного використання. | CS1591 (error); GenerateDocumentationFile |
| C-07 | ОБОВ'ЯЗКОВО | `summary` — одне речення з крапкою, що описує призначення: метод — «Обчислює…», тип — іменна група. | Підказка в редакторі лишається короткою. | SA1629 (error); огляд |
| C-08 | ОБОВ'ЯЗКОВО | `param` для кожного параметра, `returns` для кожного методу не `void`. | Без цього підказка неповна. | SA1611, SA1615 (error); CS1573 |
| C-09 | ОБОВ'ЯЗКОВО | Властивість: «Gets …» або «Gets or sets …»; конструктор: «Initializes a new instance of the <see cref="X"/> class.». | Це формули, яких вимагає StyleCop; вони єдині для всього коду. | SA1623, SA1642 (error) |
| C-10 | РЕКОМЕНДОВАНО | Для публічного методу, що свідомо кидає виняток, пишуть `<exception>`. | Викликач бачить контракт без читання тіла. | огляд (чекліст п. 10) |
| C-11 | РЕКОМЕНДОВАНО | Коментарі українською, ідентифікатори англійською; англійськими лишаються лише формули з C-09. | Одна мова в коментарях і єдиний словник імен. | огляд |
| C-12 | ОБОВ'ЯЗКОВО | Кожне придушення правила супроводжується причиною: коментар у файлі або `Justification` (див. A-05). | Придушення без причини ховає дефект. | огляд (чекліст п. 18) |
| C-13 | ДОПУСКАЄТЬСЯ | У XML-документації допускаються `<remarks>` і `<example>`, коли пояснення не вміщується в `summary`. | Додаткові розділи потрібні нечасто. | огляд |

### 3.3. Приклади

**Е-13** (правило F-08) — Дужки

Погано:

```csharp
if (amount < 0m)
    throw new Exception();
```

Добре:

```csharp
if (amount < 0m)
{
    throw new ArgumentOutOfRangeException(
        nameof(amount));
}
```

**Е-14** (правило C-02) — Коментар «що» і «чому»

Погано:

```csharp
// множимо кількість на тариф
var sum = quantity * tariff;
```

Добре:

```csharp
// Угору до копійки: вимога варіанта 14,
// виконавець не недоодержує через округлення.
return decimal.Ceiling(raw * 100m) / 100m;
```

**Е-15** (правило C-06, C-08) — XML-документація

Погано:

```csharp
public decimal ChargeFor(decimal a, bool p)
{ ... }
```

Добре:

```csharp
/// <summary>Обчислює суму до сплати.</summary>
/// <param name="amount">Сума за тарифом.</param>
/// <param name="isPrivileged">Пільга.</param>
/// <returns>Сума до сплати, грн.</returns>
```

**Е-16** (правило C-01) — Закоментований код

Погано:

```csharp
// var old = Calc(a, b);
// if (old > 0) Save(old);
var total = TotalOf(bill);
```

Добре:

```csharp
var total = TotalOf(bill);
```

Історію зберігає Git.


## 4. Дані, типи й предметна область

### 4.1. Загальні правила

| ID | Рівень | Правило | Обґрунтування | Перевірка |
| --- | --- | --- | --- | --- |
| D-01 | РЕКОМЕНДОВАНО | Змінну оголошують якнайближче до першого використання, одразу з ініціалізацією. | Менше стану в голові читача. | огляд (чекліст п. 5) |
| D-02 | РЕКОМЕНДОВАНО | Змінна має один зміст: не використовують повторно під інше значення. | Повторне використання ховає помилки. | огляд (чекліст п. 5) |
| D-03 | РЕКОМЕНДОВАНО | `var` — лише коли тип видно справа (`new`, явне приведення). | Тип лишається видимим без підказки редактора. | csharp_style_var_* (suggestion) |
| D-04 | РЕКОМЕНДОВАНО | Однобуквені імена — лише `i`, `j` у лічильнику циклу. | Імена пояснюють зміст. | огляд (чекліст п. 3) |
| D-05 | ОБОВ'ЯЗКОВО | Числові й рядкові літерали в логіці замінюють іменованими константами; винятки: `0`, `1`, `-1`, `string.Empty`. | Назва пояснює значення і дозволяє змінити його в одному місці. | огляд (чекліст п. 6) |
| D-06 | РЕКОМЕНДОВАНО | `const` — для значень, відомих під час компіляції (`decimal` теж); `static readonly` — для решти. | Компілятор підставляє `const` у місце використання. | огляд |
| D-07 | РЕКОМЕНДОВАНО | Ставки, які змінюють без перекомпіляції (тарифи), — дані (довідник `UtilityService.Tariff`), а не константи. | Зміна тарифу не має вимагати нової збірки. | огляд |
| D-08 | ОБОВ'ЯЗКОВО | `Nullable` увімкнено в усьому рішенні; попередження CS86xx зупиняють збірку. | Помилки з `null` знаходить компілятор. | Directory.Build.props: Nullable, TreatWarningsAsErrors |
| D-09 | ОБОВ'ЯЗКОВО | Публічні колекції — `IReadOnlyList<T>` чи `IReadOnlyCollection<T>`, а не `List<T>`. | Ззовні не можна додати рядок у виставлений рахунок. | огляд (чекліст п. 8) |
| D-10 | РЕКОМЕНДОВАНО | `List<T>` — коли важливий порядок; `Dictionary<K,V>` — пошук за ключем; `HashSet<T>` — унікальність. | Правильна колекція дає потрібну швидкодію й зміст. | огляд |
| D-11 | ОБОВ'ЯЗКОВО | Стани й види — перелічення (`BillStatus`), а не рядки й числа. | Недопустимого стану компілятор не пропустить. | огляд (чекліст п. 7) |
| D-12 | ОБОВ'ЯЗКОВО | Гроші, тарифи й обсяги — `decimal`; `double` і `float` для них заборонені. | `double` дає помилки двійкового дробу: 0.1 + 0.2 ≠ 0.3. | огляд (чекліст п. 12); тести |
| D-13 | ОБОВ'ЯЗКОВО | Дробові літерали грошових величин мають суфікс `m`: `0.05m`. | Без суфікса літерал — `double`, і компілятор зупинить збірку. | компілятор (CS0664) |
| D-14 | ОБОВ'ЯЗКОВО | Суми округлюють до 2 знаків за `MidpointRounding.AwayFromZero`, один раз — на підсумку документа, а не на проміжних значеннях. | Банківське округлення за замовчуванням дивує користувача; проміжні округлення накопичують похибку (ADR-002). | огляд (чекліст п. 12); тести |
| D-15 | РЕКОМЕНДОВАНО | Дата без часу — `DateOnly`; момент — `DateTimeOffset`; тривалість — `TimeSpan`; `DateTime` — лише на межі з бібліотеками. | Тип каже, чи є в значенні час і зсув. | огляд (чекліст п. 13) |
| D-16 | ОБОВ'ЯЗКОВО | `DateTime.Now`, `UtcNow` у `Domain`, `Billing`, `Services` не вживають: поточну дату передають параметром (`MarkOverdue(DateOnly today)`). | Метод, що читає годинник, не перевірити тестом. | пошук (команда A-06); огляд (чекліст п. 13) |
| D-17 | РЕКОМЕНДОВАНО | Поле, що не змінюється після конструктора, оголошують `readonly`. | Компілятор забороняє випадкову зміну стану. | IDE0044 / dotnet_style_readonly_field (suggestion) |
| D-18 | ДОПУСКАЄТЬСЯ | Кортеж `(decimal, decimal)` допускається як результат приватного методу; у публічному API — іменований тип. | Кортеж зручний усередині класу, але безіменний у публічному контракті. | огляд |
| D-19 | ДОПУСКАЄТЬСЯ | Однолітерний параметр лямбди допускається, якщо вираз вміщується в один рядок: `l => l.Amount`. | Коротка лямбда читається краще з коротким параметром. | огляд |

### 4.2. Особливості варіанта 14: тарифи, показники лічильників, період нарахування

Правила X — власні для предметної області комунальних платежів; їх у загальних стандартах немає.

| ID | Рівень | Правило | Обґрунтування | Перевірка |
| --- | --- | --- | --- | --- |
| X-01 | ОБОВ'ЯЗКОВО | Тариф — `decimal`, у гривнях за одиницю виміру; ім'я — `Tariff`; коли поряд кілька тарифів, ім'я містить одиницю: `TariffPerKilowattHour`. | Тариф без одиниці виміру не має змісту й не порівнюється. | огляд (чекліст п. 12) |
| X-02 | ОБОВ'ЯЗКОВО | Показник лічильника — `decimal` ≥ 0; пара `PreviousReading` / `CurrentReading`; `Consumption` обчислюють як різницю, але не зберігають; зменшення показника — `DomainRuleException` з правилом `meter.rollback`. | Збережене споживання розходиться з показниками; зменшення показника — помилка вводу, а не від'ємне споживання. | огляд; тест (коли з'явиться клас) |
| X-03 | ОБОВ'ЯЗКОВО | Період нарахування — календарний місяць, зберігається як `yyyy-MM` (`BillingPeriod`) чи пара `PeriodStart` / `PeriodEndExclusive`; кінець інтервалу виключний. | Напіввідкритий інтервал не дублює межу сусіднього періоду. | огляд (чекліст п. 13) |
| X-04 | ОБОВ'ЯЗКОВО | Нарахування за тарифом округлюють до 0,01 грн **угору**: `decimal.Ceiling(x * 100m) / 100m`, один раз на підсумку послуги за період; усі інші суми — за D-14. | Це вимога предметної області варіанта 14; виконавець не недоодержує через округлення. | тести `BillCalculatorTests` |
| X-05 | РЕКОМЕНДОВАНО | Одиниця виміру — перелічення `MeasurementUnit` (`CubicMeter`, `KilowattHour`, `Gigacalorie`); у звіті виводиться разом із числом. | Значення без одиниці виміру легко переплутати. | огляд |

### 4.3. Приклади

**Е-17** (правило D-12) — Тип для грошей

Погано:

```csharp
double tariff = 25.5;
double sum = volume * tariff;
```

Добре:

```csharp
decimal tariff = 25.5m;
decimal sum = volume * tariff;
```

`double` накопичує похибку двійкового дробу.

**Е-18** (правило D-05) — Магічне число

Погано:

```csharp
if (amount >= 1000m) rate += 0.10m;
```

Добре:

```csharp
if (amount >= BigBillFrom)
{
    rate += BigBillRate;
}
```

**Е-19** (правило D-16) — Годинник у логіці

Погано:

```csharp
bool IsOverdue() =>
    DateTime.Now > DueDate;
```

Добре:

```csharp
void MarkOverdue(DateOnly today)
{ if (today <= DueDate) throw ...; }
```

Дату передають параметром — метод детермінований.

**Е-20** (правило D-11) — Стан рядком і числом

Погано:

```csharp
if (bill.Status == 2) { ... }
if (state == "paid") { ... }
```

Добре:

```csharp
if (bill.Status == BillStatus.Paid)
{ ... }
```

**Е-21** (правило D-09) — Публічна колекція

Погано:

```csharp
public List<UtilityBillLine> Lines
    { get; set; }
```

Добре:

```csharp
public IReadOnlyList<UtilityBillLine> Lines
    => _lines;
```

**Е-22** (правило D-14) — Округлення

Погано:

```csharp
decimal.Round(x, 2)  // за замовчуванням
                      // ToEven
```

Добре:

```csharp
decimal.Round(x, 2,
    MidpointRounding.AwayFromZero)
```

Рівно 2,345 має стати 2,35, а не 2,34.

**Е-23** (правило X-02) — Показники лічильника

Погано:

```csharp
double start, end;
double used = end - start; // зберігаємо
```

Добре:

```csharp
decimal PreviousReading, CurrentReading;
decimal Consumption =>
    CurrentReading - PreviousReading;
```

Споживання обчислюється, не зберігається.

**Е-24** (правило X-03) — Період нарахування

Погано:

```csharp
DateTime from, to;  // to — включно?
```

Добре:

```csharp
DateOnly PeriodStart;
DateOnly PeriodEndExclusive;
```

Кінець виключний: наступний період починається там, де закінчився попередній.


## 5. Керування виконанням і складність

### 5.1. Правила

| ID | Рівень | Правило | Обґрунтування | Перевірка |
| --- | --- | --- | --- | --- |
| S-01 | РЕКОМЕНДОВАНО | Замість вкладених умов — охоронні умови й ранній вихід. | Основний шлях виконання лишається без вкладення. | огляд (чекліст п. 15) |
| S-02 | ОБОВ'ЯЗКОВО | Глибина вкладеності керівних конструкцій: попередження 3, зупинка 4. | Кожен рівень додає стан, який треба тримати в голові (розд. 5.2). | огляд (чекліст п. 15) |
| S-03 | ОБОВ'ЯЗКОВО | Цикломатична складність методу: попередження 8, зупинка 12. | Кожен шлях потребує окремого тесту (розд. 5.2). | огляд (чекліст п. 15); вимір вручну |
| S-04 | РЕКОМЕНДОВАНО | Параметрів методу: попередження 4, зупинка 6; більше — об'єкт-параметр. | Багато параметрів легко переплутати місцями (ЛР 6: 11 → 1). | огляд (чекліст п. 15) |
| S-05 | РЕКОМЕНДОВАНО | Рядків у методі: 30 / 50; публічних членів у класі: 12 / 20; рядків у файлі: 300 / 500. | Метод і клас, що вміщуються на екран, можна оцінити одразу. | огляд (чекліст п. 15) |
| S-06 | ОБОВ'ЯЗКОВО | Аргументи публічного методу перевіряють на початку (`ArgumentNullException.ThrowIfNull`, `ThrowIfNullOrWhiteSpace`); помилку не позначають значенням `-1` чи `null`. | Виняток називає причину, а `-1` змішується з сумою. | огляд (чекліст п. 16) |
| S-07 | ОБОВ'ЯЗКОВО | `switch`-вираз має бути вичерпним: усі члени перелічення оброблено, гілка `_` кидає виняток або покрита тестом. | Новий стан без обробки дає помилку компіляції. | CS8509 (error) |
| S-08 | ЗАБОРОНЕНО | Оператор `goto` заборонено. | Ламає лінійне читання коду. | огляд |
| S-09 | РЕКОМЕНДОВАНО | `break` і `continue` — лише для раннього виходу; не більше одного на цикл. | Кілька виходів із циклу ускладнюють міркування. | огляд |
| S-10 | РЕКОМЕНДОВАНО | Складну умову з більш ніж двома операндами виносять у змінну з іменем-питанням: `bool canBeCancelled = …`. | Назва пояснює зміст умови. | огляд (чекліст п. 15) |
| S-11 | ОБОВ'ЯЗКОВО | Булеве значення не порівнюють із `true` чи `false`. | Зайве порівняння — шум. | IDE0100 (error) |
| S-12 | РЕКОМЕНДОВАНО | Метод або запит (повертає значення, не змінює стан), або команда (змінює стан, нічого не повертає); виняток — шаблон `TryParse`. | Виклик запиту не має побічних ефектів (CQS). | огляд (чекліст п. 16) |
| S-13 | ОБОВ'ЯЗКОВО | `catch (Exception)` не вживають, порожній `catch` заборонено; єдиний виняток — верхній обробник у `Program.cs` з коментарем. | Широке перехоплення ховає помилки програміста. | CA1031 (error) |
| S-14 | РЕКОМЕНДОВАНО | Колекцію не змінюють під час перебору `foreach`. | Така зміна кидає `InvalidOperationException` під час виконання. | огляд (чекліст п. 15) |
| S-15 | РЕКОМЕНДОВАНО | Вибір за перелічення з трьох і більше гілок записують `switch`-виразом, а не ланцюгом `if–else if`. | Усі гілки видно одночасно, а пропущену — знаходить компілятор. | IDE0066 / csharp_style_prefer_switch_expression (suggestion) |
| S-16 | ДОПУСКАЄТЬСЯ | Тернарний оператор допускається для простого вибору значення; вкладені тернарні оператори заборонені. | Один `?:` короткий, а два — вже ребус. | огляд (чекліст п. 15) |

### 5.2. Числові межі

Межі встановлено за вимірами власного коду проєкту, а не запозичено: після ЛР 6 найбільша цикломатична складність — 5, метод до рефакторингу мав 17 і 80 рядків.

| Показник | Попередження | Зупинка | Обґрунтування | Як вимірюють |
| --- | --- | --- | --- | --- |
| Цикломатична складність методу | 8 | 12 | Після ЛР 6 найбільша CC у проєкті — 5; метод розбору вводу `UtilityBillLineParser.TryParse` має 9, тому поріг 8 на ньому «попереджає»: на огляді це допустимо, якщо кожна гілка відповідає рядку таблиці тестових даних. | вручну: 1 + кількість `if`, `for`, `foreach`, `case`, `&&`, `\|\|`, `??`, `?:` |
| Глибина вкладеності керівних конструкцій | 3 | 4 | До рефакторингу ЛР 6 метод мав 4 рівні й був непридатний до читання; після — 1; поріг 3 залишає запас для `foreach` з охоронною умовою. | вручну |
| Довжина методу, рядків | 30 | 50 | Найдовший метод після ЛР 6 — 16 рядків; 30 — приблизно половина екрана на 1080p. | вручну (рядки без порожніх) |
| Параметрів методу | 4 | 6 | Було 11 (ЛР 6), після рефакторингу максимум 4 (`ReportBuilder.Build`); більше — об'єкт-параметр. | огляд |
| Публічних членів класу | 12 | 20 | Найбільший клас `BillService` має 7 публічних членів; більше 12 — ознака кількох відповідальностей. | огляд |
| Рядків у файлі | 300 | 500 | Найбільший файл проєкту (`BillService.cs`) ≈ 230 рядків із XML-документацією; 300 залишає запас. | огляд |
| Довжина рядка, символів | 100 | 100 | Жорсткий ліміт F-06. | команда A-06 |

Кого на практиці «зупиняє» поріг: перевищення **зупинної** межі блокує злиття, перевищення **попереджувальної** потребує пояснення в описі pull request.

### 5.3. Приклади

**Е-25** (правило S-01) — Вкладені умови й охоронні умови

Погано:

```csharp
if (bill != null) {
  if (bill.Lines.Count > 0) {
    if (bill.Status == Draft) {
      Issue(bill); } } }
```

Добре:

```csharp
if (bill is null) return;
if (bill.Lines.Count == 0) return;
if (bill.Status != Draft) return;
Issue(bill);
```

**Е-26** (правило S-07) — Ланцюг `if` і `switch`

Погано:

```csharp
if (s == Draft) return Issued;
else if (s == Issued) return Paid;
else return s;
```

Добре:

```csharp
return s switch
{ Draft => Issued, Issued => Paid,
  _ => s };
```

**Е-27** (правило S-10) — Складна умова

Погано:

```csharp
if (b.Status == Draft || b.Status
  == Issued && b.Lines.Count > 0) ...
```

Добре:

```csharp
bool canBeCancelled =
    b.Status is Draft or Issued;
if (canBeCancelled) ...
```


## 6. Автоматизація: .editorconfig і Directory.Build.props

### 6.1. Файли

Машинна частина стандарту лежить у трьох файлах, а їх зміст описано нижче: `.editorconfig` (корінь репозиторію), `Directory.Build.props` (корінь) і `UtilityBills.Tests/.editorconfig` (виняток для тестів).

### 6.2. Файл .editorconfig

```ini
root = true

[*.cs]
charset = utf-8
end_of_line = crlf
insert_final_newline = true
trim_trailing_whitespace = true
indent_style = space
indent_size = 4
max_line_length = 100

# Фігурні дужки, простір імен, using (F-07 … F-10)
csharp_new_line_before_open_brace = all
csharp_prefer_braces = true:error
csharp_style_namespace_declarations = file_scoped:error
csharp_using_directive_placement = outside_namespace:error
dotnet_sort_system_directives_first = true
dotnet_separate_import_directive_groups = false

# this. заборонено (N-04)
dotnet_style_qualification_for_field = false:error
dotnet_style_qualification_for_property = false:error
dotnet_style_qualification_for_method = false:error
dotnet_style_qualification_for_event = false:error

# Типи й вирази (D-03, D-17, F-16, F-17, S-15)
csharp_style_var_for_built_in_types = false:suggestion
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_style_var_elsewhere = false:suggestion
csharp_style_prefer_switch_expression = true:suggestion
dotnet_style_readonly_field = true:suggestion
dotnet_style_require_accessibility_modifiers = for_non_interface_members:suggestion
csharp_style_expression_bodied_methods = false:silent
csharp_style_expression_bodied_properties = true:silent

# Іменування (N-02 … N-12)
dotnet_naming_style.pascal_case_style.capitalization = pascal_case
dotnet_naming_style.camel_case_style.capitalization = camel_case
dotnet_naming_style.underscore_camel_style.capitalization = camel_case
dotnet_naming_style.underscore_camel_style.required_prefix = _
dotnet_naming_style.interface_style.capitalization = pascal_case
dotnet_naming_style.interface_style.required_prefix = I
dotnet_naming_style.type_parameter_style.capitalization = pascal_case
dotnet_naming_style.type_parameter_style.required_prefix = T
dotnet_naming_style.async_style.capitalization = pascal_case
dotnet_naming_style.async_style.required_suffix = Async

dotnet_naming_symbols.interfaces.applicable_kinds = interface
dotnet_naming_symbols.type_parameters.applicable_kinds = type_parameter
dotnet_naming_symbols.async_methods.applicable_kinds = method
dotnet_naming_symbols.async_methods.required_modifiers = async
dotnet_naming_symbols.constants.applicable_kinds = field
dotnet_naming_symbols.constants.required_modifiers = const
dotnet_naming_symbols.private_fields.applicable_kinds = field
dotnet_naming_symbols.private_fields.applicable_accessibilities = private, internal
dotnet_naming_symbols.types.applicable_kinds = class, struct, enum, delegate
dotnet_naming_symbols.members.applicable_kinds = method, property, event
dotnet_naming_symbols.locals_and_params.applicable_kinds = parameter, local

dotnet_naming_rule.interfaces_start_with_i.symbols = interfaces
dotnet_naming_rule.interfaces_start_with_i.style = interface_style
dotnet_naming_rule.interfaces_start_with_i.severity = error
dotnet_naming_rule.type_parameters_start_with_t.symbols = type_parameters
dotnet_naming_rule.type_parameters_start_with_t.style = type_parameter_style
dotnet_naming_rule.type_parameters_start_with_t.severity = error
dotnet_naming_rule.async_methods_end_with_async.symbols = async_methods
dotnet_naming_rule.async_methods_end_with_async.style = async_style
dotnet_naming_rule.async_methods_end_with_async.severity = error
dotnet_naming_rule.constants_should_be_pascal_case.symbols = constants
dotnet_naming_rule.constants_should_be_pascal_case.style = pascal_case_style
dotnet_naming_rule.constants_should_be_pascal_case.severity = error
dotnet_naming_rule.private_fields_should_have_underscore.symbols = private_fields
dotnet_naming_rule.private_fields_should_have_underscore.style = underscore_camel_style
dotnet_naming_rule.private_fields_should_have_underscore.severity = error
dotnet_naming_rule.types_should_be_pascal_case.symbols = types
dotnet_naming_rule.types_should_be_pascal_case.style = pascal_case_style
dotnet_naming_rule.types_should_be_pascal_case.severity = error
dotnet_naming_rule.members_should_be_pascal_case.symbols = members
dotnet_naming_rule.members_should_be_pascal_case.style = pascal_case_style
dotnet_naming_rule.members_should_be_pascal_case.severity = error
dotnet_naming_rule.locals_and_params_should_be_camel_case.symbols = locals_and_params
dotnet_naming_rule.locals_and_params_should_be_camel_case.style = camel_case_style
dotnet_naming_rule.locals_and_params_should_be_camel_case.severity = suggestion

# Суворі правила (рівень error)
dotnet_diagnostic.IDE0055.severity = error
dotnet_diagnostic.IDE0130.severity = error
dotnet_diagnostic.IDE0100.severity = error
dotnet_diagnostic.CS1591.severity = error
dotnet_diagnostic.CA1305.severity = error
dotnet_diagnostic.CA1031.severity = error
dotnet_diagnostic.SA1028.severity = error
dotnet_diagnostic.SA1107.severity = error
dotnet_diagnostic.SA1117.severity = error
dotnet_diagnostic.SA1124.severity = error
dotnet_diagnostic.SA1201.severity = error
dotnet_diagnostic.SA1202.severity = error
dotnet_diagnostic.SA1204.severity = error
dotnet_diagnostic.SA1402.severity = error
dotnet_diagnostic.SA1208.severity = error
dotnet_diagnostic.SA1210.severity = error
dotnet_diagnostic.SA1500.severity = error
dotnet_diagnostic.SA1503.severity = error
dotnet_diagnostic.SA1507.severity = error
dotnet_diagnostic.SA1516.severity = error
dotnet_diagnostic.SA1611.severity = error
dotnet_diagnostic.SA1615.severity = error
dotnet_diagnostic.SA1623.severity = error
dotnet_diagnostic.SA1629.severity = error
dotnet_diagnostic.SA1642.severity = error
dotnet_diagnostic.SA1649.severity = error

# Вимкнені правила (обґрунтування — розділ 6.4)
dotnet_diagnostic.SA0001.severity = none
dotnet_diagnostic.SA1101.severity = none
dotnet_diagnostic.SA1200.severity = none
dotnet_diagnostic.SA1309.severity = none
dotnet_diagnostic.SA1413.severity = none
dotnet_diagnostic.SA1600.severity = none
dotnet_diagnostic.SA1601.severity = none
dotnet_diagnostic.SA1602.severity = none
dotnet_diagnostic.SA1633.severity = none
dotnet_diagnostic.SA1648.severity = none
dotnet_diagnostic.CA1707.severity = none
dotnet_diagnostic.CA1848.severity = none
```

Файл `UtilityBills.Tests/.editorconfig`:

```ini
[*.cs]
# Імена тестів у форматі Метод_Умова_Результат містять підкреслення (N-14).
dotnet_diagnostic.CA1707.severity = none
dotnet_naming_rule.members_should_be_pascal_case.severity = none
```

### 6.3. Пояснення кожного рядка

| Ключ | Значення | Що робить | Правило | Рівень правила |
| --- | --- | --- | --- | --- |
| `root` | `true` | Файл — кореневий, батьківські не шукаються | A-01 | ОБОВ'ЯЗКОВО |
| `charset` | `utf-8` | Кодування файлів C# | F-01 | ОБОВ'ЯЗКОВО |
| `end_of_line` | `crlf` | Кінці рядків | F-02 | ОБОВ'ЯЗКОВО |
| `insert_final_newline` | `true` | Порожній рядок наприкінці файла | F-03 | ОБОВ'ЯЗКОВО |
| `trim_trailing_whitespace` | `true` | Прибирає пробіли наприкінці рядка | F-04 | ОБОВ'ЯЗКОВО |
| `indent_style / indent_size` | `space / 4` | Відступ чотири пробіли | F-05 | ОБОВ'ЯЗКОВО |
| `max_line_length` | `100` | Довжина рядка | F-06 | ОБОВ'ЯЗКОВО |
| `csharp_new_line_before_open_brace` | `all` | Стиль Allman | F-07 | ОБОВ'ЯЗКОВО |
| `csharp_prefer_braces` | `true:error` | Фігурні дужки обов'язкові | F-08 | ОБОВ'ЯЗКОВО |
| `csharp_style_namespace_declarations` | `file_scoped:error` | Простір імен у стилі файла | F-09 | ОБОВ'ЯЗКОВО |
| `csharp_using_directive_placement` | `outside_namespace:error` | using поза простором імен | F-10 | ОБОВ'ЯЗКОВО |
| `dotnet_sort_system_directives_first` | `true` | System.* першими | F-10 | ОБОВ'ЯЗКОВО |
| `dotnet_style_qualification_for_* (4 ключі)` | `false:error` | Заборона `this.` | N-04 | ОБОВ'ЯЗКОВО |
| `csharp_style_var_* (3 ключі)` | `suggestion` | Політика `var` | D-03 | РЕКОМЕНДОВАНО |
| `csharp_style_prefer_switch_expression` | `true:suggestion` | switch замість ланцюга if | S-15 | РЕКОМЕНДОВАНО |
| `dotnet_style_readonly_field` | `true:suggestion` | readonly для незмінних полів | D-17 | РЕКОМЕНДОВАНО |
| `dotnet_style_require_accessibility_modifiers` | `for_non_interface_members:suggestion` | Явна видимість | F-16 | РЕКОМЕНДОВАНО |
| `csharp_style_expression_bodied_methods / _properties` | `false:silent / true:silent` | Тіло члена: обидва записи допустимі | F-17 | ДОПУСКАЄТЬСЯ |
| `dotnet_naming_rule.interfaces_start_with_i` | `error` | Інтерфейс з префіксом I | N-03 | ОБОВ'ЯЗКОВО |
| `dotnet_naming_rule.type_parameters_start_with_t` | `error` | Параметр типу з префіксом T | N-12 | ОБОВ'ЯЗКОВО |
| `dotnet_naming_rule.async_methods_end_with_async` | `error` | Суфікс Async | N-08 | ОБОВ'ЯЗКОВО |
| `dotnet_naming_rule.constants_should_be_pascal_case` | `error` | Константи — PascalCase | N-05 | ОБОВ'ЯЗКОВО |
| `dotnet_naming_rule.private_fields_should_have_underscore` | `error` | Приватні поля — _camelCase | N-04 | ОБОВ'ЯЗКОВО |
| `dotnet_naming_rule.types_should_be_pascal_case` | `error` | Типи — PascalCase | N-02 | ОБОВ'ЯЗКОВО |
| `dotnet_naming_rule.members_should_be_pascal_case` | `error` | Методи, властивості, події — PascalCase | N-06 | ОБОВ'ЯЗКОВО |
| `dotnet_naming_rule.locals_and_params_should_be_camel_case` | `suggestion` | Параметри й локальні — camelCase | N-10 | РЕКОМЕНДОВАНО |
| `IDE0055` | `error` | Форматування (відступи, пробіли) | F-05 | ОБОВ'ЯЗКОВО |
| `IDE0130` | `error` | Простір імен збігається з текою | N-01 | ОБОВ'ЯЗКОВО |
| `IDE0100` | `error` | Зайве порівняння булевого з true/false | S-11 | ОБОВ'ЯЗКОВО |
| `CS1591` | `error` | Публічний член без XML-документації | C-06 | ОБОВ'ЯЗКОВО |
| `CA1305` | `error` | Не вказано культуру форматування | D-14 | ОБОВ'ЯЗКОВО |
| `CA1031` | `error` | Широкий catch (Exception) | S-13 | ОБОВ'ЯЗКОВО |
| `SA1028` | `error` | Пробіли наприкінці рядка | F-04 | ОБОВ'ЯЗКОВО |
| `SA1107 / SA1117` | `error` | Один оператор — один рядок; параметри | F-13 | ОБОВ'ЯЗКОВО |
| `SA1124` | `error` | Заборона #region | F-14 | ЗАБОРОНЕНО |
| `SA1201 / SA1202 / SA1204` | `error` | Порядок членів | F-11 | ОБОВ'ЯЗКОВО |
| `SA1402 / SA1649` | `error` | Один тип — один файл; ім'я файла = ім'я типу | N-15 | ОБОВ'ЯЗКОВО |
| `SA1208 / SA1210` | `error` | using: System першими, за алфавітом | F-10 | ОБОВ'ЯЗКОВО |
| `SA1500 / SA1503` | `error` | Дужки на окремому рядку; дужки обов'язкові | F-07 | ОБОВ'ЯЗКОВО |
| `SA1507 / SA1516` | `error` | Порожні рядки між членами | F-12 | ОБОВ'ЯЗКОВО |
| `SA1611 / SA1615` | `error` | param для кожного параметра, returns для не-void | C-08 | ОБОВ'ЯЗКОВО |
| `SA1623 / SA1642` | `error` | Формули Gets / Initializes у документації | C-09 | ОБОВ'ЯЗКОВО |
| `SA1629` | `error` | Речення в документації закінчується крапкою | C-07 | ОБОВ'ЯЗКОВО |

Вимкнені правила:

| Ключ | Значення | Чому вимкнено |
| --- | --- | --- |
| `SA0001, SA1600–SA1602` | `none` | XML-стиль і вимога документувати все — документування керує CS1591 (C-06) |
| `SA1101` | `none` | Вимога префікса this. — суперечить N-04 (див. 6.4) |
| `SA1200` | `none` | using усередині простору імен — суперечить F-10 |
| `SA1309` | `none` | Вимога поля без _ — суперечить N-04 (див. 6.4) |
| `SA1413, SA1633, SA1648` | `none` | Кінцева кома, заголовок файла, inheritdoc — не вимога проєкту |
| `CA1707` | `none` | Підкреслення в іменах тестів (N-14) — лише для тестів |
| `CA1848` | `none` | LoggerMessage-делегати — для навчального проєкту зайві |

Перевірка відповідності A-03: кожен рядок із `error` належить правилу ОБОВ'ЯЗКОВО чи ЗАБОРОНЕНО, кожен `suggestion` — РЕКОМЕНДОВАНО, кожен `silent` — ДОПУСКАЄТЬСЯ; розбіжностей немає.

### 6.4. Придушені правила й обґрунтування

**SA1101.** Вимагає префікса `this.` перед кожним полем. Це суперечить правилу N-04: поле вже відрізняється префіксом `_`, а `this.` ми забороняємо (error у `dotnet_style_qualification_for_*`). Вмикати обидва правила одночасно неможливо.

**SA1309.** Вимагає, щоб ім'я поля не починалося з підкреслення. Наш стандарт (N-04) вимагає саме `_camelCase`, як у dotnet/runtime і Microsoft Learn. Дотримання префікса контролює IDE1006, тож SA1309 було б джерелом суперечливих помилок.

### 6.5. Файл Directory.Build.props

```xml
<Project>

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>12.0</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <AnalysisLevel>8.0</AnalysisLevel>
    <AnalysisMode>Recommended</AnalysisMode>
  </PropertyGroup>

  <PropertyGroup Condition="'$(MSBuildProjectName)' == 'UtilityBills.Core'">
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>

</Project>
```

| Властивість | Навіщо |
| --- | --- |
| `TargetFramework` | Одна версія .NET для всіх проєктів. |
| `LangVersion` | Фіксує мову C# 12, щоб новіший SDK не вмикав нових можливостей непомітно. |
| `Nullable` | Помилки з `null` знаходить компілятор (D-08). |
| `ImplicitUsings` | Прибирає типові `using` із кожного файла. |
| `TreatWarningsAsErrors` | Попередження неможливо проігнорувати. |
| `EnableNETAnalyzers` | Вмикає аналізатори .NET (A-04). |
| `EnforceCodeStyleInBuild` | Правила стилю IDE діють під час складання, а не лише в редакторі. |
| `AnalysisLevel` + `AnalysisMode` | Набір правил фіксований на 8.0 у режимі Recommended, однаковий на всіх машинах. |
| `GenerateDocumentationFile` (лише для Core) | Вмикає CS1591 для публічного API бібліотеки (C-06). |

### 6.6. Правила автоматизації

| ID | Рівень | Правило | Обґрунтування | Перевірка |
| --- | --- | --- | --- | --- |
| A-01 | ОБОВ'ЯЗКОВО | Файл `.editorconfig` лежить у корені з `root = true`; зміна рівня правила — лише через pull request із записом у журнал рішень. | Єдина точка правди; зміни можна простежити. | огляд (процедура 7.4) |
| A-02 | ОБОВ'ЯЗКОВО | `Directory.Build.props` задає спільні властивості: Nullable, TreatWarningsAsErrors, EnforceCodeStyleInBuild, AnalysisMode, GenerateDocumentationFile (для Core), LangVersion. | Правила однакові для всіх проєктів і не дублюються в `.csproj`. | складання |
| A-03 | ОБОВ'ЯЗКОВО | Рівні в `.editorconfig`: ОБОВ'ЯЗКОВО і ЗАБОРОНЕНО — `error`; РЕКОМЕНДОВАНО — `suggestion`; ДОПУСКАЄТЬСЯ — `silent`; розбіжність між текстом і файлом — дефект стандарту. | Стандарт і конфігурація не можуть суперечити. | порівняння таблиці 6.3 з файлом |
| A-04 | ОБОВ'ЯЗКОВО | До `UtilityBills.Core` і `UtilityBills.App` підключено `StyleCop.Analyzers` 1.2.0-beta.556; аналізатори .NET увімкнено (`EnableNETAnalyzers`). | Одні й ті самі правила працюють у редакторі, складанні та CI. | складання |
| A-05 | ОБОВ'ЯЗКОВО | Правило придушують лише записом у `.editorconfig` із коментарем або атрибутом `SuppressMessage` із непорожнім `Justification`; директиви `#pragma warning disable` у тілі методу заборонені; на рівні файла дозволені лише в `Legacy/` і `Program.cs` із коментарем. | Точкове придушення без причини перетворюється на глушіння всіх зауважень. | огляд (чекліст п. 18) |
| A-06 | ОБОВ'ЯЗКОВО | Перед комітом локально виконують `dotnet format --verify-no-changes`, `dotnet build -warnaserror` і перевірку довжини рядка (команда нижче). | Зауваження знаходять до того, як їх побачить рецензент. | CI (ЛР 8) повторює ті самі команди |
| A-07 | РЕКОМЕНДОВАНО | Нове правило, що вмикається як `error`, вводять разом з виправленням наявного коду, інакше — спершу як `suggestion`. | Збірка не повинна падати від самого стандарту. | огляд (процедура 7.4) |
| A-08 | РЕКОМЕНДОВАНО | Метрики складності (CA1502) підключають окремим рішенням після ЛР 8; пороги — із розділу 5.2. | Автоматична перевірка CC замінить ручний вимір. | журнал рішень |

Команди перед комітом (A-06), PowerShell:

```powershell
dotnet format --verify-no-changes
dotnet build -warnaserror

# рядки довші за 100 символів (F-06)
Get-ChildItem -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' } |
  ForEach-Object { $path = $_.FullName; $n = 0;
    Get-Content $path | ForEach-Object { $n++; if ($_.Length -gt 100) { "${path}:$n" } } }

# поточний час у логіці (D-16)
Select-String -Path UtilityBills.Core\Domain\*.cs, UtilityBills.Core\Billing\*.cs, UtilityBills.Core\Services\*.cs -Pattern 'DateTime\.(Now|UtcNow)'
```

Приклад до A-05:

**Е-28** (правило A-05) — Придушення правила

Погано:

```csharp
void Pay() {
#pragma warning disable CA1031
  try { ... } catch (Exception) { }
}
```

Добре:

```csharp
[SuppressMessage("Design", "CA1032",
  Justification = "Правило завжди
  створюється з ідентифікатором.")]
class DomainRuleException : Exception
```

Директиву `#pragma` у тілі методу не вживаємо.

### 6.7. Перевірка: навмисні порушення

Три порушення вносять у будь-який клас `UtilityBills.Core`, запускають `dotnet build` і повертають файл (`git checkout -- <файл>`). Текст повідомлень може відрізнятися.

| № | Порушення | Правило | Очікувана діагностика |
| --- | --- | --- | --- |
| 1 | `private decimal total;` | N-04 | `error IDE1006: Naming rule violation: Missing prefix: '_'` |
| 2 | `if (amount < 0m) return;` (без дужок) | F-08 | `error IDE0011: Add braces` |
| 3 | `this._total = 0m;` | N-04 | `error IDE0003: Remove 'this' qualification` |


## 7. Процес: огляд, порівняння, зміна стандарту

### 7.1. Чекліст огляду коду

Повний чекліст — у файлі `docs/REVIEW_CHECKLIST.md`; у ньому лише те, що не може перевірити машина (18 пунктів).

### 7.2. Культура огляду

| ID | Правило | Перевірка |
| --- | --- | --- |
| R-01 | Один огляд — не більше 400 змінених рядків; більшу зміну рецензент вправі повернути з проханням розбити. | огляд |
| R-02 | Зауваження має три частини: що не так — чому — пропозиція; блокувальне зауваження позначають словом «блокує», решта — «пропозиція». | огляд |

### 7.3. Порівняння з публічними стандартами

Порівняно зі сторінками «C# Coding Conventions» і «Identifier names» (Microsoft Learn) та «coding-style.md» репозиторію dotnet/runtime.

| Тема | Microsoft Learn | dotnet/runtime | Наш стандарт | Висновок |
| --- | --- | --- | --- | --- |
| Приватні поля | _camelCase | _camelCase; статичні `s_` | _camelCase, також статичні | Відхилення 1: у проєкті практично немає статичних приватних полів (константи — `const`), тому три префікси (`_`, `s_`, `t_`) — зайва вартість навчання. |
| Константи | PascalCase (поля й локальні) | — | PascalCase для полів; локальні `const` — camelCase | Відхилення 2: локальна константа поводиться як змінна свого блока; один регістр на область видимості простіше. Правило N-05 охоплює лише поля. |
| `this.` | уникати | уникати, крім необхідного | заборонено (error) | Суворіше за джерела, але в тому самому напрямі: префікс `_` усуває потребу в `this.`. |
| `var` | коли тип очевидний | лише коли тип явно названо справа | те саме (suggestion) | Збігається; рівень suggestion, бо надмірно суворе правило дратує без користі. |
| Відступ | 4 пробіли | 4 пробіли, без табуляцій | 4 пробіли, табуляції заборонені | Збігається. |
| Фігурні дужки | Allman, нова лінія | Allman; дужки можна пропускати, коли всі гілки в один рядок | Allman; дужки завжди | Відхилення 3: «завжди» — суворіше. Додавання другого рядка до однорядкового `if` без дужок — класична помилка; вартість зайвих двох рядків менша. |
| Простір імен | стиль файла рекомендовано | — | стиль файла обов'язковий (error) | Суворіше: один стиль у всіх 100 % файлів дешевший за змішаний. |
| Явна видимість | завжди | завжди | рекомендовано (suggestion) | Відхилення 4: лише рекомендація, бо в наявному коді є неявні `private`; перехід за правилом бойскаута. |
| Суфікс `Async` | обов'язковий | — | обов'язковий (error) | Збігається. |
| Довжина рядка | в наведених джерелах числового ліміту не знайдено | — | 100 символів | Відхилення 5: ліміт потрібен для сусідніх вікон у перегляді різниці; обґрунтовано в ADR-001. |
| XML-документація | рекомендована | — | обов'язкова для публічного API Core (error) | Відхилення 6: бібліотека навчальна, її API вивчають за підказками; без документації CS1591 зупиняє збірку. |
| Стиль наявного файла | — | якщо файл відрізняється, діє стиль файла | діє стандарт, наявний код переводять за бойскаутом | Відхилення 7: змішаний стиль у проєкті з одним автором не виправдовується; відхилення записано в 7.3. |
| Регіони | — | — | заборонено (error) | Не вимагається джерелами; регіон ховає надто великий клас. |

### 7.4. Процедура зміни стандарту

1. Ініціатор створює ідею в трекері: заголовок «Стандарт: <правило>», опис проблеми й приклад коду.
2. Автор пропозиції готує зміну в окремій гілці `standard/<короткий-опис>`: текст правила, рівень, обґрунтування, спосіб перевірки та приклад.
3. Якщо правило машинне — у тому ж коміті оновлюється `.editorconfig`; новий `error` вводять разом з виправленням наявного коду (A-07).
4. Відкривається pull request у `main`; у ньому зазначено, які правила змінено і чому.
5. Рецензент перевіряє узгодженість тексту й файла (A-03), вплив на наявний код і чи виконується складання.
6. Після схвалення зміну зливають, збільшують версію стандарту (мінорну — нове правило, мажорну — зміна рівня на суворіший) і додають запис до журналу рішень.
7. Рішення вступає в силу від наступного pull request; наявний код переводять за правилом бойскаута.

### 7.5. Журнал рішень

#### ADR-001. Максимальна довжина рядка: 100 чи 120 символів

- **Дата:** 2026-11-02
- **Контекст:** Команда переглядає зміни у веб-інтерфейсі GitHub у режимі «два файли поряд». Лінія 120 символів виходить за межі однієї колонки, і рецензент змушений прокручувати рядок горизонтально.
- **Варіанти:** (А) 120 символів — менше переносів; (Б) 100 символів — більше переносів; (В) без ліміту.
- **Рішення:** Ухвалено варіант Б — 100 символів (F-06): у наявному коді 100 % рядків уже коротші за 100, тому стандарт нічого не ламає, а перегляд різниці лишається зручним.
- **Наслідки:** Довгі рядки переносять за F-15; перевірку виконує команда A-06 і, пізніше, крок CI. Міняти ліміт тепер можна лише за процедурою 7.4.
- **Ухвалив:** Римарцов В. (автор стандарту)
- **Статус:** прийнято

#### ADR-002. Округлення грошових сум: AwayFromZero чи банківське (ToEven)

- **Дата:** 2026-11-02
- **Контекст:** `decimal.Round(x, 2)` за замовчуванням округлює до парного (ToEven): 2,345 стає 2,34. Користувач очікує 2,35. У наявному коді так округлюють `BillTotalCalculator` (ЛР 3) і `DiscountPricingPolicy` (ЛР 4); політика цін округлює кожен рядок окремо, що накопичує похибку. Нарахування за тарифом у варіанті 14 за вимогою округлюють угору.
- **Варіанти:** (А) лишити ToEven — нічого не змінювати; (Б) AwayFromZero для всіх сум; (В) AwayFromZero для сум і `Ceiling` лише для нарахування за тарифом.
- **Рішення:** Ухвалено варіант В: D-14 — AwayFromZero до 2 знаків, один раз на підсумку документа; X-04 — нарахування за тарифом угору. Чотири місця в наявному коді мають перейти на нове правило за бойскаутом, найпізніше до початку наступної лабораторної роботи.
- **Наслідки:** Старі тести з підсумками на межі 0,005 можуть змінити очікування; кожну таку зміну записують у PR. Приклад Е-22 фіксує правило.
- **Ухвалив:** Римарцов В.
- **Статус:** прийнято, виконується

### 7.6. Статистика рівнів

| Рівень | Кількість | Частка |
| --- | --- | --- |
| ОБОВ'ЯЗКОВО | 53 | 51 % |
| РЕКОМЕНДОВАНО | 32 | 31 % |
| ДОПУСКАЄТЬСЯ | 6 | 6 % |
| ЗАБОРОНЕНО | 12 | 12 % |
| **Разом правил** | **103** | 100 % |
