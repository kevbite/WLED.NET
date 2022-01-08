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
    /// ID of currently set preset.
    /// </summary>
    [JsonPropertyName("ps")]
    public int PresetId { get; set; }

    /// <summary>
    /// ID of currently set playlist. For now, this sets the preset cycle feature, -1 is off and 0 is on.
    /// </summary>
    [JsonPropertyName("pl")]
    public int PlaylistId { get; set; }

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
    /// Live data override. 0 is off, 1 is override until live data ends, 2 is override until ESP reboot (available since 0.10.0)
    /// </summary>
    [JsonPropertyName("lor")]
    public byte LiveDataOverride { get; set; }

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
}