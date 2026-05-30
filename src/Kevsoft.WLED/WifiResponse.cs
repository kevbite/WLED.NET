namespace Kevsoft.WLED;

/// <summary>
/// Information about the current Wi-Fi connection's signal strength.
/// </summary>
public sealed class WifiResponse
{
    /// <summary>The BSSID of the currently connected network.</summary>
    [JsonPropertyName("bssid")]
    public string Bssid { get; set; } = null!;

    /// <summary>Relative signal quality of the current connection (0–100).</summary>
    [JsonPropertyName("signal")]
    public int Signal { get; set; }

    /// <summary>The current Wi-Fi channel (1–14).</summary>
    [JsonPropertyName("channel")]
    public int Channel { get; set; }
}
