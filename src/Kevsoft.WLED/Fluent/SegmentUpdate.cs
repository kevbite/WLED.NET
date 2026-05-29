namespace Kevsoft.WLED;

/// <summary>
/// Fluent builder for a sparse segment update. The segment <c>id</c> is always sent so the
/// device patches the matching segment rather than replacing all segments.
/// </summary>
public sealed class SegmentUpdate
{
    private readonly SegmentRequest _request;

    internal SegmentUpdate(int id) => _request = new SegmentRequest { Id = id };

    /// <summary>Turn the segment on.</summary>
    public SegmentUpdate TurnOn() => On(Toggleable.On);

    /// <summary>Turn the segment off.</summary>
    public SegmentUpdate TurnOff() => On(Toggleable.Off);

    /// <summary>Toggle the segment on/off.</summary>
    public SegmentUpdate Toggle() => On(Toggleable.Toggle);

    /// <summary>Set the segment on/off state explicitly.</summary>
    public SegmentUpdate On(Toggleable value)
    {
        _request.SegmentState = value;
        return this;
    }

    /// <summary>Freeze the segment's effect.</summary>
    public SegmentUpdate Freeze() => Freeze(Toggleable.On);

    /// <summary>Set the segment's freeze state explicitly.</summary>
    public SegmentUpdate Freeze(Toggleable value)
    {
        _request.Freeze = value;
        return this;
    }

    /// <summary>Select the effect by id, relative movement or at random.</summary>
    public SegmentUpdate Effect(Selector effect)
    {
        _request.EffectId = effect;
        return this;
    }

    /// <summary>Select the color palette by id, relative movement or at random.</summary>
    public SegmentUpdate Palette(Selector palette)
    {
        _request.ColorPaletteId = palette;
        return this;
    }

    internal SegmentRequest Build() => _request;
}
