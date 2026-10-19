# Класова модель варіанта 14 (UtilityBills)

Предметна область: нарахування комунальних платежів. Документ — рахунок (`UtilityBill`),
позиція — рядок рахунку (`UtilityBillLine`), довідник учасників — абонент (`Subscriber`),
довідник об'єктів обліку — комунальна послуга (`UtilityService`).

## Сутності та інваріанти

| Сутність | Роль | Незмінний | Інваріант |
| --- | --- | --- | --- |
| UtilityBillLine | позиція документа | так, record | UtilityServiceId > 0, Quantity > 0, Tariff >= 0 |
| Subscriber | довідник учасників | так, record | Id > 0, FullName і Address не порожні, Email містить @ |
| UtilityService | довідник обліку | так, record (новий тариф — новий об'єкт) | Id > 0, Name не порожнє, Tariff > 0 |
| UtilityBill | документ зі станом | ні, агрегат | Id > 0, SubscriberId > 0; рядки додають лише в стані Draft |

## Стани рахунку (BillStatus)

| Зі стану | До стану | Умова |
| --- | --- | --- |
| Draft | Issued | є хоча б один рядок |
| Draft | Cancelled | завжди |
| Issued | Paid | завжди |
| Issued | Overdue | сьогодні пізніше за DueDate |
| Issued | Cancelled | завжди |
| Overdue | Paid | завжди |

Інші переходи заборонені й кидають `InvalidOperationException`. Paid і Cancelled — кінцеві стани.
