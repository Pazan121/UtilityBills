using System;
using System.Collections.Generic;

namespace UtilityBills.Legacy;

public class LegacyBillProcessor
{
    private decimal _tmpSum;
    private decimal _tmpDiscount;
    private readonly DiscountPolicy _discounts = new();
    private readonly ShippingPolicy _shipping = new();
    private readonly ReportBuilder _report = new();
    private readonly List<string> _log = new();

    public string Handle(int billId, int consumerId,
        string consumerName, string? consumerMail,
        ConsumerKind consumerKind, int consumerDone,
        List<ServiceLine>? lines, BillState state,
        string currency, DateTime createdAt, bool sendMail)
    {
        if (lines is null) return "ERR: null";
        if (lines.Count == 0) return "ERR: empty";
        if (state is not (BillState.New or BillState.Paid)) return "ERR: state";
        if (consumerMail is null || !consumerMail.Contains("@")) return "ERR: mail";
        _log.Add("ok " + billId);
        _tmpSum = Subtotal(lines);
        _tmpDiscount = _discounts.For(_tmpSum, consumerKind, consumerDone);
        decimal ship = _shipping.For(_tmpSum - _tmpDiscount);
        decimal total = _tmpSum - _tmpDiscount + ship;
        string txt = _report.Build(billId, consumerName, lines,
            currency, _tmpDiscount, ship, total);
        if (sendMail)
        {
            _log.Add("mail -> " + consumerMail);
        }
        return txt;
    }

    public decimal Preview(ConsumerKind consumerKind, int consumerDone,
        List<ServiceLine> lines)
    {
        decimal subtotal = Subtotal(lines);
        decimal discount = _discounts.For(subtotal, consumerKind, consumerDone);
        decimal payable = subtotal - discount;
        return payable + _shipping.For(payable);
    }

    public string DescribeClient(Consumer c)
    {
        string s = c.Name.Trim().ToUpper();
        if (c.Kind == ConsumerKind.Vip) s += " [VIP]";
        if (c.DoneCount > PricingRules.LoyalOrders) s += " [ЛОЯЛЬНИЙ]";
        s += " <" + c.Email.ToLower() + ">";
        int years = DateTime.Now.Year - c.SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }

    private static decimal Subtotal(List<ServiceLine> lines)
    {
        decimal sum = 0m;
        for (int i = 0; i < lines.Count; i++)
        {
            sum += lines[i].Quantity * lines[i].Tariff;
        }
        return sum;
    }
}
