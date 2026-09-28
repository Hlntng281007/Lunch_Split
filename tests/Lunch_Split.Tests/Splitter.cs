using System.Numerics;
using Lunch_Split.Model;
using Lunch_Split.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SplitterTest
{
    [TestClass]
    public class SplitterTest
    {
        private readonly Lunch_Split.Engine.Splitter _splitter = new();

        [TestMethod]
        public void ComputeTip_NoTipMode_ReturnsZero()
        {
            decimal billTotal = 100.00m;
            TipMode mode = TipMode.none;
            decimal expectedTip = 0.0m;

            decimal actualResult = _splitter.ComputeTip(billTotal, mode, 0m);

            Assert.AreEqual(expectedTip, actualResult);
        }

        [TestMethod]
         public void ComputeTip_PercentTipMode_ReturnsCorrectPercent()
        {
            decimal billTotal = 120.00m;
            decimal percentage = 15m;
            decimal expectedValue = 18.00m;
            TipMode tipMode = TipMode.percent;

            decimal actualResult = _splitter.ComputeTip(billTotal, tipMode, percentage);

            Assert.AreEqual(expectedValue, actualResult);
        }

        [TestMethod]
        public void ComputeTip_FixedTipMode_ReturnsFixedAmount()
        {
            decimal billTotal = 100.00m;
            TipMode mode = TipMode.fixed_tip;
            decimal fixedValue = 15.00m;
            decimal expectedValue = 15.00m;

            decimal actualResult = _splitter.ComputeTip(billTotal, mode, fixedValue);

            Assert.AreEqual(expectedValue, actualResult);
        }

        [TestMethod]
        public void ComputeTip_NegativeSubtotal_ThrowsException()
        {
            decimal billTotal = -300.00m;
            TipMode mode = TipMode.none;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => _splitter.ComputeTip(billTotal, mode, 0m));

        }

        [TestMethod]
        public void ComputeTip_NegativeFixedTip_ThrowsException()
        {
            decimal billTotal = 100.00m;
            TipMode mode = TipMode.fixed_tip;
            decimal tipInput = -15.00m;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => _splitter.ComputeTip(billTotal, mode, tipInput));


        }
    }
}
