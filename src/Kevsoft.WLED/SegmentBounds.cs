namespace Kevsoft.WLED;

/// <summary>The bounds of a segment on a 1D strip (<c>start</c> inclusive, <c>stop</c> exclusive).</summary>
public readonly struct SegmentBounds : IEquatable<SegmentBounds>
{
    /// <summary>Creates segment bounds, validating that both values are zero or greater.</summary>
    public SegmentBounds(int start, int stop)
    {
        if (start < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(start), start, "Segment start must be zero or greater.");
        }

        if (stop < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stop), stop, "Segment stop must be zero or greater.");
        }

        Start = start;
        Stop = stop;
    }

    /// <summary>The first LED in the segment (inclusive).</summary>
    public int Start { get; }

    /// <summary>The LED after the last LED in the segment (exclusive).</summary>
    public int Stop { get; }

    /// <summary>The number of LEDs covered by these bounds.</summary>
    public int Length => Stop > Start ? Stop - Start : 0;

    /// <summary>Creates segment bounds from a start and stop.</summary>
    public static SegmentBounds From(int start, int stop) => new(start, stop);

    public bool Equals(SegmentBounds other) => Start == other.Start && Stop == other.Stop;

    public override bool Equals(object? obj) => obj is SegmentBounds other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            return (Start * 397) ^ Stop;
        }
    }

    public override string ToString() => $"[{Start}, {Stop})";

    public static bool operator ==(SegmentBounds left, SegmentBounds right) => left.Equals(right);

    public static bool operator !=(SegmentBounds left, SegmentBounds right) => !left.Equals(right);
}
