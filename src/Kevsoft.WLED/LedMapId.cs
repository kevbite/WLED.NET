namespace Kevsoft.WLED;

/// <summary>A WLED ledmap id (0 to 9), selecting <c>ledmap.json</c>..<c>ledmap9.json</c>.</summary>
public readonly struct LedMapId : IEquatable<LedMapId>
{
    /// <summary>The smallest valid ledmap id.</summary>
    public const int MinValue = 0;

    /// <summary>The largest valid ledmap id.</summary>
    public const int MaxValue = 9;

    /// <summary>Creates a ledmap id, validating that it is between 0 and 9.</summary>
    public LedMapId(int value)
    {
        if (value < MinValue || value > MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Ledmap id must be between {MinValue} and {MaxValue}.");
        }

        Value = value;
    }

    /// <summary>The underlying id.</summary>
    public int Value { get; }

    /// <summary>Creates a ledmap id from an <see cref="int"/>.</summary>
    public static LedMapId From(int value) => new(value);

    public static implicit operator int(LedMapId id) => id.Value;

    public bool Equals(LedMapId other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is LedMapId other && Equals(other);

    public override int GetHashCode() => Value;

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    public static bool operator ==(LedMapId left, LedMapId right) => left.Equals(right);

    public static bool operator !=(LedMapId left, LedMapId right) => !left.Equals(right);
}
