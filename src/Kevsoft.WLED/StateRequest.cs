namespace Kevsoft.WLED;

public sealed class StateRequest
{
    /// <inheritdoc cref="StateResponse.On"/>
    [JsonPropertyName("on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? On { get; set; }

    /// <inheritdoc cref="StateResponse.Brightness"/>
    [JsonPropertyName("bri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte? Brightness { get; set; }

    /// <inheritdoc cref="StateResponse.Transition"/>
    [JsonPropertyName("transition")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte? Transition { get; set; }

    /// <inheritdoc cref="StateResponse.PresetId"/>
    [JsonPropertyName("ps")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PresetId { get; set; }

    /// <inheritdoc cref="StateResponse.PlaylistId"/>
    [JsonPropertyName("pl")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PlaylistId { get; set; }

    /// <inheritdoc cref="StateResponse.Nightlight"/>
    [JsonPropertyName("nl")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public NightlightRequest? Nightlight { get; set; } = null!;

    /// <inheritdoc cref="StateResponse.UdpPackets"/>
    [JsonPropertyName("udpn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UdpPacketsRequest? UdpPackets { get; set; } = null!;

    /// <inheritdoc cref="StateResponse.LiveDataOverride"/>
    [JsonPropertyName("lor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte? LiveDataOverride { get; set; }

    /// <inheritdoc cref="StateResponse.MainSegment"/>
    [JsonPropertyName("mainseg")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MainSegment { get; set; }

    /// <inheritdoc cref="StateResponse.Segments"/>
    [JsonPropertyName("seg")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SegmentRequest[]? Segments { get; set; } = null!;

    /// <summary>
    /// Timebase for effects.
    /// </summary>
    [JsonPropertyName("tb")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Timebase { get; set; }

    public static StateRequest From(StateResponse stateResponse)
    {
        return new StateRequest()
        {
            On = stateResponse.On,
            Brightness = stateResponse.Brightness,
            Transition = stateResponse.Transition,
            PresetId = stateResponse.PresetId,
            PlaylistId = stateResponse.PlaylistId,
            Nightlight = stateResponse.Nightlight,
            UdpPackets = stateResponse.UdpPackets,
            LiveDataOverride = stateResponse.LiveDataOverride,
            MainSegment = stateResponse.MainSegment,
            Segments = stateResponse.Segments.Select(SegmentRequest.From).ToArray(),
            Timebase = stateResponse.Timebase
        };
    }
        
    public static implicit operator StateRequest(StateResponse rhs) => From(rhs);
}