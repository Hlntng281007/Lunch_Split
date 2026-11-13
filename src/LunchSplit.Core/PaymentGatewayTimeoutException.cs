namespace LunchSplit.Core;

/// <summary>
/// Thrown when the payment gateway is unreachable or does not answer in time.
/// </summary>
public class PaymentGatewayTimeoutException : Exception
{
    /// <summary>Creates the exception with a message.</summary>
    public PaymentGatewayTimeoutException(string message) : base(message)
    {
    }

    /// <summary>Creates the exception, preserving the original transport error.</summary>
    public PaymentGatewayTimeoutException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}