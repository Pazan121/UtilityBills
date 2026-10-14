namespace UtilityBills.Core.Domain;

 

/// <summary>Стан рахунку за комунальні послуги.</summary>

public enum BillStatus

{

    Draft = 0,

    Issued = 1,

    Paid = 2,

    Overdue = 3,

    Cancelled = 4,

}