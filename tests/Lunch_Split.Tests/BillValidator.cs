using Lunch_Split.Model;
using Lunch_Split.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Bson;

namespace Lunch_Split.Tests;

[TestClass]
public class BillValidatorTests
{
    [TestMethod]
    public void Validate_CompleteBillDetails_ReturnsOk()
    {
        Bill bill = new Bill (Subtotal: 90.00m, Tax: 9.00m, TipMode.percent, tipInput: 15m);
        List<Attendee>attendees = new List<Attendee>
        {
            new("Alex", Weight: 1, Included: true),
            new("Sam", Weight: 1, Included: true)
        };
        BillValidator validator = new BillValidator();

        ValidationResult result = validator.Validate(bill, attendees);

        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Validate_EmptyAttendeeCollection_ReturnsFail()
    {
        Bill bill = new Bill (Subtotal: 50.00m, Tax: 5.00m, TipMode.none, tipInput: 0m);
        List<Attendee> attendees = new List<Attendee>();
        BillValidator validator = new BillValidator();

        ValidationResult result = validator.Validate(bill,attendees);

        Assert.IsFalse(result.IsValid);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
    }

    [TestMethod]
    public void Validate_NegativeSUbtotal_ReturnsFail()
    {
        Bill bill = new Bill(Subtotal: -10.00m, Tax: 0m, TipMode.none, tipInput: 0m);
        List<Attendee> attendees = new List<Attendee>
        {
            new ("Alex", Weight: 1, Included: true)
        };
        BillValidator validator = new BillValidator;

        ValidationResult result = validator.Validate(bill, attendees);

        Assert.IsFalse(result.IsValid);
    }

    public void Validate_ZeroAttendees_ReturnsFail()
    {
        Bill bill = new Bill (Subtotal: 10.00m, Tax: 0m, TipMode.none, tipInput: 0m);
        List<Attendee> attendees = new List<Attendee> ();
        BillValidator validator = new BillValidator;

        ValidationResult result = validator.Validate(bill,attendees);
    }
}
