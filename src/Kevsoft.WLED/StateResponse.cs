namespace Kevsoft.WLED;

public sealed class StateResponse
{
    /// <summary>
    /// On/Off state of the light
    /// </summary>
    [JsonPropertyName("on")]
    public bool On { get; set; }

    /// <summary>
    /// Brightness of the light. If On is false, contains last brightness when light was on (aka brightness when On is set to true).
    /// </summary>
    [JsonPropertyName("bri")]
    public byte Brightness { get; set; }

    /// <summary>
    /// Duration of the crossfade between different colors/brightness levels. One unit is 100ms, so a value of 4 results in a transition of 400ms.
    /// </summary>
    [JsonPropertyName("transition")]
    public byte Transition { get; set; }

    /// <summary>
    /// ID of currently set preset, or <c>null</c> when none is active.
    /// </summary>
    [JsonPropertyName("ps")]
    [JsonConverter(typeof(NullableSentinelInt32JsonConverter))]
    public int? PresetId { get; set; }

    /// <summary>
    /// ID of currently set playlist, or <c>null</c> when none is active.
    /// </summary>
    [JsonPropertyName("pl")]
    [JsonConverter(typeof(NullableSentinelInt32JsonConverter))]
    public int? PlaylistId { get; set; }

    /// <summary>
    /// Nightlight 
    /// </summary>
    [JsonPropertyName("nl")]
    public NightlightResponse Nightlight { get; set; } = null!;

    /// <summary>
    /// UDP Packets
    /// </summary>
    [JsonPropertyName("udpn")]
    public UdpPacketsResponse UdpPackets { get; set; } = null!;

    /// <summary>
    /// Live data override. Off shows live data, or override until it ends / until reboot (available since 0.10.0).
    /// </summary>
    [JsonPropertyName("lor")]
    public LiveDataOverride LiveDataOverride { get; set; }

    /// <summary>
    /// Main Segment
    /// </summary>
    [JsonPropertyName("mainseg")]
    public int MainSegment { get; set; }

    /// <summary>
    /// Segments are individual parts of the LED strip.
    /// </summary>
    [JsonPropertyName("seg")]
    public SegmentResponse[] Segments { get; set; } = null!;
    
    /// <summary>
    /// Timebase for effects.
    /// </summary>
    [JsonPropertyName("tb")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Timebase { get; set; }
}