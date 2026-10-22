# UtilityBills
Навчальний проєкт з дисципліни «Конструювання програмного забезпечення».
Варіант № 14: нарахування комунальних платежів.
## Склад рішення
- `UtilityBills.Core` — бібліотека з моделлю предметної області.
- `UtilityBills.App` — консольний застосунок для демонстрації моделі.
## Вимоги
.NET SDK 8.0 або новіший, Git.
## Складання та запуск
```bash
dotnet build
dotnet run --project UtilityBills.App
```
## Стандарт кодування
Правила описано у `CODING_STANDARD.md` і закріплено в `.editorconfig`.
Перевірка форматування: `dotnet format --verify-no-changes`.
## Обробка помилок і журнал

- Помилки класифіковано в `docs/errors.md`.
- Введення перевіряє `BillLineParser` (App) і повертає `Result<UtilityBillLine>`; ядро (`Core`) приймає лише типізовані дані.
- Порушення правил предметної області — `DomainRuleException` з полем `Rule`.
- Журнал: `ILogger` у `BillService`, у консолі застосунку — `AddSimpleConsole` з областями (`BeginScope`).
- Аудит дій пишеться в `audit.log` (`FileAuditLog`, `IDisposable`); файл у репозиторій не потрапляє.
## Автор
Римарцов Володимир Володимирович, група 471.
