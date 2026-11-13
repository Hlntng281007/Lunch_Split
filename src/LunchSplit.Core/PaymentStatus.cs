namespace LunchSplit.Core;

/// <summary>
/// Lifecycle state of a payment, independent of any vendor's wording.
/// </summary>
public enum PaymentStatus
{
    /// <summary>Unrecognised state, or the payment was not found.</summary>
    Unknown,
    /// <summary>Authorized but not yet captured.</summary>
    Approved,
    /// <summary>Still being processed.</summary>
    Pending,
    /// <summary>Captured; money has moved ("COMPLETED").</summary>
    Completed,
    /// <summary>Voided before capture.</summary>
    Canceled,
    /// <summary>Rejected or errored.</summary>
    Failed
}