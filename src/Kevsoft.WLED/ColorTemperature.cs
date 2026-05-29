namespace Kevsoft.WLED;

/// <summary>
/// A segment colour temperature. WLED accepts either a relative value (0–255) or an
/// absolute value in Kelvin (1900–10091), so this type makes the caller state which they
/// mean and guarantees the value is in range.
/// </summary>
[JsonConverter(typeof(ColorTemperatureJsonConverter))]
public readonly struct ColorTemperature : IEquatable<ColorTemperature>
{
    internal const int MinKelvin = 1900;
    internal const int MaxKelvin = 10091;

    private ColorTemperature(int value, bool isKelvin)
    {
        Value = value;
        IsKelvin = isKelvin;
    }

    /// <summary>The raw value, interpreted according to <see cref="IsKelvin"/>.</summary>
    public int Value { get; }

    /// <summary><c>true</c> if <see cref="Value"/> is in Kelvin; otherwise it is relative (0–255).</summary>
    public bool IsKelvin { get; }

    /// <summary>A relative colour temperature (0 = warmest, 255 = coldest).</summary>
    public static ColorTemperature Relative(byte value) => new(value, false);

    /// <summary>An absolute colour temperature in Kelvin (1900–10091).</summary>
    public static ColorTemperature Kelvin(int kelvin)
    {
        if (kelvin < MinKelvin || kelvin > MaxKelvin)
        {
            throw new ArgumentOutOfRangeException(nameof(kelvin), kelvin, $"Kelvin must be between {MinKelvin} and {MaxKelvin}.");
        }

        return new ColorTemperature(kelvin, true);
    }

    internal static ColorTemperature FromWire(int value) => new(value, value > byte.MaxValue);

    public bool Equals(ColorTemperature other) => Value == other.Value && IsKelvin == other.IsKelvin;

    public override bool Equals(object? obj) => obj is ColorTemperature other && Equals(other);

    public override int GetHashCode() => (Value * 397) ^ (IsKelvin ? 1 : 0);

    public override string ToString() => IsKelvin ? $"{Value}K" : Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    public static bool operator ==(ColorTemperature left, ColorTemperature right) => left.Equals(right);

    public static bool operator !=(ColorTemperature left, ColorTemperature right) => !left.Equals(right);
}
