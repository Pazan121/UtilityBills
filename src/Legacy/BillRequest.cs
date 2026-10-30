namespace UtilityBills.Legacy;

public sealed record BillRequest(
    int BillId,
    Consumer Client,
    IReadOnlyList<ServiceLine>? Lines,
    BillState State,
    string Currency,
    bool SendMail);
