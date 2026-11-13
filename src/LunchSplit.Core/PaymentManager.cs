namespace LunchSplit.Core;

/// <summary>
/// Charges single payments and split-bill batches through an <see cref="IPaymentProcessor"/>.
/// </summary>
public class PaymentManager
{
    /// <summary>Domain error code returned when a $0.00 charge is requested.</summary>
    public const string ZeroAmount = "ZERO_AMOUNT";

    private readonly IPaymentProcessor _processor;

    /// <summary>
    /// Creates a manager that submits payments through <paramref name="processor"/>.
    /// </summary>
    /// <param name="processor">Gateway used to submit payments.</param>
    public PaymentManager(IPaymentProcessor processor)
    {
        _processor = processor ?? throw new ArgumentNullException(nameof(processor));
    }

    /// <summary>
    /// Charges a single amount.
    /// </summary>
    /// <param name="amount">Amount in dollars. Negative amounts are rejected.</param>
    /// <param name="currencyCode">ISO-4217 currency code, e.g. "CAD".</param>
    /// <param name="cardNonce">Card token to charge.</param>
    /// <param name="payerName">Optional payer name.</param>
    /// <returns>
    /// The gateway result, or a failed result with <see cref="ZeroAmount"/> when the amount is
    /// $0.00 (in which case the gateway is never called).
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">The amount is negative.</exception>
    /// <exception cref="PaymentGatewayTimeoutException">The gateway could not be reached.</exception>
    public async Task<PaymentResult> ChargeAsync(decimal amount, string currencyCode, string cardNonce, string? payerName = null)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Payment amount cannot be negative.");
        }

        // Precondition short-circuit: nothing is owed, so do not call the gateway at all.
        if (amount == 0m)
        {
            return PaymentResult.Failure(ZeroAmount, errorDetail: "A $0.00 payment does not need to be charged.");
        }

        // A new idempotency key per charge, so a retry of THIS request is never charged twice.
        PaymentRequest request = new PaymentRequest(amount, currencyCode, Guid.NewGuid().ToString(), cardNonce, payerName);

        try
        {
            return await _processor.ProcessPaymentAsync(request).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            // Network-level failure (DNS, connection dropped, socket error...).
            throw new PaymentGatewayTimeoutException("The payment gateway could not be reached.", ex);
        }
        catch (TaskCanceledException ex)
        {
            // HttpClient reports its own timeout as a TaskCanceledException.
            throw new PaymentGatewayTimeoutException("The payment gateway did not respond in time.", ex);
        }
    }

    /// <summary>
    /// Charges every attendee of a split bill, one gateway call per attendee who owes money.
    /// A failure for one attendee (decline or gateway timeout) does not stop the others.
    /// </summary>
    /// <param name="payments">Attendee shares and their card nonces.</param>
    /// <param name="currencyCode">ISO-4217 currency code for every payment.</param>
    /// <returns>A summary with one outcome per attendee, in input order.</returns>
    public async Task<BatchPaymentSummary> ProcessBatchAsync(IEnumerable<AttendeePayment> payments, string currencyCode)
    {
        ArgumentNullException.ThrowIfNull(payments);

        List<AttendeePaymentOutcome> outcomes = new List<AttendeePaymentOutcome>();

        foreach (AttendeePayment payment in payments)
        {
            // Excluded attendees end up with a $0.00 share: record them as skipped.
            if (payment.Amount == 0m)
            {
                outcomes.Add(new AttendeePaymentOutcome(payment.Name, payment.Amount, null));
                continue;
            }

            PaymentResult result;
            try
            {
                result = await ChargeAsync(payment.Amount, currencyCode, payment.CardNonce, payment.Name).ConfigureAwait(false);
            }
            catch (PaymentGatewayTimeoutException ex)
            {
                // Keep going: one unreachable attempt must not lose everyone else's payment.
                result = PaymentResult.Failure(PaymentResult.GatewayTimeout, errorDetail: ex.Message);
            }

            outcomes.Add(new AttendeePaymentOutcome(payment.Name, payment.Amount, result));
        }

        return new BatchPaymentSummary(outcomes);
    }
}