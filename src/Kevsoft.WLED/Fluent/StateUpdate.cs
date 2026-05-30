namespace Kevsoft.WLED;

/// <summary>
/// Fluent builder for a sparse state update. Only the properties you set are sent to the
/// device, so an update never accidentally overwrites unrelated state.
/// </summary>
public sealed class StateUpdate
{
    private readonly StateRequest _request = new();
    private readonly List<SegmentRequest> _segments = new();
    private SegmentUpdate? _selectedSegment;
    private SegmentMode _segmentMode = SegmentMode.None;

    private enum SegmentMode
    {
        None,
        Selected,
        Explicit,
    }

    /// <summary>Turn the light on.</summary>
    public StateUpdate TurnOn() => On(Toggleable.On);

    /// <summary>Turn the light off.</summary>
    public StateUpdate TurnOff() => On(Toggleable.Off);

    /// <summary>Toggle the light on/off.</summary>
    public StateUpdate Toggle() => On(Toggleable.Toggle);

    /// <summary>Set the on/off state explicitly.</summary>
    public StateUpdate On(Toggleable value)
    {
        _request.On = value;
        return this;
    }

    /// <summary>Set, nudge or wrap the master brightness (0–255).</summary>
    public StateUpdate Brightness(ByteAdjust brightness)
    {
        _request.Brightness = brightness;
        return this;
    }

    /// <summary>Set the crossfade transition duration (rounded to 100ms units, max ~109 minutes).</summary>
    public StateUpdate Transition(TimeSpan duration)
    {
        _request.Transition = ToTransitionUnits(duration);
        return this;
    }

    /// <summary>Set the transition duration for this call only.</summary>
    public StateUpdate TransitionOnce(TimeSpan duration)
    {
        _request.TransientTransition = ToTransitionUnits(duration);
        return this;
    }

    /// <summary>Apply a preset.</summary>
    public StateUpdate Preset(PresetSelector preset)
    {
        _request.PresetId = preset;
        return this;
    }

    /// <summary>Set the main segment id.</summary>
    public StateUpdate MainSegment(int id)
    {
        _request.MainSegment = id;
        return this;
    }

    /// <summary>Set how live/realtime data overrides the normal output.</summary>
    public StateUpdate LiveDataOverride(LiveDataOverride mode)
    {
        _request.LiveDataOverride = mode;
        return this;
    }

    /// <summary>Enter (or leave) realtime/blank live mode for this call only.</summary>
    public StateUpdate EnterLiveMode(bool enabled = true)
    {
        _request.Live = enabled;
        return this;
    }

    /// <summary>Set the device clock.</summary>
    public StateUpdate SetTime(DateTimeOffset time)
    {
        _request.Time = time.ToUnixTimeSeconds();
        return this;
    }

    /// <summary>Load the ledmap with the given id (0–9).</summary>
    public StateUpdate LoadLedMap(byte id)
    {
        if (id > 9)
        {
            throw new ArgumentOutOfRangeException(nameof(id), id, "Ledmap id must be between 0 and 9.");
        }

        _request.LedMap = id;
        return this;
    }

    /// <summary>Load the ledmap with the given id (0–9).</summary>
    public StateUpdate LoadLedMap(LedMapId id)
    {
        _request.LedMap = (byte)id.Value;
        return this;
    }

    /// <summary>Remove the last custom palette.</summary>
    public StateUpdate RemoveLastCustomPalette()
    {
        _request.RemoveLastCustomPalette = true;
        return this;
    }

    /// <summary>Advance to the next preset in the active playlist.</summary>
    public StateUpdate NextPreset()
    {
        _request.NextPreset = true;
        return this;
    }

    /// <summary>Configure the segment with the given id, patching only the properties you set.</summary>
    /// <remarks>
    /// This targets the segment by id using the array form (<c>"seg":[{"id":N,...}]</c>). It cannot be
    /// combined with <see cref="SelectedSegments"/> in the same update.
    /// </remarks>
    public StateUpdate Segment(int id, Action<SegmentUpdate> configure)
    {
        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        if (_segmentMode == SegmentMode.Selected)
        {
            throw new InvalidOperationException(
                "Cannot mix Segment(id, ...) and SelectedSegments(...) in the same update: WLED's 'seg' field " +
                "is either the explicit-id array form or the selected-segment object form, not both.");
        }

        _segmentMode = SegmentMode.Explicit;
        var segment = new SegmentUpdate(id);
        configure(segment);
        _segments.Add(segment.Build());
        return this;
    }

    /// <summary>
    /// Configure the currently selected segments, patching only the properties you set.
    /// </summary>
    /// <remarks>
    /// This targets the selected segments using the object form (<c>"seg":{...}</c>) without naming an id.
    /// Calling it more than once configures the same selected-segment update cumulatively. It cannot be
    /// combined with <see cref="Segment"/> in the same update.
    /// </remarks>
    public StateUpdate SelectedSegments(Action<SegmentUpdate> configure)
    {
        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        if (_segmentMode == SegmentMode.Explicit)
        {
            throw new InvalidOperationException(
                "Cannot mix SelectedSegments(...) and Segment(id, ...) in the same update: WLED's 'seg' field " +
                "is either the selected-segment object form or the explicit-id array form, not both.");
        }

        _segmentMode = SegmentMode.Selected;
        _selectedSegment ??= new SegmentUpdate();
        configure(_selectedSegment);
        return this;
    }

    internal StateRequest Build()
    {
        switch (_segmentMode)
        {
            case SegmentMode.Explicit:
                _request.Segments = SegmentPayload.List(_segments.ToArray());
                break;
            case SegmentMode.Selected:
                _request.Segments = SegmentPayload.Selected(_selectedSegment!.Build());
                break;
        }

        return _request;
    }

    private static ushort ToTransitionUnits(TimeSpan duration)
    {
        var units = Math.Round(duration.TotalMilliseconds / 100.0, MidpointRounding.AwayFromZero);
        if (units < 0)
        {
            units = 0;
        }
        else if (units > ushort.MaxValue)
        {
            units = ushort.MaxValue;
        }

        return (ushort)units;
    }
}
