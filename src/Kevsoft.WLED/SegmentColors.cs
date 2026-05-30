namespace Kevsoft.WLED;

/// <summary>
/// The primary, secondary (background) and tertiary color slots of a segment.
/// </summary>
[JsonConverter(typeof(SegmentColorsJsonConverter))]
public sealed record SegmentColors(Color Primary, Color? Secondary = null, Color? Tertiary = null)
{
    /// <summary>The color slots in wire order, omitting trailing unset slots.</summary>
    public IReadOnlyList<Color> Slots
    {
        get
        {
            var slots = new List<Color> { Primary };
            if (Secondary.HasValue)
            {
                slots.Add(Secondary.Value);
                if (Tertiary.HasValue)
                {
                    slots.Add(Tertiary.Value);
                }
            }

            return slots;
        }
    }

    /// <summary>Builds a <see cref="SegmentColors"/> from up to three color slots.</summary>
    public static SegmentColors FromSlots(IReadOnlyList<Color> slots)
    {
        if (slots is null || slots.Count == 0)
        {
            throw new ArgumentException("At least a primary color is required.", nameof(slots));
        }

        return new SegmentColors(
            slots[0],
            slots.Count > 1 ? slots[1] : null,
            slots.Count > 2 ? slots[2] : null);
    }
}
