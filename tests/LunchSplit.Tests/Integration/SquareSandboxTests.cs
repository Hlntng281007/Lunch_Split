using LunchSplit.Core;
using LunchSplit.Infrastructure;

namespace LunchSplit.Tests.Integration;
[TestClass]
[TestCategory("Integration")]
public class SquareSandboxTests
{
    private const string Cad = "CAD";

    private static SquarePaymentService? _service;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        SquareOptions? options = SquareOptions.Load(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
        if (options is not null)
        {
            _service = new SquarePaymentService(options);
        }
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _service?.Dispose();
    }

    [TestMethod]
    public async Task M3_01_ProcessPayment_OkNonce_ReturnsSuccessAndTransactionId()
    {
        //Arange
        SquarePaymentService service = RequireSandbox();
        PaymentRequest request = NewRequest (12.50m, Cad, SandboxNonces.Ok);

        //Act
        PaymentResult result = await service.ProcessPaymentAsync(request);

        //Assert
        Assert.IsTrue(result.IsSuccess, $"Expected success but got {result.ErrorCode}: {result.ErrorDetail}");
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.TransactionId), "A successful payment must carry a transaction Id.");
        
    }

    [TestMethod]
    public async Task M3_02_ProcessPayment_DeclinedNonce_ReturnsDeclinedError()
    {
        //Arange
        SquarePaymentService service = RequireSandbox();
        PaymentRequest request = NewRequest (12.50m, Cad, SandboxNonces.Declined);

        //Act
        PaymentResult result = await service.ProcessPaymentAsync(request);

        //Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("CARD_DECLINED", result.ErrorCode, $"Square returned raw code {result.GatewayErrorCode}: {result.ErrorDetail}");
    }

     [TestMethod]
    public async Task M3_08_ProcessPayment_UnsupportedCurrency_ReturnsHttp400()
    {
        // Arrange
        SquarePaymentService service = RequireSandbox();
        PaymentRequest request = NewRequest(12.50m, "EUR", SandboxNonces.Ok);

        // Act
        PaymentResult result = await service.ProcessPaymentAsync(request);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(400, result.HttpStatusCode,
            $"Square returned {result.GatewayErrorCode}: {result.ErrorDetail}");
    }

    private static SquarePaymentService RequireSandbox()
    {
        if(_service is null)
        {
            Assert.Inconclusive(
                $"Square Sanbox credentials not found. Set {SquareOptions.AccessTokenVariable}" +
                "or copy appsettings.example.json to appsettings.json."
            );
        }

        return _service;
    }
    private static PaymentRequest NewRequest (decimal amount, string currency, string nonce) =>
    new PaymentRequest(amount, currency, Guid.NewGuid().ToString(), nonce, "Alex");

}