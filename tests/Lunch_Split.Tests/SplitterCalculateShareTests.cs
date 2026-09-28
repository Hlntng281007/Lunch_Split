using System.Linq;
using Lunch_Split.Model;
using Lunch_Split.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lunch_Split.Tests;

[TestClass]
public class SplitterCalculateSharesTests
{
    [TestMethod]
    public void CalculateShares_EqualSplit_ApportionEvenly()
    {
        Bill bill = new Bill(Subtotal: 90.00m, Tax: 0m, TipMode.none, tipInput: 0m);
        List<Attendee>attendees = new List<Attendee>
        {
            new ("Alex", Weight: 1, Included: true),
            new ("Sam", Weight: 1, Included: true),
            new ("Jo", Weight: 1, Included: true)
        };
        Splitter splitter = new Splitter();

        List<Share> result = splitter.CalculateShares(bill,attendees,RoundingMode.Bankers);

        Assert.AreEqual(30.00m, result[0].Amount);
        Assert.AreEqual(30.00m, result[1].Amount);
        Assert.AreEqual(30.00m, result[2].Amount);
    }

    [TestMethod]
    public void CalculateShares_PropotionalSplit_ApportionaWeighted()
    {
        Bill bill = new Bill(Subtotal: 90.00m, Tax:0m, TipMode.none,tipInput: 0m);
        List<Attendee> attendees = new List<Attendee>
        {
            new("Alex", Weight: 2, Included: true),
            new("Sam", Weight: 1, Included: true)
        };
        Splitter splitter = new Splitter();

        List<Share> result = splitter.CalculateShares(bill, attendees, RoundingMode.Bankers);

        Assert.AreEqual(60.00m, result[0].Amount);
        Assert.AreEqual(30.00m, result[1].Amount);
    }

    [TestMethod]
    public void CalculateShares_ExcludedAttendee_ApportionsZero()
    {
        Bill bill = new Bill(Subtotal: 60.00m, Tax: 0m, TipMode.none, tipInput: 0m);
        List<Attendee> attendees = new List<Attendee>
        {
            new ("Alex", Weight: 1, Included: true),
            new ("Sam", Weight: 1, Included: true),
            new ("Jo", Weight: 1, Included: true)

        };
        Splitter splitter = new Splitter();
        
        List<Share> result = splitter.CalculateShares(bill, attendees, RoundingMode.Bankers);

        Assert.AreEqual(30.00m, result[0].Amount);
        Assert.AreEqual(30.00m, result[1].Amount);
        Assert.AreEqual(0.00m, result[2].Amount);
    }

    [TestMethod]
    public void CalculateShares_TaxAndPercentTip_ApportionsTotal()
    {
        Bill bill = new Bill(Subtotal: 100.00m, Tax: 13.00m, TipMode.percent, tipInput: 15m);
        List<Attendee>attendees=new List<Attendee>
        {
            new Attendee("Alex", Weight: 1, Included: true)
        };
        Splitter splitter = new Splitter;

        List<Share> result = splitter.CalculateShares(bill, attendees, RoundingMode.Bankers);

        decimal grandTotal = result.Sum(Share => Share.Amount);
        Assert.AreEqual(128.00m, grandTotal);
    }

    [TestMethod]
    public void CalculateShares_TaxAndFixedTip_ApportionsTotal()
    {
        Bill bill = new Bill (Subtotal: 100.00m, Tax: 13.00m, TipMode.fixed_tip, tipInput: 20.00m);
        List<Attendee>attendees = new List<Attendee>
        {
            new ("Alex", Weight: 1, Included: true)
        };

        Splitter splitter = new Splitter();

        List<Share> result = splitter.CalculateShares(bill, attendees, RoundingMode.Bankers);

        decimal grandTotal = result.Sum(Share => Share.Amount);
        Assert.AreEqual(133.00m, grandTotal);
    }

    [TestMethod]
    public void CalculateShares_AllAttendeeExcluded_ThrowsException()
    {
        Bill bill = new Bill(Subtotal: 50.00m, Tax: 0m, TipMode.none, tipInput: 0m);
        List<Attendee> attendees = new List<Attendee>
        {
            new("Alex", Weight: 1, Included: false),
            new("Sam", Weight: 1, Included: false)
        };
        Splitter splitter = new Splitter();

        Assert.ThrowsExactly<InvalidOperationException>(()=>splitter.CalculateShares(bill, attendees, RoundingMode.Bankers));
    }

    [TestMethod]
    public void CalculateShares_TotalWeightsZero_ThrowsException()
    {
        Bill bill = new Bill (Subtotal: 50.00m, Tax: 0m, TipMode.none, tipInput: 0m);
        List<Attendee> attendees = new List<Attendee>
        {
            new("Alex", Weight: 0, Included: true),
            new("Sam", Weight: 0, Included: true)
        };
        Splitter splitter = new Splitter();

        Assert.ThrowsExactly<InvalidOperationException>(()=>splitter.CalculateShares(bill, attendees, RoundingMode.Bankers));
    }
}


