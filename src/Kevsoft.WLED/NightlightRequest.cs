namespace Kevsoft.WLED;

public sealed class NightlightRequest
{
    /// <inheritdoc cref="NightlightRequest.On"/>
    [JsonPropertyName("on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? On { get; set; }

    /// <inheritdoc cref="NightlightRequest.Duration"/>
    [JsonPropertyName("dur")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Duration { get; set; }

    /// <inheritdoc cref="NightlightRequest.Mode"/>
    [JsonPropertyName("mode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte? Mode { get; set; }

    /// <inheritdoc cref="NightlightRequest.TargetBrightness"/>
    [JsonPropertyName("tbri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TargetBrightness { get; set; }
        
    public static NightlightRequest From(NightlightResponse nightlightResponse)
    {
        return new NightlightRequest
        {
            On = nightlightResponse.On,
            Duration = nightlightResponse.Duration,
            Mode = nightlightResponse.Mode,
            TargetBrightness = nightlightResponse.TargetBrightness
        };
    }
        
    public static implicit operator NightlightRequest(NightlightResponse rhs) => From(rhs);
}