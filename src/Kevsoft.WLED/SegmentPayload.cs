namespace Kevsoft.WLED;

/// <summary>
/// The <c>seg</c> field of a state request, which WLED accepts as either a single object or
/// an array of objects.
/// </summary>
/// <remarks>
/// Use the <b>object</b> form (<see cref="Selected"/>) to target the currently <i>selected</i>
/// segments without naming an id. Use the <b>array</b> form (<see cref="List"/>) to target
/// specific segments by id. This distinction matters: WLED infers <c>id:0</c> for an array
/// entry that omits its id, so the object form is the only way to address "the selected
/// segments".
/// </remarks>
[JsonConverter(typeof(SegmentPayloadJsonConverter))]
public sealed class SegmentPayload
{
    private SegmentPayload(SegmentRequest? single, SegmentRequest[]? many)
    {
        Single = single;
        Many = many;
    }

    /// <summary>The single-segment (object) form, or <c>null</c> when this is the array form.</summary>
    internal SegmentRequest? Single { get; }

    /// <summary>The multi-segment (array) form, or <c>null</c> when this is the object form.</summary>
    internal SegmentRequest[]? Many { get; }

    /// <summary>
    /// Target the currently selected segments using the object form (<c>"seg":{...}</c>).
    /// </summary>
    public static SegmentPayload Selected(SegmentRequest segment)
        => new(segment ?? throw new ArgumentNullException(nameof(segment)), null);

    /// <summary>
    /// Target specific segments by id using the array form (<c>"seg":[{...}]</c>).
    /// </summary>
    public static SegmentPayload List(params SegmentRequest[] segments)
        => new(null, segments ?? throw new ArgumentNullException(nameof(segments)));

    /// <summary>
    /// Implicitly wraps a single segment. A segment with no <see cref="SegmentRequest.Id"/> uses
    /// the object form (selected segments); one with an id uses the array form (id-targeted), so
    /// the WLED <c>id:0</c> inference can never silently mis-target an explicitly-id'd segment.
    /// </summary>
    public static implicit operator SegmentPayload(SegmentRequest segment)
        => segment is null
            ? throw new ArgumentNullException(nameof(segment))
            : segment.Id is null ? Selected(segment) : List(segment);

    public static implicit operator SegmentPayload(SegmentRequest[] segments) => List(segments);
}
