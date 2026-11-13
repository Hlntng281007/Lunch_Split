using System.Text.Json;

namespace LunchSplit.Infrastructure;

/// <summary>
/// Settings required to call the Square Sandbox.
/// </summary>
/// <param name="AccessToken">Sandbox access token (required).</param>
/// <param name="LocationId">Sandbox location id (optional; null means the main location).</param>
/// <param name="Timeout">Maximum time to wait for any single Square call.</param>
public record SquareOptions(string AccessToken, string? LocationId, TimeSpan Timeout)
{
    /// <summary>Environment variable holding the sandbox access token (named in the A3 setup guide).</summary>
    public const string AccessTokenVariable = "SQUARE_SANDBOX_ACCESS_TOKEN";

    /// <summary>Optional environment variable holding the sandbox location id.</summary>
    public const string LocationIdVariable = "SQUARE_SANDBOX_LOCATION_ID";

    /// <summary>Explicit 5-second timeout so scripted runs never hang on a dropped socket.</summary>
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Loads settings. Environment variables win; otherwise the "Square" section of
    /// <paramref name="jsonPath"/> is used, e.g.
    /// { "Square": { "AccessToken": "...", "LocationId": "...", "TimeoutSeconds": 5 } }.
    /// </summary>
    /// <param name="jsonPath">Optional path to an appsettings.json file.</param>
    /// <returns>The settings, or null when no access token is configured.</returns>
    public static SquareOptions? Load(string? jsonPath = null)
    {
        string? token = Environment.GetEnvironmentVariable(AccessTokenVariable);
        string? locationId = Environment.GetEnvironmentVariable(LocationIdVariable);
        TimeSpan timeout = DefaultTimeout;

        if (jsonPath is not null && File.Exists(jsonPath))
        {
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(jsonPath));
            if (document.RootElement.TryGetProperty("Square", out JsonElement square))
            {
                token ??= ReadString(square, "AccessToken");
                locationId ??= ReadString(square, "LocationId");
                if (square.TryGetProperty("TimeoutSeconds", out JsonElement seconds) && seconds.TryGetInt32(out int value) && value > 0)
                {
                    timeout = TimeSpan.FromSeconds(value);
                }
            }
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        return new SquareOptions(token, string.IsNullOrWhiteSpace(locationId) ? null : locationId, timeout);
    }

    /// <summary>Reads a string property, treating blanks and placeholders as missing.</summary>
    private static string? ReadString(JsonElement parent, string name)
    {
        if (!parent.TryGetProperty(name, out JsonElement element) || element.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        string? value = element.GetString();
        return string.IsNullOrWhiteSpace(value) || value.StartsWith("YOUR_", StringComparison.Ordinal) ? null : value;
    }
}