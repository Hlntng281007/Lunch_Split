namespace LunchSplit.Core;

public interface IPaymentProcessor{

/// <summary>
/// Abstraction over an external payment gateway.
/// </summary>
 /// <param name="request">Amount, currency, idempotency key and card nonce.</param>
/// <returns>
/// A <see cref="PaymentResult"/>. Business failures (e.g. a declined card) are returned
/// as IsSuccess == false rather than thrown. Network failures may throw
/// (e.g. <see cref="HttpRequestException"/>).
/// </returns>
Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);

/// <summary>
/// Looks up the current status of a previously submitted payment.
/// </summary>
/// <param name="transactionId">The gateway's payment / transaction id.</param>
/// <returns>The payment's current <see cref="PaymentStatus"/>.</returns>
    Task<PaymentStatus> GetTransactionStatusAsync(string transactionId);
}
