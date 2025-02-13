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
    public int[][]? Colors { get; set; }

    /// <inheritdoc cref="SegmentResponse.EffectId"/>
    [JsonPropertyName("fx")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EffectId { get; set; }

    /// <inheritdoc cref="SegmentResponse.EffectSpeed"/>
    [JsonPropertyName("sx")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EffectSpeed { get; set; }

    /// <inheritdoc cref="SegmentResponse.EffectIntensity"/>
    [JsonPropertyName("ix")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EffectIntensity { get; set; }

    /// <inheritdoc cref="SegmentResponse.ColorPaletteId"/>
    [JsonPropertyName("pal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ColorPaletteId { get; set; }

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
    public bool? Freeze { get; set; }

    /// <inheritdoc cref="SegmentResponse.SegmentState"/>
    [JsonPropertyName("on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SegmentState { get; set; }

    /// <inheritdoc cref="SegmentResponse.Brightness"/>
    [JsonPropertyName("bri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Brightness { get; set; }

    /// <inheritdoc cref="SegmentResponse.Mirror"/>
    [JsonPropertyName("mi")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Mirror { get; set; }

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
            Mirror = segmentResponse.Mirror
        };
    }

    public static implicit operator SegmentRequest(SegmentResponse rhs) => From(rhs);
}