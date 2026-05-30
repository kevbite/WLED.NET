namespace Kevsoft.WLED;

/// <summary>A savable WLED preset id (1 to 250).</summary>
public readonly struct PresetId : IEquatable<PresetId>
{
    /// <summary>The smallest valid preset id.</summary>
    public const int MinValue = 1;

    /// <summary>The largest valid preset id.</summary>
    public const int MaxValue = 250;

    /// <summary>Creates a preset id, validating that it is between 1 and 250.</summary>
    public PresetId(int value)
    {
        if (value < MinValue || value > MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Preset id must be between {MinValue} and {MaxValue}.");
        }

        Value = value;
    }

    /// <summary>The underlying id.</summary>
    public int Value { get; }

    /// <summary>Creates a preset id from an <see cref="int"/>.</summary>
    public static PresetId From(int value) => new(value);

    public static implicit operator int(PresetId id) => id.Value;

    public bool Equals(PresetId other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is PresetId other && Equals(other);

    public override int GetHashCode() => Value;

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    public static bool operator ==(PresetId left, PresetId right) => left.Equals(right);

    public static bool operator !=(PresetId left, PresetId right) => !left.Equals(right);
}
