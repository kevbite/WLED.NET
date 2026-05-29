namespace Kevsoft.WLED;

/// <summary>
/// Nightlight
/// </summary>
public sealed class NightlightResponse
{
    /// <summary>
    /// Nightlight currently active.
    /// </summary>
    [JsonPropertyName("on")]
    public bool On { get; set; }

    /// <summary>
    /// Duration of nightlight in minutes
    /// </summary>
    [JsonPropertyName("dur")]
    public int Duration { get; set; }

    /// <summary>
    /// Nightlight mode (instant, fade, color fade, sunrise) (available since 0.10.2).
    /// </summary>
    [JsonPropertyName("mode")]
    public NightlightMode Mode { get; set; }

    /// <summary>
    /// Target brightness.
    /// </summary>
    [JsonPropertyName("tbri")]
    public int TargetBrightness { get; set; }

    /// <summary>
    /// Remaining nightlight duration in seconds, or <c>null</c> when nightlight is inactive.
    /// </summary>
    [JsonPropertyName("rem")]
    [JsonConverter(typeof(NullableSentinelInt32JsonConverter))]
    public int? Remaining { get; set; }
}