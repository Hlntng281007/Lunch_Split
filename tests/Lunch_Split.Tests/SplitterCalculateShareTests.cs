using System.Linq;
using Lunch_Split.Model;
using Lunch_Split.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lunch_Split.Tests;
[TestClass]
public class CalculateShares_EqualSplit_ApportionsEvenly()
{
    Bill bill = new Bill (Subtotal: 90.00m, Tax: 0m, TipMode.none, tipInput: 0m);
    List<Attendee>attendees = new List<Attendee>
    {
        new("Alex", Weight: 1, Included: true),
        new ("Sam", Weight: 1, Included: true),
        new("Jo", Weight: 1, Included: true)
    };
    Splitter splitter = new Splitter();


}