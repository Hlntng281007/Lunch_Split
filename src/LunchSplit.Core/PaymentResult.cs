namespace LunchSplit.Core;

/// <summary>
/// Outcome of a single payment attempt.
/// </summary>
/// <param name="IsSuccess">True when the gateway accepted the payment.</param>
/// <param name="TransactionId">Gateway payment id (only set on success).</param>
/// <param name="ErrorCode">LunchSplit domain error code, e.g. "CARD_DECLINED" (only set on failure).</param>
/// <param name="HttpStatusCode">HTTP status returned by the gateway, when known (e.g. 200, 400).</param>
/// <param name="GatewayErrorCode">The vendor's raw error code, kept for diagnostics.</param>
/// <param name="ErrorDetail">Human-readable explanation of the failure.</param>
public record PaymentResult(
    bool IsSuccess,
    string? TransactionId,
    string? ErrorCode,
    int? HttpStatusCode = null,
    string? GatewayErrorCode = null,
    string? ErrorDetail = null)
{
    /// <summary>Domain error code used when a card is declined by the issuer.</summary>
    public const string CardDeclined = "CARD_DECLINED";

    /// <summary>Domain error code used when the gateway could not be reached in time.</summary>
    public const string GatewayTimeout = "GATEWAY_TIMEOUT";

    /// <summary>Creates a successful result.</summary>
    public static PaymentResult Success(string transactionId, int? httpStatusCode = 200) =>
        new(true, transactionId, null, httpStatusCode);

    /// <summary>Creates a failed result.</summary>
    public static PaymentResult Failure(string errorCode, int? httpStatusCode = null,
        string? gatewayErrorCode = null, string? errorDetail = null) =>
        new(false, null, errorCode, httpStatusCode, gatewayErrorCode, errorDetail);
}