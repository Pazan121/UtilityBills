using System;
using System.Collections.Generic;

namespace UtilityBills.Legacy;

public class LegacyBillProcessor
{
    private decimal _tmpSum;
    private decimal _tmpDiscount;
    private readonly List<string> _log = new();

    public string Handle(int billId, int consumerId,
        string consumerName, string? consumerMail,
        ConsumerKind consumerKind, int consumerDone,
        List<ServiceLine>? lines, BillState state,
        string currency, DateTime createdAt, bool sendMail)
    {
        if (lines != null)
        {
            if (lines.Count > 0)
            {
                if (state == BillState.New || state == BillState.Paid)
                {
                    if (consumerMail != null &&
                        consumerMail.Contains("@"))
                    {
                        _log.Add("ok " + billId);
                    }
                    else
                    {
                        return "ERR: mail";
                    }
                }
                else
                {
                    return "ERR: state";
                }
            }
            else
            {
                return "ERR: empty";
            }
        }
        else
        {
            return "ERR: null";
        }
        _tmpSum = 0m;
        for (int i = 0; i < lines.Count; i++)
        {
            _tmpSum += lines[i].Quantity * lines[i].Tariff;
        }
        _tmpDiscount = 0m;
        if (consumerKind == ConsumerKind.Vip)
        {
            _tmpDiscount = _tmpSum * PricingRules.VipRate;
            if (_tmpDiscount > PricingRules.MaxDiscount) _tmpDiscount = PricingRules.MaxDiscount;
        }
        else if (consumerKind == ConsumerKind.Staff)
        {
            _tmpDiscount = _tmpSum * PricingRules.StaffRate;
            if (_tmpDiscount > PricingRules.MaxDiscount) _tmpDiscount = PricingRules.MaxDiscount;
        }
        else if (consumerDone > PricingRules.LoyalOrders)
        {
            _tmpDiscount = _tmpSum * PricingRules.LoyalRate;
            if (_tmpDiscount > PricingRules.MaxDiscount) _tmpDiscount = PricingRules.MaxDiscount;
        }
        decimal ship = 0m;
        if (_tmpSum - _tmpDiscount < PricingRules.FreeShipFrom) ship = PricingRules.ShipCost;
        decimal total = _tmpSum - _tmpDiscount + ship;
        string txt = "Документ #" + billId + "\n";
        txt += "Клієнт: " + consumerName + "\n";
        for (int i = 0; i < lines.Count; i++)
        {
            txt += lines[i].ServiceCode + " x" + lines[i].Quantity + " = " +
                (lines[i].Quantity * lines[i].Tariff)
                .ToString("0.00") + " " + currency + "\n";
        }
        txt += "Знижка: " + _tmpDiscount.ToString("0.00") +
            " " + currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") +
            " " + currency + "\n";
        txt += "Разом: " + total.ToString("0.00") +
            " " + currency + "\n";
        if (sendMail)
        {
            _log.Add("mail -> " + consumerMail);
        }
        return txt;
    }

    public decimal Preview(ConsumerKind consumerKind, int consumerDone,
        List<ServiceLine> lines)
    {
        decimal s = 0m;
        foreach (var it in lines)
        {
            s += it.Quantity * it.Tariff;
        }
        decimal d = 0m;
        if (consumerKind == ConsumerKind.Vip)
        {
            d = s * PricingRules.VipRate;
            if (d > PricingRules.MaxDiscount) d = PricingRules.MaxDiscount;
        }
        else if (consumerKind == ConsumerKind.Staff)
        {
            d = s * PricingRules.StaffRate;
            if (d > PricingRules.MaxDiscount) d = PricingRules.MaxDiscount;
        }
        else if (consumerDone > PricingRules.LoyalOrders)
        {
            d = s * PricingRules.LoyalRate;
            if (d > PricingRules.MaxDiscount) d = PricingRules.MaxDiscount;
        }
        decimal sh = 0m;
        if (s - d < PricingRules.FreeShipFrom) sh = PricingRules.ShipCost;
        return s - d + sh;
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
}
