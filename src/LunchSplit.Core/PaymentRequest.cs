namespace LunchSplit.Core;

/// <summary>
/// One payment to submit to an <see cref="IPaymentProcessor"/>.
/// </summary>
/// <param name="Amount">Amount in dollars, e.g. 12.50m. At most 2 decimal places.</param>
/// <param name="CurrencyCode">ISO-4217 currency code, e.g. "CAD".</param>
/// <param name="IdempotencyKey">Unique key so a retried request is never charged twice.</param>
/// <param name="CardNonce">Single-use card token (Sandbox magic nonce, e.g. "cnon:card-nonce-ok").</param>
/// <param name="PayerName">Optional attendee name, used for notes and reporting.</param>
public record PaymentRequest(
    decimal Amount,
    string CurrencyCode,
    string IdempotencyKey,
    string CardNonce,
    string? PayerName = null);