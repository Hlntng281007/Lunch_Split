using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bill.Tests
{
    [TestClass]
    public class BillTests
    {
        [TestMethod]
        public void ComputeTip_NoTipMode_ReturnsZero()
        {
            //arrange
            decimal billTotal = 100.00m;
            TipMode tm = TipMode.none;
            decimal expectedTip = 0.0m;

            Bill bill = new Bill ();

            //act
            decimal actualResult = Bill.calculateTip(billTotal,tm);
            //assert
            Assert.AreEqual(expectedTip,actualResult);
        }

        [TestMethod]
         public void ComputeTip_PercentTipMode_ReturnsCorrectPercent()
        {
            decimal billTotal = 120.00m;
            decimal percentage = 0.15m;
            decimal expectValue = 18.00m;
            TipMode tipMode = TipMode.percent;

            Bill bill = new Bill();

            decimal actualResult = Bill.calculateTip(billTotal,tipMode, percentage);

            Assert.AreEqual(expectValue, actualResult);
        }

        [TestMethod]
        public void ComputeTip_FixedTipMode_ReturnsFixedAmount()
        {
            decimal billTotal = 100.00m;
            TipMode tm = TipMode.fixed_tip;
            decimal fixedValue = 15.00m;
            decimal expectedValue = 15.00m;

            Bill bill = new Bill();

            decimal actualResult = Bill.calculateTip(billTotal, tm, fixedValue);

            Assert.AreEqual(expectedValue, actualResult);
        }

        [TestMethod]
        public void ComputeTip_NegativeSubtotal_ThrowsException()
        {
            decimal billTotal = -300.00m;
            TipMode tm = TipMode.none;
            decimal tipInput = 0m;

            Bill bill = new Bill();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Bill.calculateTip(billTotal, tm, tipInput));

        }

        [TestMethod]
        public void ComputeTip_NegativeFixedTip_ThrowsException()
        {
            decimal billTotal = 100.00m;
            TipMode tm = TipMode.fixed_tip;
            decimal tipInput = -15.00m;

            Bill bill = new Bill();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Bill.calculateTip(billTotal, tm, tipInput));


        }
    }
  
}
