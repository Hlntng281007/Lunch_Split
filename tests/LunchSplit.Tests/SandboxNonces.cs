namespace LunchSplit.Tests;

/// <summary>
/// Square Sandbox test card nonces that produce predictable outcomes.
/// </summary>
internal static class SandboxNonces
{
    /// <summary>HTTP 200 COMPLETED payment authorization (M3-01).</summary>
    public const string Ok = "cnon:card-nonce-ok";

    /// <summary>HTTP 400 card declined (M3-02).</summary>
    public const string Declined = "cnon:card-nonce-declined";
}