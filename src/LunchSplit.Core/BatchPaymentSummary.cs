namespace LunchSplit.Core;

/// <summary>
/// One attendee's share to be charged, together with the card nonce they paid with.
/// </summary>
/// <param name="Name">Attendee name.</param>
/// <param name="Amount">Rounded share amount in dollars.</param>
/// <param name="CardNonce">Card token for this attendee.</param>
public record AttendeePayment(string Name, decimal Amount, string CardNonce);

/// <summary>
/// The result of charging one attendee within a batch.
/// </summary>
/// <param name="Name">Attendee name.</param>
/// <param name="Amount">Amount that was (or would have been) charged.</param>
/// <param name="Result">Gateway result, or null when the attendee was skipped ($0.00 share).</param>
public record AttendeePaymentOutcome(string Name, decimal Amount, PaymentResult? Result)
{
    /// <summary>True when the attendee owed nothing and the gateway was not called.</summary>
    public bool WasSkipped => Result is null;
}

/// <summary>
/// Summary of a batch of attendee payments.
/// </summary>
/// <param name="Outcomes">One outcome per attendee, in input order.</param>
public record BatchPaymentSummary(IReadOnlyList<AttendeePaymentOutcome> Outcomes)
{
    /// <summary>Number of attendees whose payment succeeded.</summary>
    public int SuccessCount => Outcomes.Count(o => o.Result is { IsSuccess: true });

    /// <summary>Number of attendees whose payment failed (declined or gateway error).</summary>
    public int FailureCount => Outcomes.Count(o => o.Result is { IsSuccess: false });

    /// <summary>Number of attendees skipped because they owed $0.00.</summary>
    public int SkippedCount => Outcomes.Count(o => o.WasSkipped);
}