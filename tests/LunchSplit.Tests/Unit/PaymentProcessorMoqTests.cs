using Lunch_Split.Engine;
using Lunch_Split.Model;
using LunchSplit.Core;
using Moq;

namespace LunchSplit.Tests.Unit;

[TestClass]
[TestCategory("Unit")]
public class PaymentProcessorMoqTests
{
    private const string Cad = "CAD";

    
    [TestMethod]
    public async Task M3_03_Charge_GatewayThrowsHttpRequestException_ThrowsPaymentGatewayTimeoutException()
    {
        // Arrange
        Mock<IPaymentProcessor> processor = new Mock<IPaymentProcessor>();
        processor
            .Setup(p => p.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
            .ThrowsAsync(new HttpRequestException("Simulated network timeout"));
        PaymentManager manager = new PaymentManager(processor.Object);

        // Act + Assert
        await Assert.ThrowsExactlyAsync<PaymentGatewayTimeoutException>(
            () => manager.ChargeAsync(25.00m, Cad, SandboxNonces.Ok));
    }

    [TestMethod]
    public async Task M3_04_Charge_ValidAmount_InvokesProcessorWithExactAmount()
    {
        // Arrange
        Mock<IPaymentProcessor> processor = new Mock<IPaymentProcessor>();
        processor
            .Setup(p => p.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
            .ReturnsAsync(PaymentResult.Success("txn-001"));
        PaymentManager manager = new PaymentManager(processor.Object);

        // Act
        await manager.ChargeAsync(42.75m, Cad, SandboxNonces.Ok);

        // Assert
        processor.Verify(p => p.ProcessPaymentAsync(It.Is<PaymentRequest>(r =>
                r.Amount == 42.75m &&
                r.CurrencyCode == Cad &&
                r.CardNonce == SandboxNonces.Ok)),
            Times.Once());
    }

    [TestMethod]
    public async Task M3_05_ProcessBatch_ZeroDollarBill_NeverInvokesProcessor()
    {
        // Arrange
        Mock<IPaymentProcessor> processor = new Mock<IPaymentProcessor>();
        PaymentManager manager = new PaymentManager(processor.Object);
        Bill bill = new Bill(Subtotal: 0.00m, Tax: 0.00m, TipMode.none, tipInput: 0m);
        List<Attendee> attendees = new List<Attendee>
        {
            new("Alex", Weight: 1, Included: true),
            new("Sam", Weight: 1, Included: true)
        };
        List<AttendeePayment> payments = ToPayments(new Splitter().CalculateShares(bill, attendees, RoundingMode.Bankers));

        // Act
        await manager.ProcessBatchAsync(payments, Cad);

        // Assert
        processor.Verify(p => p.ProcessPaymentAsync(It.IsAny<PaymentRequest>()), Times.Never());
    }

    [TestMethod]
    public async Task M3_06_ProcessBatch_FourAttendees_InvokesProcessorExactlyFourTimes()
    {
        // Arrange
        Mock<IPaymentProcessor> processor = new Mock<IPaymentProcessor>();
        processor
            .Setup(p => p.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
            .ReturnsAsync((PaymentRequest r) => PaymentResult.Success($"txn-{r.PayerName}"));
        PaymentManager manager = new PaymentManager(processor.Object);
        Bill bill = new Bill(Subtotal: 100.00m, Tax: 0.00m, TipMode.none, tipInput: 0m);
        List<Attendee> attendees = new List<Attendee>
        {
            new("Alex", Weight: 1, Included: true),
            new("Sam", Weight: 1, Included: true),
            new("Jo", Weight: 1, Included: true),
            new("Kim", Weight: 1, Included: true)
        };
        List<AttendeePayment> payments = ToPayments(new Splitter().CalculateShares(bill, attendees, RoundingMode.Bankers));

        // Act
        await manager.ProcessBatchAsync(payments, Cad);

        // Assert
        processor.Verify(p => p.ProcessPaymentAsync(It.IsAny<PaymentRequest>()), Times.Exactly(4));
    }
    private static List<AttendeePayment> ToPayments(List<Share> shares) =>
        shares.Select(s => new AttendeePayment(s.Name, s.Amount, SandboxNonces.Ok)).ToList();
}