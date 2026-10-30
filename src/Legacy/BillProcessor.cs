namespace UtilityBills.Legacy;

public sealed class BillProcessor
{
    private readonly DiscountPolicy _discounts = new();
    private readonly ShippingPolicy _shipping = new();
    private readonly ReportBuilder _report = new();
    private readonly List<string> _log = new();

    public string Handle(BillRequest req)
    {
        var error = Validate(req);
        if (error != null) return error;
        _log.Add("ok " + req.BillId);
        decimal subtotal = Subtotal(req.Lines!);
        decimal discount = _discounts.For(subtotal, req.Client);
        decimal shipping = _shipping.For(subtotal - discount);
        decimal total = subtotal - discount + shipping;
        string txt = _report.Build(req, discount, shipping, total);
        if (req.SendMail)
        {
            _log.Add("mail -> " + req.Client.Email);
        }
        return txt;
    }

    public decimal Preview(Consumer client,
        IReadOnlyList<ServiceLine> lines)
    {
        decimal subtotal = Subtotal(lines);
        decimal discount = _discounts.For(subtotal, client);
        decimal payable = subtotal - discount;
        return payable + _shipping.For(payable);
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }

    private static string? Validate(BillRequest req)
    {
        if (req.Lines is null) return "ERR: null";
        if (req.Lines.Count == 0) return "ERR: empty";
        if (req.State is not (BillState.New or BillState.Paid)) return "ERR: state";
        if (!req.Client.HasValidEmail) return "ERR: mail";
        return null;
    }

    private static decimal Subtotal(IReadOnlyList<ServiceLine> lines)
    {
        decimal sum = 0m;
        foreach (var line in lines)
        {
            sum += line.Quantity * line.Tariff;
        }
        return sum;
    }
}
