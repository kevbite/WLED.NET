namespace Kevsoft.WLED;

/// <summary>
/// The live LED colour stream from the <c>/json/live</c> endpoint (only available when the
/// firmware is built with <c>WLED_ENABLE_JSONLIVE</c>).
/// </summary>
public sealed class LiveResponse
{
    /// <summary>The current colour of each LED.</summary>
    [JsonPropertyName("leds")]
    [JsonConverter(typeof(HexRgbColorArrayJsonConverter))]
    public RgbColor[] Leds { get; set; } = Array.Empty<RgbColor>();

    /// <summary>The strip length the colours map onto.</summary>
    [JsonPropertyName("n")]
    public int Length { get; set; }
}
