namespace Kevsoft.WLED;

/// <summary>The bounds of a segment on a 2D matrix (X and Y, each <c>start</c> inclusive, <c>stop</c> exclusive).</summary>
public readonly struct MatrixBounds : IEquatable<MatrixBounds>
{
    /// <summary>Creates matrix bounds, validating that every value is zero or greater.</summary>
    public MatrixBounds(int startX, int stopX, int startY, int stopY)
    {
        if (startX < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(startX), startX, "Matrix startX must be zero or greater.");
        }

        if (stopX < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stopX), stopX, "Matrix stopX must be zero or greater.");
        }

        if (startY < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(startY), startY, "Matrix startY must be zero or greater.");
        }

        if (stopY < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stopY), stopY, "Matrix stopY must be zero or greater.");
        }

        StartX = startX;
        StopX = stopX;
        StartY = startY;
        StopY = stopY;
    }

    /// <summary>The first column in the segment (inclusive).</summary>
    public int StartX { get; }

    /// <summary>The column after the last column in the segment (exclusive).</summary>
    public int StopX { get; }

    /// <summary>The first row in the segment (inclusive).</summary>
    public int StartY { get; }

    /// <summary>The row after the last row in the segment (exclusive).</summary>
    public int StopY { get; }

    /// <summary>Creates matrix bounds from X and Y start/stop values.</summary>
    public static MatrixBounds From(int startX, int stopX, int startY, int stopY) => new(startX, stopX, startY, stopY);

    public bool Equals(MatrixBounds other) =>
        StartX == other.StartX && StopX == other.StopX && StartY == other.StartY && StopY == other.StopY;

    public override bool Equals(object? obj) => obj is MatrixBounds other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = StartX;
            hash = (hash * 397) ^ StopX;
            hash = (hash * 397) ^ StartY;
            hash = (hash * 397) ^ StopY;
            return hash;
        }
    }

    public override string ToString() => $"X[{StartX}, {StopX}) Y[{StartY}, {StopY})";

    public static bool operator ==(MatrixBounds left, MatrixBounds right) => left.Equals(right);

    public static bool operator !=(MatrixBounds left, MatrixBounds right) => !left.Equals(right);
}
