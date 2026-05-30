namespace Kevsoft.WLED;

/// <summary>
/// Fluent builder for a sparse segment update. The segment <c>id</c> is always sent so the
/// device patches the matching segment rather than replacing all segments.
/// </summary>
public sealed class SegmentUpdate
{
    private readonly SegmentRequest _request;

    internal SegmentUpdate(int id) => _request = new SegmentRequest { Id = id };

    /// <summary>Creates an id-less segment update targeting the selected segments (object form).</summary>
    internal SegmentUpdate() => _request = new SegmentRequest();

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

    /// <summary>Select the effect from a catalog entry. Throws if the entry is a reserved placeholder.</summary>
    public SegmentUpdate Effect(EffectCatalogEntry effect)
    {
        if (effect is null)
        {
            throw new ArgumentNullException(nameof(effect));
        }

        if (effect.IsReserved)
        {
            throw new ArgumentException($"Effect '{effect.Name}' (id {effect.Id}) is a reserved placeholder and cannot be selected.", nameof(effect));
        }

        return Effect(Selector.Id(effect.Id));
    }

    /// <summary>Select the effect described by the given metadata.</summary>
    public SegmentUpdate Effect(EffectMetadata effect)
    {
        if (effect is null)
        {
            throw new ArgumentNullException(nameof(effect));
        }

        return Effect(Selector.Id(effect.EffectId));
    }

    /// <summary>
    /// Apply the effect's recommended default values (speed, intensity and custom sliders) from its
    /// metadata. Only the controls the effect actually defines defaults for are set.
    /// </summary>
    public SegmentUpdate ApplyEffectDefaults(EffectMetadata effect)
    {
        if (effect is null)
        {
            throw new ArgumentNullException(nameof(effect));
        }

        foreach (var slider in effect.Defaults)
        {
            var value = slider.Value;
            switch (slider.Key)
            {
                case "sx":
                    Speed(ToByte(value));
                    break;
                case "ix":
                    Intensity(ToByte(value));
                    break;
                case "c1":
                    CustomSlider1(ToByte(value));
                    break;
                case "c2":
                    CustomSlider2(ToByte(value));
                    break;
                case "c3":
                    _request.CustomSlider3 = value < 0 ? (byte)0 : value > 31 ? (byte)31 : (byte)value;
                    break;
            }
        }

        return this;
    }

    private static byte ToByte(int value)
    {
        if (value < 0)
        {
            return 0;
        }

        return value > 255 ? (byte)255 : (byte)value;
    }

    /// <summary>Select the color palette by id, relative movement or at random.</summary>
    public SegmentUpdate Palette(Selector palette)
    {
        _request.ColorPaletteId = palette;
        return this;
    }

    /// <summary>Select the palette from a catalog entry. Throws if the entry is a reserved placeholder.</summary>
    public SegmentUpdate Palette(PaletteCatalogEntry palette)
    {
        if (palette is null)
        {
            throw new ArgumentNullException(nameof(palette));
        }

        if (palette.IsReserved)
        {
            throw new ArgumentException($"Palette '{palette.Name}' (id {palette.Id}) is a reserved placeholder and cannot be selected.", nameof(palette));
        }

        return Palette(Selector.Id(palette.Id));
    }

    /// <summary>Set the segment's color slots (primary, optional secondary and tertiary).</summary>
    public SegmentUpdate Color(SegmentColors colors)
    {
        _request.Colors = colors ?? throw new ArgumentNullException(nameof(colors));
        return this;
    }

    /// <summary>Set the segment's primary color.</summary>
    public SegmentUpdate Color(Color primary) => Color(new SegmentColors(primary));

    /// <summary>Set the segment's primary, secondary and optional tertiary colors.</summary>
    public SegmentUpdate Color(Color primary, Color secondary, Color? tertiary = null)
        => Color(new SegmentColors(primary, secondary, tertiary));

    /// <summary>Set, nudge or wrap the relative effect speed (0–255).</summary>
    public SegmentUpdate Speed(ByteAdjust speed)
    {
        _request.EffectSpeed = speed;
        return this;
    }

    /// <summary>Set, nudge or wrap the effect intensity (0–255).</summary>
    public SegmentUpdate Intensity(ByteAdjust intensity)
    {
        _request.EffectIntensity = intensity;
        return this;
    }

    /// <summary>Set, nudge or wrap the segment brightness (0–255).</summary>
    public SegmentUpdate Brightness(ByteAdjust brightness)
    {
        _request.Brightness = brightness;
        return this;
    }

    /// <summary>Set the segment name.</summary>
    public SegmentUpdate Name(string name)
    {
        _request.Name = name ?? throw new ArgumentNullException(nameof(name));
        return this;
    }

    /// <summary>Set the segment's color temperature.</summary>
    public SegmentUpdate Cct(ColorTemperature cct)
    {
        _request.Cct = cct;
        return this;
    }

