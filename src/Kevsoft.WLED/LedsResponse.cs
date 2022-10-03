namespace Kevsoft.WLED;

public sealed class LedsResponse
{
    /// <summary>
    /// Total LED count.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// Current frames per second. (available since 0.12.0)
    /// </summary>
    [JsonPropertyName("fps")]
    public byte Fps { get; set; }

    /// <summary>
    /// Logical AND of all active segment's virtual light capabilities
    /// </summary>
    [JsonPropertyName("lc")]
    public byte LightCapabilities { get; set; }

    /// <summary>
    /// Current LED power usage in milliamps as determined by the ABL. 0 if ABL is disabled.
    /// </summary>
    [JsonPropertyName("pwr")]
    public int PowerUsage { get; set; }

    /// <summary>
    /// Maximum power budget in milliamps for the ABL. 0 if ABL is disabled.
    /// </summary>
    [JsonPropertyName("maxpwr")]
    public int MaximumPower { get; set; }

    /// <summary>
    /// Maximum number of segments supported by this version.
    /// </summary>
    [JsonPropertyName("maxseg")]
    public byte MaximumSegments { get; set; }
}