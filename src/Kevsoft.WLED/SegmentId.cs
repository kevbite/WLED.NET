namespace Kevsoft.WLED;

/// <summary>A WLED segment id (zero or greater).</summary>
public readonly struct SegmentId : IEquatable<SegmentId>
{
    /// <summary>Creates a segment id, validating that it is zero or greater.</summary>
    public SegmentId(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Segment id must be zero or greater.");
        }

        Value = value;
    }

    /// <summary>The underlying id.</summary>
    public int Value { get; }

    /// <summary>Creates a segment id from an <see cref="int"/>.</summary>
    public static SegmentId From(int value) => new(value);

    public static implicit operator SegmentId(int value) => new(value);

    public static implicit operator int(SegmentId id) => id.Value;

    public bool Equals(SegmentId other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is SegmentId other && Equals(other);

    public override int GetHashCode() => Value;

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    public static bool operator ==(SegmentId left, SegmentId right) => left.Equals(right);

    public static bool operator !=(SegmentId left, SegmentId right) => !left.Equals(right);
}
