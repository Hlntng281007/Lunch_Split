using System.Linq;
using Lunch_Split.Model;
using Lunch_Split.Validation;
 
namespace Lunch_Split.Engine;
 

public class Splitter
{
    public decimal ComputeTip(decimal subtotal, TipMode mode, decimal tipInput)
    {
        if (subtotal < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(subtotal), "Subtotal cannot be negative.");
        }
 
        if (tipInput < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tipInput), "Tip input cannot be negative.");
        }
 
        return mode switch
        {
            TipMode.none => 0m,
            TipMode.percent => subtotal * (tipInput / 100m),
            TipMode.fixed_tip => tipInput,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), "Unrecognized tip mode.")
        };
    }
 
    public List<Share> CalculateShares(Bill bill, List<Attendee> attendees, RoundingMode roundingMode)
    {
        ValidationResult validation = new BillValidator().Validate(bill, attendees);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(validation.ErrorMessage);
        }
 
        List<Attendee> includedAttendees = attendees.Where(a => a.Included).ToList();
        if (includedAttendees.Count == 0)
        {
            throw new InvalidOperationException("At least one attendee must be included to split a bill.");
        }
 
        int totalWeight = includedAttendees.Sum(a => a.Weight);
        if (totalWeight == 0)
        {
            throw new InvalidOperationException("Total weight of included attendees cannot be zero.");
        }
 
        decimal tip = ComputeTip(bill.Subtotal, bill.TipMode, bill.tipInput);
        decimal grandTotal = bill.Subtotal + bill.Tax + tip;
 
        List<Share> rawShares = attendees
            .Select(a => new Share(a.Name, a.Included ? grandTotal * a.Weight / totalWeight : 0m))
            .ToList();
 
        return new Rounder().RoundShares(rawShares, roundingMode);
    }
}