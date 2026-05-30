namespace Kevsoft.WLED;

public sealed class SegmentRequest
{
    /// <inheritdoc cref="SegmentResponse.Id"/>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Id { get; set; }

    /// <inheritdoc cref="SegmentResponse.Start"/>
    [JsonPropertyName("start")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Start { get; set; }

    /// <inheritdoc cref="SegmentResponse.Stop"/>
    [JsonPropertyName("stop")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Stop { get; set; }

    /// <inheritdoc cref="SegmentResponse.Length"/>
    [JsonPropertyName("len")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Length { get; set; }

    /// <inheritdoc cref="SegmentResponse.Group"/>
    [JsonPropertyName("grp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Group { get; set; }

    /// <inheritdoc cref="SegmentResponse.Spacing"/>
    [JsonPropertyName("spc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Spacing { get; set; }

    /// <inheritdoc cref="SegmentResponse.Offset"/>
    [JsonPropertyName("of")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Offset { get; set; }

    /// <inheritdoc cref="SegmentResponse.Colors"/>
    [JsonPropertyName("col")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SegmentColors? Colors { get; set; }

    /// <inheritdoc cref="SegmentResponse.EffectId"/>
    [JsonPropertyName("fx")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Selector? EffectId { get; set; }

    /// <inheritdoc cref="SegmentResponse.EffectSpeed"/>
    [JsonPropertyName("sx")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ByteAdjust? EffectSpeed { get; set; }

    /// <inheritdoc cref="SegmentResponse.EffectIntensity"/>
    [JsonPropertyName("ix")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ByteAdjust? EffectIntensity { get; set; }

    /// <inheritdoc cref="SegmentResponse.ColorPaletteId"/>
    [JsonPropertyName("pal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Selector? ColorPaletteId { get; set; }

    /// <inheritdoc cref="SegmentResponse.Selected"/>
    [JsonPropertyName("sel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Selected { get; set; }

    /// <inheritdoc cref="SegmentResponse.Reverse"/>
    [JsonPropertyName("rev")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Reverse { get; set; }

    /// <inheritdoc cref="SegmentResponse.Freeze"/>
    [JsonPropertyName("frz")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Toggleable? Freeze { get; set; }

    /// <inheritdoc cref="SegmentResponse.SegmentState"/>
    [JsonPropertyName("on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Toggleable? SegmentState { get; set; }

    /// <inheritdoc cref="SegmentResponse.Brightness"/>
    [JsonPropertyName("bri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ByteAdjust? Brightness { get; set; }

    /// <inheritdoc cref="SegmentResponse.Mirror"/>
    [JsonPropertyName("mi")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Mirror { get; set; }

    /// <inheritdoc cref="SegmentResponse.Name"/>
    [JsonPropertyName("n")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    /// <inheritdoc cref="SegmentResponse.Cct"/>
    [JsonPropertyName("cct")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ColorTemperature? Cct { get; set; }

    /// <inheritdoc cref="SegmentResponse.CustomSlider1"/>
    [JsonPropertyName("c1")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte? CustomSlider1 { get; set; }

    /// <inheritdoc cref="SegmentResponse.CustomSlider2"/>
    [JsonPropertyName("c2")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte? CustomSlider2 { get; set; }

    /// <inheritdoc cref="SegmentResponse.CustomSlider3"/>
    [JsonPropertyName("c3")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte? CustomSlider3 { get; set; }

    /// <inheritdoc cref="SegmentResponse.Option1"/>
    [JsonPropertyName("o1")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Option1 { get; set; }

    /// <inheritdoc cref="SegmentResponse.Option2"/>
    [JsonPropertyName("o2")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Option2 { get; set; }

    /// <inheritdoc cref="SegmentResponse.Option3"/>
    [JsonPropertyName("o3")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Option3 { get; set; }

    /// <inheritdoc cref="SegmentResponse.Expand1D"/>
    [JsonPropertyName("m12")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Expand1D? Expand1D { get; set; }

    /// <inheritdoc cref="SegmentResponse.SoundSimulation"/>
    [JsonPropertyName("si")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SoundSimulation? SoundSimulation { get; set; }

    /// <inheritdoc cref="SegmentResponse.Set"/>
    [JsonPropertyName("set")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte? Set { get; set; }

    /// <inheritdoc cref="SegmentResponse.StartY"/>
    [JsonPropertyName("startY")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? StartY { get; set; }

    /// <inheritdoc cref="SegmentResponse.StopY"/>
    [JsonPropertyName("stopY")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? StopY { get; set; }

    /// <inheritdoc cref="SegmentResponse.ReverseY"/>
    [JsonPropertyName("rY")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? ReverseY { get; set; }

    /// <inheritdoc cref="SegmentResponse.MirrorY"/>
    [JsonPropertyName("mY")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? MirrorY { get; set; }

    /// <inheritdoc cref="SegmentResponse.Transpose"/>
    [JsonPropertyName("tp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Transpose { get; set; }

    /// <summary>Write-only: reset all effect parameters (speed, intensity, custom sliders, options) to the effect defaults.</summary>
    [JsonPropertyName("fxdef")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? LoadEffectDefaults { get; set; }

    /// <summary>Write-only: repeat the segment's settings to fill the whole strip.</summary>
    [JsonPropertyName("rpt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? RepeatToFill { get; set; }

    /// <summary>Write-only: individual LED assignments. Build through <see cref="IndividualLedBuilder"/>.</summary>
    [JsonPropertyName("i")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IndividualLedData? IndividualLeds { get; set; }

    public static SegmentRequest From(SegmentResponse segmentResponse)
    {
        return new SegmentRequest
        {
            Id = segmentResponse.Id,
            Start = segmentResponse.Start,
            Stop = segmentResponse.Stop,
            Length = segmentResponse.Length,
            Group = segmentResponse.Group,
            Spacing = segmentResponse.Spacing,
            Offset = segmentResponse.Offset,
            Colors = segmentResponse.Colors,
            EffectId = segmentResponse.EffectId,
            EffectSpeed = segmentResponse.EffectSpeed,
            EffectIntensity = segmentResponse.EffectIntensity,
            ColorPaletteId = segmentResponse.ColorPaletteId,
            Selected = segmentResponse.Selected,
            Reverse = segmentResponse.Reverse,
            Freeze = segmentResponse.Freeze,
            SegmentState = segmentResponse.SegmentState,
            Brightness = segmentResponse.Brightness,
            Mirror = segmentResponse.Mirror,
            Name = segmentResponse.Name,
            Cct = segmentResponse.Cct,
            CustomSlider1 = segmentResponse.CustomSlider1,
            CustomSlider2 = segmentResponse.CustomSlider2,
            CustomSlider3 = segmentResponse.CustomSlider3,
            Option1 = segmentResponse.Option1,
            Option2 = segmentResponse.Option2,
            Option3 = segmentResponse.Option3,
            Expand1D = segmentResponse.Expand1D,
            SoundSimulation = segmentResponse.SoundSimulation,
            Set = segmentResponse.Set,
            StartY = segmentResponse.StartY,
            StopY = segmentResponse.StopY,
            ReverseY = segmentResponse.ReverseY,
            MirrorY = segmentResponse.MirrorY,
            Transpose = segmentResponse.Transpose
        };
    }

    public static implicit operator SegmentRequest(SegmentResponse rhs) => From(rhs);
}