    /// <summary>Set custom slider 1 (0–255, effect dependent).</summary>
    public SegmentUpdate CustomSlider1(byte value)
    {
        _request.CustomSlider1 = value;
        return this;
    }

    /// <summary>Set custom slider 2 (0–255, effect dependent).</summary>
    public SegmentUpdate CustomSlider2(byte value)
    {
        _request.CustomSlider2 = value;
        return this;
    }

    /// <summary>Set custom slider 3 (0–31, effect dependent).</summary>
    public SegmentUpdate CustomSlider3(byte value)
    {
        if (value > 31)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Custom slider 3 must be between 0 and 31.");
        }

        _request.CustomSlider3 = value;
        return this;
    }

    /// <summary>Set effect option 1 (effect dependent checkbox).</summary>
    public SegmentUpdate Option1(bool value)
    {
        _request.Option1 = value;
        return this;
    }

    /// <summary>Set effect option 2 (effect dependent checkbox).</summary>
    public SegmentUpdate Option2(bool value)
    {
        _request.Option2 = value;
        return this;
    }

    /// <summary>Set effect option 3 (effect dependent checkbox).</summary>
    public SegmentUpdate Option3(bool value)
    {
        _request.Option3 = value;
        return this;
    }

    /// <summary>Set how a 1D effect is expanded onto a 2D matrix.</summary>
    public SegmentUpdate Expand1D(Expand1D expand)
    {
        _request.Expand1D = expand;
        return this;
    }

    /// <summary>Set the sound simulation type for audio-reactive effects.</summary>
    public SegmentUpdate SoundSimulation(SoundSimulation simulation)
    {
        _request.SoundSimulation = simulation;
        return this;
    }

    /// <summary>Set the segment group/set id (0–3).</summary>
    public SegmentUpdate Set(byte value)
    {
        if (value > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Set must be between 0 and 3.");
        }

        _request.Set = value;
        return this;
    }

    /// <summary>Set the segment bounds on a 1D strip.</summary>
    public SegmentUpdate Range(int start, int stop)
    {
        _request.Start = start;
        _request.Stop = stop;
        return this;
    }

    /// <summary>Set the segment bounds on a 1D strip.</summary>
    public SegmentUpdate Range(SegmentBounds bounds) => Range(bounds.Start, bounds.Stop);

    /// <summary>Set the 2D matrix bounds of the segment.</summary>
    public SegmentUpdate Range2D(int startX, int stopX, int startY, int stopY)
    {
        _request.Start = startX;
        _request.Stop = stopX;
        _request.StartY = startY;
        _request.StopY = stopY;
        return this;
    }

    /// <summary>Set the 2D matrix bounds of the segment.</summary>
    public SegmentUpdate Range2D(MatrixBounds bounds) =>
        Range2D(bounds.StartX, bounds.StopX, bounds.StartY, bounds.StopY);

    /// <summary>Reverse the segment (flips animation direction).</summary>
    public SegmentUpdate Reverse(bool value = true)
    {
        _request.Reverse = value;
        return this;
    }

    /// <summary>Mirror the segment.</summary>
    public SegmentUpdate Mirror(bool value = true)
    {
        _request.Mirror = value;
        return this;
    }

    /// <summary>Reverse the segment vertically (2D matrix only).</summary>
    public SegmentUpdate ReverseY(bool value = true)
    {
        _request.ReverseY = value;
        return this;
    }

    /// <summary>Mirror the segment vertically (2D matrix only).</summary>
    public SegmentUpdate MirrorY(bool value = true)
    {
        _request.MirrorY = value;
        return this;
    }

    /// <summary>Transpose the segment, swapping X and Y (2D matrix only).</summary>
    public SegmentUpdate Transpose(bool value = true)
    {
        _request.Transpose = value;
        return this;
    }

    /// <summary>Select (or deselect) the segment.</summary>
    public SegmentUpdate Select(bool value = true)
    {
        _request.Selected = value;
        return this;
    }

    /// <summary>Set grouping and spacing for the segment.</summary>
    public SegmentUpdate Grouping(int group, int spacing)
    {
        _request.Group = group;
        _request.Spacing = spacing;
        return this;
    }

    /// <summary>Rotate the virtual start of the segment by the given offset.</summary>
    public SegmentUpdate Offset(int offset)
    {
        _request.Offset = offset;
        return this;
    }

    /// <summary>Reset all effect parameters to the effect defaults.</summary>
    public SegmentUpdate LoadEffectDefaults()
    {
        _request.LoadEffectDefaults = true;
        return this;
    }

    /// <summary>Repeat the segment's settings to fill the whole strip.</summary>
    public SegmentUpdate RepeatToFill()
    {
        _request.RepeatToFill = true;
        return this;
    }

    internal SegmentRequest Build() => _request;
}
