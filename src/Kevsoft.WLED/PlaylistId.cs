namespace Kevsoft.WLED;

/// <summary>A WLED playlist id (1 to 250).</summary>
public readonly struct PlaylistId : IEquatable<PlaylistId>
{
    /// <summary>The smallest valid playlist id.</summary>
    public const int MinValue = 1;

    /// <summary>The largest valid playlist id.</summary>
    public const int MaxValue = 250;

    /// <summary>Creates a playlist id, validating that it is between 1 and 250.</summary>
    public PlaylistId(int value)
    {
        if (value < MinValue || value > MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Playlist id must be between {MinValue} and {MaxValue}.");
        }

        Value = value;
    }

    /// <summary>The underlying id.</summary>
    public int Value { get; }

    /// <summary>Creates a playlist id from an <see cref="int"/>.</summary>
    public static PlaylistId From(int value) => new(value);

    public static implicit operator int(PlaylistId id) => id.Value;

    public bool Equals(PlaylistId other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is PlaylistId other && Equals(other);

    public override int GetHashCode() => Value;

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    public static bool operator ==(PlaylistId left, PlaylistId right) => left.Equals(right);

    public static bool operator !=(PlaylistId left, PlaylistId right) => !left.Equals(right);
}
