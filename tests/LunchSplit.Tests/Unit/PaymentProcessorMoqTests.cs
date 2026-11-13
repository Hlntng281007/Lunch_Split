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

    
    private static List<AttendeePayment> ToPayments(List<Share> shares) =>
        shares.Select(s => new AttendeePayment(s.Name, s.Amount, SandboxNonces.Ok)).ToList();
}