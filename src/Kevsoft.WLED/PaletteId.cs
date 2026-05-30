namespace Kevsoft.WLED;

/// <summary>A WLED palette id (zero or greater).</summary>
public readonly struct PaletteId : IEquatable<PaletteId>
{
    /// <summary>Creates a palette id, validating that it is zero or greater.</summary>
    public PaletteId(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Palette id must be zero or greater.");
        }

        Value = value;
    }

    /// <summary>The underlying id.</summary>
    public int Value { get; }

    /// <summary>Creates a palette id from an <see cref="int"/>.</summary>
    public static PaletteId From(int value) => new(value);

    public static implicit operator PaletteId(int value) => new(value);

    public static implicit operator int(PaletteId id) => id.Value;

    public bool Equals(PaletteId other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is PaletteId other && Equals(other);

    public override int GetHashCode() => Value;

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    public static bool operator ==(PaletteId left, PaletteId right) => left.Equals(right);

    public static bool operator !=(PaletteId left, PaletteId right) => !left.Equals(right);
}
