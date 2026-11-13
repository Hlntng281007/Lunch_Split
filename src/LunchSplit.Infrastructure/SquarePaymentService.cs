using Square;
using Square.Payments;

// Aliases keep LunchSplit's domain types distinct from any similarly named Square types.
using DomainPaymentProcessor = LunchSplit.Core.IPaymentProcessor;
using DomainPaymentRequest = LunchSplit.Core.PaymentRequest;
using DomainPaymentResult = LunchSplit.Core.PaymentResult;
using DomainPaymentStatus = LunchSplit.Core.PaymentStatus;

namespace LunchSplit.Infrastructure;

/// <summary>
/// <see cref="DomainPaymentProcessor"/> backed by the Square Payments API (Sandbox).
/// </summary>
public sealed class SquarePaymentService : DomainPaymentProcessor, IDisposable
{
    // Square error category for card problems (declines, bad CVV, insufficient funds...).
    private const string PaymentMethodErrorCategory = "PAYMENT_METHOD_ERROR";

    private readonly SquareOptions _options;
    private readonly HttpClient _httpClient;
    private readonly SquareClient _client;

    /// <summary>
    /// Creates the service and a Square client pointed at the Sandbox.
    /// </summary>
    /// <param name="options">Access token, optional location id and timeout.</param>
    public SquarePaymentService(SquareOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        // Explicit HttpClient timeout: a dropped socket fails fast instead of hanging `dotnet test`.
        _httpClient = new HttpClient { Timeout = options.Timeout };

        _client = new SquareClient(
            options.AccessToken,
            new ClientOptions
            {
                BaseUrl = SquareEnvironment.Sandbox,
                HttpClient = _httpClient,
            });
    }

    /// <inheritdoc />
    public async Task<DomainPaymentResult> ProcessPaymentAsync(DomainPaymentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Translate the domain DTO into Square's CreatePayment payload.
        CreatePaymentRequest squareRequest = new CreatePaymentRequest
        {
            SourceId = request.CardNonce,
            IdempotencyKey = request.IdempotencyKey,
            AmountMoney = new Money
            {
                Amount = ToMinorUnits(request.Amount),
                Currency = Currency.FromCustom(request.CurrencyCode.ToUpperInvariant()),
            },
            LocationId = _options.LocationId, // null => the account's main location
            Autocomplete = true,              // capture immediately so the payment ends up COMPLETED
            Note = request.PayerName is null ? null : $"LunchSplit share for {request.PayerName}",
        };

        try
        {
            var response = await _client.Payments.CreateAsync(squareRequest, CreateRequestOptions());
            var payment = response.Payment;

            if (payment is null || string.IsNullOrWhiteSpace(payment.Id))
            {
                return DomainPaymentResult.Failure("INVALID_GATEWAY_RESPONSE", 200,
                    errorDetail: "Square returned a success status without a payment id.");
            }

            DomainPaymentStatus status = MapStatus(Convert.ToString(payment.Status));
            if (status is DomainPaymentStatus.Failed or DomainPaymentStatus.Canceled)
            {
                return DomainPaymentResult.Failure("PAYMENT_NOT_COMPLETED", 200,
                    errorDetail: $"Square reported payment status {payment.Status}.");
            }

            return DomainPaymentResult.Success(payment.Id, 200);
        }
        catch (SquareApiException ex)
        {
            // Square signals declines and validation problems with HTTP 4xx + an errors[] array.
            // These are business outcomes, so they are returned instead of thrown.
            var firstError = ex.Errors?.FirstOrDefault();
            string? gatewayCode = firstError is null ? null : Convert.ToString(firstError.Code);
            string? category = firstError is null ? null : Convert.ToString(firstError.Category);

            return DomainPaymentResult.Failure(
                MapErrorCode(gatewayCode, category),
                ex.StatusCode,
                gatewayCode,
                firstError?.Detail ?? ex.Message);
        }
        // HttpRequestException / TaskCanceledException are deliberately NOT caught here:
        // PaymentManager converts them into PaymentGatewayTimeoutException.
    }

    /// <inheritdoc />
    public async Task<DomainPaymentStatus> GetTransactionStatusAsync(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
        {
            throw new ArgumentException("A transaction id is required.", nameof(transactionId));
        }

        try
        {
            // GET /v2/payments/{payment_id}
            var response = await _client.Payments.GetAsync(
                new GetPaymentsRequest { PaymentId = transactionId },
                CreateRequestOptions());

            return response.Payment is null
                ? DomainPaymentStatus.Unknown
                : MapStatus(Convert.ToString(response.Payment.Status));
        }
        catch (SquareApiException ex) when (ex.StatusCode == 404)
        {
            // Unknown payment id.
            return DomainPaymentStatus.Unknown;
        }
    }

    /// <summary>Releases the underlying HttpClient.</summary>
    public void Dispose() => _httpClient.Dispose();

    /// <summary>
    /// Per-request options: explicit timeout, and no automatic retries so each test
    /// performs exactly one call and finishes quickly.
    /// </summary>
    private RequestOptions CreateRequestOptions() => new RequestOptions
    {
        Timeout = _options.Timeout,
        MaxRetries = 0,
    };

    /// <summary>
    /// Converts dollars to cents, the unit Square expects (e.g. 12.50 -> 1250).
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Amount is zero/negative or has fractions of a cent.</exception>
    private static long ToMinorUnits(decimal amount)
    {
        if (amount <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        decimal cents = amount * 100m;
        if (cents != decimal.Truncate(cents))
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot contain fractions of a cent.");
        }

        return (long)cents;
    }

    /// <summary>
    /// Maps Square's error information to a LunchSplit domain error code.
    /// Every card problem (Square category PAYMENT_METHOD_ERROR, e.g. GENERIC_DECLINE,
    /// CVV_FAILURE, INSUFFICIENT_FUNDS) becomes "CARD_DECLINED"; the raw Square code is
    /// kept in PaymentResult.GatewayErrorCode.
    /// </summary>
    private static string MapErrorCode(string? gatewayCode, string? category)
    {
        if (string.Equals(category, PaymentMethodErrorCategory, StringComparison.OrdinalIgnoreCase))
        {
            return DomainPaymentResult.CardDeclined;
        }

        return string.IsNullOrWhiteSpace(gatewayCode) ? "GATEWAY_ERROR" : gatewayCode;
    }

    /// <summary>
    /// Maps Square's payment status text (APPROVED, PENDING, COMPLETED, CANCELED, FAILED).
    /// </summary>
    private static DomainPaymentStatus MapStatus(string? squareStatus) => squareStatus?.ToUpperInvariant() switch
    {
        "APPROVED" => DomainPaymentStatus.Approved,
        "PENDING" => DomainPaymentStatus.Pending,
        "COMPLETED" => DomainPaymentStatus.Completed,
        "CANCELED" => DomainPaymentStatus.Canceled,
        "FAILED" => DomainPaymentStatus.Failed,
        _ => DomainPaymentStatus.Unknown,
    };
}