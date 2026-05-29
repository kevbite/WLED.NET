namespace Kevsoft.WLED;

/// <summary>
/// A nearby Wi-Fi network reported by the <c>/json/net</c> endpoint.
/// </summary>
public sealed class NetworkResponse
{
    /// <summary>The network SSID.</summary>
    [JsonPropertyName("ssid")]
    public string Ssid { get; set; } = null!;

    /// <summary>The received signal strength indicator, in dBm.</summary>
    [JsonPropertyName("rssi")]
    public int Rssi { get; set; }

    /// <summary>The network BSSID.</summary>
    [JsonPropertyName("bssid")]
    public string Bssid { get; set; } = null!;

    /// <summary>The Wi-Fi channel.</summary>
    [JsonPropertyName("channel")]
    public int Channel { get; set; }

    /// <summary>The raw encryption type as reported by the device.</summary>
    [JsonPropertyName("enc")]
    public int Encryption { get; set; }
}

/// <summary>Wrapper for the <c>/json/net</c> response.</summary>
internal sealed class NetworksResponse
{
    [JsonPropertyName("networks")]
    public NetworkResponse[] Networks { get; set; } = Array.Empty<NetworkResponse>();
}
