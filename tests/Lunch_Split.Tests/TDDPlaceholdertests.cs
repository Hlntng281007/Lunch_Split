using Lunch_Split.Model;
using Lunch_Split.Future;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lunch_Split.Tests;
[TestClass]
public class TDDPlaceholderTests
{
    [TestMethod]
    public void Format_CsvReceiptExporter_MatchesSchema()
    {
        Bill bill = new Bill(Subtotal:50.00m, Tax: 5.00m, TipMode.none, tipInput: 0m);
        List<Attendee>attendees = new List<Attendee>
        {
            new("Alex", Weight: 1, Included: true)
        };
        CsvReceiptExporter exporter = new CsvReceiptExporter();

        string cvs = exporter.Export(bill, attendees, shares);

        StringAssert.StartsWith(cvs, "Name, Amout");
    }

    [TestMethod]
    public void CalculateShares_PaymentRequest_ValidatesStripeMock()
    {
        List <Share> shares = new List<Share>
        {
            new("Alex", 55.00m)
        };

        StripePaymentValidator validator = new StripePaymentValidator();

        bool IsValid = validator.ValidatePaymentRequesr(shares, "cus_mockStripeID");

        Assert.IsTrue(IsValid);
    }
}