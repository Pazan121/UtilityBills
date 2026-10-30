using System.Globalization;
using UtilityBills.Legacy;
using Xunit;

namespace UtilityBills.Legacy.Tests;

public class LegacyCharacterizationTests
{
    public LegacyCharacterizationTests()
    {
        // формат чисел залежить від культури середовища,
        // тому фіксуємо її явно
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    private static List<ServiceLine> TwoLines() => new()
    {
        new ServiceLine { ServiceCode = "A-1", Quantity = 2, Tariff = 150m },
        new ServiceLine { ServiceCode = "B-2", Quantity = 1, Tariff = 400m },
    };

    private static List<ServiceLine> OneLine(int quantity, decimal tariff) => new()
    {
        new ServiceLine { ServiceCode = "C-3", Quantity = quantity, Tariff = tariff },
    };

    [Theory]
    [InlineData(null, "new", "a@b.c", "ERR: null")]
    [InlineData("empty", "new", "a@b.c", "ERR: empty")]
    [InlineData("two", "draft", "a@b.c", "ERR: state")]
    [InlineData("two", "new", "no-mail", "ERR: mail")]
    public void Handle_BadInput_ReturnsErrorCode(
        string? kindOfLines, string state, string mail,
        string expected)
    {
        List<ServiceLine>? lines = kindOfLines switch
        {
            "two" => TwoLines(),
            "empty" => new List<ServiceLine>(),
            _ => null,
        };
        var sut = new LegacyBillProcessor();
        var actual = sut.Handle(1001, 7, "Іван", mail, "regular", 0, lines, state, "UAH",
            new DateTime(2026, 3, 1), false);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Handle_VipOrder_ReturnsExactReport()
    {
        var sut = new LegacyBillProcessor();
        var report = sut.Handle(1001, 7, "Іван", "a@b.c", "vip", 3, TwoLines(), "new", "UAH",
            new DateTime(2026, 3, 1), false);
        var expected =
            "Документ #1001\n" +
            "Клієнт: Іван\n" +
            "A-1 x2 = 300.00 UAH\n" +
            "B-2 x1 = 400.00 UAH\n" +
            "Знижка: 105.00 UAH\n" +
            "Доставка: 60.00 UAH\n" +
            "Разом: 655.00 UAH\n";
        Assert.Equal(expected, report);
    }

    [Fact]
    public void Handle_SendMail_WritesToLog()
    {
        var sut = new LegacyBillProcessor();
        sut.Handle(1001, 7, "Іван", "a@b.c", "regular", 0, TwoLines(), "new", "UAH",
            new DateTime(2026, 3, 1), true);
        Assert.Equal("ok 1001\nmail -> a@b.c\n", sut.DumpLog());
    }

    [Theory]
    [InlineData("regular", 0, 760)]
    [InlineData("vip", 0, 655)]
    [InlineData("staff", 0, 550)]
    [InlineData("regular", 11, 725)]
    public void Preview_ByKind_ReturnsTotal(string kind, int done, decimal expected)
    {
        var sut = new LegacyBillProcessor();
        var total = sut.Preview(kind, done, TwoLines());
        Assert.Equal(expected, total);
    }

    [Fact]
    public void Preview_PayableExactly1000_HasNoShipping()
    {
        var sut = new LegacyBillProcessor();
        var total = sut.Preview("regular", 0, OneLine(4, 250m));
        Assert.Equal(1000m, total);
    }

    [Fact]
    public void Preview_StaffLargeBill_DiscountCappedAt500()
    {
        var sut = new LegacyBillProcessor();
        var total = sut.Preview("staff", 0, OneLine(4, 500m));
        Assert.Equal(1500m, total);
    }

    [Fact]
    public void DescribeClient_Vip_BuildsCaption()
    {
        var c = new Consumer
        {
            Name = " іван ", Email = "IVAN@MAIL.COM",
            Kind = "vip", DoneCount = 12,
            SinceUtc = new DateTime(2020, 1, 1)
        };
        // метод залежить від системного годинника,
        // тому очікування обчислюємо, а не вписуємо
        var years = DateTime.Now.Year - 2020;
        var expected = "ІВАН [VIP] [ЛОЯЛЬНИЙ] " +
            "<ivan@mail.com> стаж " + years;
        var actual = new LegacyBillProcessor().DescribeClient(c);
        Assert.Equal(expected, actual);
    }
}
