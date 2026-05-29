namespace Kevsoft.WLED;

/// <summary>
/// Fluent builder for a sparse state update. Only the properties you set are sent to the
/// device, so an update never accidentally overwrites unrelated state.
/// </summary>
public sealed class StateUpdate
{
    private readonly StateRequest _request = new();
    private readonly List<SegmentRequest> _segments = new();

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

    /// <summary>Set the crossfade transition duration (rounded to 100ms units, max 25.5s).</summary>
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

    /// <summary>Configure the segment with the given id, patching only the properties you set.</summary>
    public StateUpdate Segment(int id, Action<SegmentUpdate> configure)
    {
        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var segment = new SegmentUpdate(id);
        configure(segment);
        _segments.Add(segment.Build());
        return this;
    }

    internal StateRequest Build()
    {
        if (_segments.Count > 0)
        {
            _request.Segments = _segments.ToArray();
        }

        return _request;
    }

    private static byte ToTransitionUnits(TimeSpan duration)
    {
        var units = Math.Round(duration.TotalMilliseconds / 100.0, MidpointRounding.AwayFromZero);
        if (units < 0)
        {
            units = 0;
        }
        else if (units > byte.MaxValue)
        {
            units = byte.MaxValue;
        }

        return (byte)units;
    }
}
