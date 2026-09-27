using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using Lunch_Split.Model;
using Lunch_Split.Engine;

namespace Lunch_Split.Tests;

[TestClass]
public class RounderTest
{
    public void RoundShares_NoRounding_PreservesRawDecimals()
    {
        List<Share> rawShares = new List<Share>
        {
            new ("Alex", 3.3333m),
            new Share("Sam", 3.3333m)
        };

        Rounder rounder = new Rounder();

        List<Share> result = rounder.RoundShares(rawShares,RoundingMode.None);

        Assert.AreEqual(3.3333m, result [0].Amount);
        Assert.AreEqual(3.3333m, result [1].Amount);
    }

    [TestMethod]
    public void RoundShares_RoundUp_AppliesCeilingToCents()
    {
        List<Share> rawShares = new List<Share>
        {
            new("Alex", 10.331m)
        };
        Rounder rounder = new Rounder();

        List <Share> result = rounder.RoundShares(rawShares, RoundingMode.Up);

        Assert.AreEqual(10.34m, result[0].Amount);

    }

    [TestMethod]
    public void RoundShares_RoundUp_AppliesFloorToCents()
    {
        List<Share> rawShares = new List<Share>
        {
            new ("Alex", 10.339m)
        };

        Rounder rounder = new Rounder();

        List<Share> result = rounder.RoundShares(rawShares,RoundingMode.Down);

        Assert.AreEqual(10.33m, result[0].Amount);

    }

    [TestMethod]
    public void RoundShares_UnevenSplit_ReconcileRemainder()
    {
        decimal rawEach = 10.00m / 3m;
        List<Share> rawShares = new List<Share>
        {
        new("Alex", rawEach),
        new("Sam", rawEach),
        new("Jo", rawEach)

        };
        Rounder rounder = new Rounder();

        List<Share> result = rounder.RoundShares(rawShares,RoundingMode.Bankers);

        Assert.AreEqual(3.34m, result[0].Amount);
        Assert.AreEqual(3.33m, result[1].Amount);
        Assert.AreEqual(3.33m, result[2].Amount);
        decimal total = result.Sum(share => share.Amount);
        Assert.AreEqual(10.00m, total);
    }
    [TestMethod]
    public void RoundShares_EmptyShareCollection_ReturtnsEmpty()
    {
        List <Share> rawShares = new List<Share>();
        Rounder rounder = new Rounder();

        List<Share> result = rounder.RoundShares(rawShares, RoundingMode.Bankers);

        Assert.AreEqual(0, result.Count);

    }
}

