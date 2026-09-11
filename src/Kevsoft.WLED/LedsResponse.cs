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
    public LightCapability LightCapabilities { get; set; }

    /// <summary>
    /// Per-segment virtual light capabilities.
    /// </summary>
    [JsonPropertyName("seglc")]
    public LightCapability[] SegmentLightCapabilities { get; set; } = Array.Empty<LightCapability>();

    /// <summary>
    /// <c>true</c> if LEDs are 4-channel (RGB + White). Deprecated in favour of <see cref="LightCapabilities"/>.
    /// </summary>
    [JsonPropertyName("rgbw")]
    // [Obsolete("Use LightCapabilities instead.")]
    public bool Rgbw { get; set; }

    /// <summary>
    /// <c>true</c> if a white channel slider should be displayed. Deprecated in favour of <see cref="LightCapabilities"/>.
    /// </summary>
    [JsonPropertyName("wv")]
    [JsonConverter(typeof(DeprecatedBooleanJsonConverter))]
    // [Obsolete("Use LightCapabilities instead.")]
    public bool? WhiteValueSlider { get; set; }

    /// <summary>
    /// <c>true</c> if the light supports colour temperature control. Deprecated in favour of <see cref="LightCapabilities"/>.
    /// </summary>
    [JsonPropertyName("cct")]
    [JsonConverter(typeof(DeprecatedBooleanJsonConverter))]
    // [Obsolete("Use LightCapabilities instead.")]
    public bool? SupportsColorTemperature { get; set; }

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