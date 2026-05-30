namespace Kevsoft.WLED;

/// <summary>A WLED effect id (zero or greater).</summary>
public readonly struct EffectId : IEquatable<EffectId>
{
    /// <summary>Creates an effect id, validating that it is zero or greater.</summary>
    public EffectId(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Effect id must be zero or greater.");
        }

        Value = value;
    }

    /// <summary>The underlying id.</summary>
    public int Value { get; }

    /// <summary>Creates an effect id from an <see cref="int"/>.</summary>
    public static EffectId From(int value) => new(value);

    public static implicit operator EffectId(int value) => new(value);

    public static implicit operator int(EffectId id) => id.Value;

    public bool Equals(EffectId other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is EffectId other && Equals(other);

    public override int GetHashCode() => Value;

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    public static bool operator ==(EffectId left, EffectId right) => left.Equals(right);

    public static bool operator !=(EffectId left, EffectId right) => !left.Equals(right);
}
