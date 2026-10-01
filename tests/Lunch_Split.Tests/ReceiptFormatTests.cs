using Lunch_Split.Model;
using Lunch_Split.Output;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lunch_Split.Tests;
[TestClass]
public class ReceiptFormatTests
{
    [TestMethod]
    public void Format_DefaultBillReceipt_ConstainsLineItems()
    {
        Bill bill = new Bill (Subtotal: 100.00m, Tax: 10.00m, TipMode.none, tipInput: 0m);
        List<Attendee>attendees = new List<Attendee>
        {
            new("Alex", Weight: 1, Included: true),
            new("Sam", Weight: 1, Included: true)
        };

        List<Share> shares = new List<Share>
        {
            new("Alex", 55.00m),
            new("Sam", 55.00m)
        };

        ReceiptFormat format = new ReceiptFormat(() => new DateTime(2026, 9, 27, 14, 30, 0));

        string receipt = format.Format(bill,attendees,shares);

        StringAssert.Contains(receipt, "Alex");
        StringAssert.Contains(receipt, "$55.00");
        StringAssert.Contains(receipt, "Sam");
    }

    [TestMethod]
    public void Format_ReceiptOutput_ContainsRequiredMetaData()
    {
        Bill bill = new Bill (Subtotal: 50.00m, Tax: 5.00m, TipMode.none, tipInput: 0m);
        List<Attendee>attendees = new List<Attendee>
        {
            new("Alex", Weight: 1, Included: true)
        };
        List<Share> shares = new List<Share>
        {
            new("Alex", 55.00m)
        };
        DateTime fixedNow = new DateTime(2026, 9, 27, 14, 30, 0);
        ReceiptFormat format = new ReceiptFormat(()=>fixedNow);

        string receipt = format.Format(bill, attendees, shares);

        StringAssert.Contains(receipt, "Student");
        StringAssert.Contains(receipt, "2026-09-27");
        StringAssert.Contains(receipt, "14:30:00");
    }
}