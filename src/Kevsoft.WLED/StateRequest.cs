namespace Kevsoft.WLED;

public sealed class StateRequest
{
    /// <inheritdoc cref="StateResponse.On"/>
    [JsonPropertyName("on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Toggleable? On { get; set; }

    /// <inheritdoc cref="StateResponse.Brightness"/>
    [JsonPropertyName("bri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ByteAdjust? Brightness { get; set; }

    /// <inheritdoc cref="StateResponse.Transition"/>
    [JsonPropertyName("transition")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ushort? Transition { get; set; }

    /// <summary>
    /// Sets the transition time for the current API call only (the <c>tt</c> field).
    /// One unit is 100ms. Range 0–65535.
    /// </summary>
    [JsonPropertyName("tt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ushort? TransientTransition { get; set; }

    /// <inheritdoc cref="StateResponse.PresetId"/>
    [JsonPropertyName("ps")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PresetSelector? PresetId { get; set; }

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
    public LiveDataOverride? LiveDataOverride { get; set; }

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

    /// <summary>
    /// Enter realtime/blank live mode for this call only (the <c>live</c> field, write-only).
    /// </summary>
    [JsonPropertyName("live")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Live { get; set; }

    /// <summary>
    /// Set the device clock to this Unix time in seconds (the <c>time</c> field, write-only).
    /// </summary>
    [JsonPropertyName("time")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? Time { get; set; }

    /// <summary>
    /// Load the ledmap with this id (0–9) (the <c>ledmap</c> field, write-only).
    /// </summary>
    [JsonPropertyName("ledmap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte? LedMap { get; set; }

    /// <summary>
    /// Remove the last custom palette (the <c>rmcpal</c> field, write-only).
    /// </summary>
    [JsonPropertyName("rmcpal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? RemoveLastCustomPalette { get; set; }

    /// <summary>
    /// Advance to the next preset in the active playlist (the <c>np</c> field, write-only).
    /// </summary>
    [JsonPropertyName("np")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? NextPreset { get; set; }

    /// <summary>
    /// Save the current state to this preset slot (the <c>psave</c> field, write-only).
    /// </summary>
    [JsonPropertyName("psave")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? SavePresetSlot { get; set; }

    /// <summary>
    /// Delete the preset in this slot (the <c>pdel</c> field, write-only).
    /// </summary>
    [JsonPropertyName("pdel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? DeletePresetSlot { get; set; }

    /// <summary>
    /// Name for a preset being saved (the <c>n</c> field, write-only).
    /// </summary>
    [JsonPropertyName("n")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PresetName { get; set; }

    /// <summary>
    /// Quick-load label for a preset being saved (the <c>ql</c> field, write-only).
    /// </summary>
    [JsonPropertyName("ql")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? QuickLabel { get; set; }

    /// <summary>
    /// Save segment bounds with the preset (the <c>sb</c> field, write-only).
    /// </summary>
    [JsonPropertyName("sb")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SaveSegmentBounds { get; set; }

    /// <summary>
    /// Include brightness in the saved preset (the <c>ib</c> field, write-only).
    /// </summary>
    [JsonPropertyName("ib")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IncludeBrightness { get; set; }

    /// <summary>
    /// Save which segments are selected with the preset (the <c>sc</c> field, write-only).
    /// </summary>
    [JsonPropertyName("sc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SaveSelectedSegments { get; set; }

    /// <summary>
    /// Start a playlist (the <c>playlist</c> field, write-only).
    /// </summary>
    [JsonPropertyName("playlist")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PlaylistRequest? Playlist { get; set; }

    /// <summary>
    /// Reboot the device (the <c>rb</c> field, write-only).
    /// </summary>
    [JsonPropertyName("rb")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Reboot { get; set; }

    public static StateRequest From(StateResponse stateResponse)
    {
        return new StateRequest()
        {
            On = stateResponse.On,
            Brightness = stateResponse.Brightness,
            Transition = stateResponse.Transition,
            PresetId = stateResponse.PresetId is { } presetId ? PresetSelector.Id(presetId) : null,
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