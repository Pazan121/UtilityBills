using UtilityBills.Core.Domain;

 

UtilityBillLine line = new()

{

    UtilityServiceId = 1,

    Quantity = 10m,

    Tariff = 5m,

};

 

UtilityBill bill = new()

{

    Id = 1,

    SubscriberId = 1,

    BillingPeriod = "2026-10",

    Lines = new[] { line },

};

 

Console.WriteLine($"Рахунок №{bill.Id}: {bill.Total} грн");