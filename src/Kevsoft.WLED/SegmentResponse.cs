namespace Kevsoft.WLED;

public sealed class SegmentResponse
{
    /// <summary>
    /// Zero-indexed ID of the segment. May be omitted, in that case the ID will be inferred from the order of the segment objects in the seg array. As such, not included in state response.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// LED the segment starts at.
    /// </summary>
    [JsonPropertyName("start")]
    public int Start { get; set; }

    /// <summary>
    /// LED the segment stops at, not included in range. If stop is set to a lower or equal value than start (setting to 0 is recommended), the segment is invalidated and deleted.
    /// </summary>
    [JsonPropertyName("stop")]
    public int Stop { get; set; }

    /// <summary>
    /// Length of the segment (stop - start). stop has preference, so if it is included, len is ignored.
    /// </summary>
    [JsonPropertyName("len")]
    public int Length { get; set; }

    /// <summary>
    /// Grouping (how many consecutive LEDs of the same segment will be grouped to the same color)
    /// </summary>
    [JsonPropertyName("grp")]
    public int Group { get; set; }

    /// <summary>
    /// Spacing (how many LEDs are turned off and skipped between each group)
    /// </summary>
    [JsonPropertyName("spc")]
    public int Spacing { get; set; }

    /// <summary>
    /// Offset (how many LEDs to rotate the virtual start of the segments, available since 0.13.0)
    /// </summary>
    [JsonPropertyName("of")]
    public int Offset { get; set; }

    /// <summary>
    /// The primary, secondary (background) and tertiary colors of the segment.
    /// </summary>
    [JsonPropertyName("col")]
    public SegmentColors Colors { get; set; } = null!;

    /// <summary>
    /// ID of the effect.
    /// </summary>
    [JsonPropertyName("fx")]
    public int EffectId { get; set; }

    /// <summary>
    /// Relative effect speed (0–255).
    /// </summary>
    [JsonPropertyName("sx")]
    public byte EffectSpeed { get; set; }

    /// <summary>
    /// Effect intensity (0–255).
    /// </summary>
    [JsonPropertyName("ix")]
    public byte EffectIntensity { get; set; }

    /// <summary>
    /// ID of the color palette
    /// </summary>
    [JsonPropertyName("pal")]
    public int ColorPaletteId { get; set; }

    /// <summary>
    /// true if the segment is selected. Selected segments will have their state (color/FX) updated by APIs that don't support segments.
    /// </summary>
    [JsonPropertyName("sel")]
    public bool Selected { get; set; }

    /// <summary>
    /// Flips the segment, causing animations to change direction.
    /// </summary>
    [JsonPropertyName("rev")]
    public bool Reverse { get; set; }

    /// <summary>
    /// freezes/unfreezes the current effect
    /// </summary>
    [JsonPropertyName("frz")]
    public bool Freeze { get; set; }

    /// <summary>
    /// Turns on and off the individual segment. (available since 0.10.0)
    /// </summary>
    [JsonPropertyName("on")]
    public bool SegmentState { get; set; }

    /// <summary>
    /// Sets the individual segment brightness (0–255, available since 0.10.0)
    /// </summary>
    [JsonPropertyName("bri")]
    public byte Brightness { get; set; }

    /// <summary>
    /// Mirrors the segment (available since 0.10.2)
    /// </summary>
    [JsonPropertyName("mi")]
    public bool Mirror { get; set; }

    /// <summary>
    /// Segment name.
    /// </summary>
    [JsonPropertyName("n")]
    public string? Name { get; set; }

    /// <summary>
    /// Color temperature of the segment.
    /// </summary>
    [JsonPropertyName("cct")]
    public ColorTemperature Cct { get; set; }

    /// <summary>
    /// Custom slider 1 (effect dependent, 0–255).
    /// </summary>
    [JsonPropertyName("c1")]
    public byte CustomSlider1 { get; set; }

    /// <summary>
    /// Custom slider 2 (effect dependent, 0–255).
    /// </summary>
    [JsonPropertyName("c2")]
    public byte CustomSlider2 { get; set; }

    /// <summary>
    /// Custom slider 3 (effect dependent, 0–31).
    /// </summary>
    [JsonPropertyName("c3")]
    public byte CustomSlider3 { get; set; }

    /// <summary>
    /// Effect option 1 (effect dependent checkbox).
    /// </summary>
    [JsonPropertyName("o1")]
    public bool Option1 { get; set; }

    /// <summary>
    /// Effect option 2 (effect dependent checkbox).
    /// </summary>
    [JsonPropertyName("o2")]
    public bool Option2 { get; set; }

    /// <summary>
    /// Effect option 3 (effect dependent checkbox).
    /// </summary>
    [JsonPropertyName("o3")]
    public bool Option3 { get; set; }

    /// <summary>
    /// How a 1D effect is expanded onto a 2D matrix.
    /// </summary>
    [JsonPropertyName("m12")]
    public Expand1D Expand1D { get; set; }

    /// <summary>
    /// The sound simulation type used for audio-reactive effects.
    /// </summary>
    [JsonPropertyName("si")]
    public SoundSimulation SoundSimulation { get; set; }

    /// <summary>
    /// Group/set id (0–3).
    /// </summary>
    [JsonPropertyName("set")]
    public byte Set { get; set; }

    /// <summary>
    /// Source segment this segment was cloned from, or <c>null</c> if not a clone.
    /// </summary>
    [JsonPropertyName("cln")]
    [JsonConverter(typeof(NullableSentinelInt32JsonConverter))]
    public int? Clones { get; set; }

    // 2D matrix only. These are ignored on 1D strips.

    /// <summary>2D matrix: LED row the segment starts at.</summary>
    [JsonPropertyName("startY")]
    public int? StartY { get; set; }

    /// <summary>2D matrix: LED row the segment stops at (exclusive).</summary>
    [JsonPropertyName("stopY")]
    public int? StopY { get; set; }

    /// <summary>2D matrix: flip the segment vertically.</summary>
    [JsonPropertyName("rY")]
    public bool ReverseY { get; set; }

    /// <summary>2D matrix: mirror the segment vertically.</summary>
    [JsonPropertyName("mY")]
    public bool MirrorY { get; set; }

    /// <summary>2D matrix: transpose (swap X and Y).</summary>
    [JsonPropertyName("tp")]
    public bool Transpose { get; set; }
}