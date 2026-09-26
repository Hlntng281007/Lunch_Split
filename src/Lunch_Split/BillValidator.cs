using System.Linq;
using Lunch_Split.Model;

namespace Lunch_Split.Validation;

public class BillValidator
{
    public ValidationResult Validate(Bill bill, List<Attendee> attendees)
    {
        if (bill.Subtotal < 0)
        {
            return ValidationResult.Fail("Subtotal cannot be negative.");
        }

        if (bill.Tax < 0)
        {
            return ValidationResult.Fail("Tax cannot be negative.");
        }

        if (attendees.Count == 0)
        {
            return ValidationResult.Fail("At least one attendee is required to split a bill.");
        }

        if (attendees.Any(a => a.Weight < 0))
        {
            return ValidationResult.Fail("Attendee weights cannot be negative.");
        }

        return ValidationResult.Ok();
    }
}