namespace Kevsoft.WLED;

/// <summary>
/// A single WLED color slot, which is either an RGB or an RGBW color.
/// </summary>
/// <remarks>
/// WLED represents a color as an array of 3 (RGB) or 4 (RGBW) bytes, or as a hex string.
/// This type unifies those representations so an invalid color cannot be constructed.
/// </remarks>
[JsonConverter(typeof(ColorJsonConverter))]
public readonly struct Color : IEquatable<Color>
{
    private Color(byte r, byte g, byte b, byte? w)
    {
        R = r;
        G = g;
        B = b;
        W = w;
    }

    /// <summary>Red channel.</summary>
    public byte R { get; }

    /// <summary>Green channel.</summary>
    public byte G { get; }

    /// <summary>Blue channel.</summary>
    public byte B { get; }

    /// <summary>White channel, or <c>null</c> for an RGB color.</summary>
    public byte? W { get; }

    /// <summary><c>true</c> if this color has a dedicated white channel.</summary>
    public bool IsRgbw => W.HasValue;

    /// <summary>Creates an RGB color.</summary>
    public static Color Rgb(byte r, byte g, byte b) => new(r, g, b, null);

    /// <summary>Creates an RGBW color.</summary>
    public static Color Rgbw(byte r, byte g, byte b, byte w) => new(r, g, b, w);

    /// <summary>Parses a 6 (RGB) or 8 (RGBW) digit hex color, with an optional leading <c>#</c>.</summary>
    public static Color FromHex(string hex)
    {
        var (r, g, b, w) = HexColor.Parse(hex);
        return new Color(r, g, b, w);
    }

    /// <summary>Returns the color as an upper-case hex string (6 or 8 digits, no leading <c>#</c>).</summary>
    public string ToHex() => HexColor.Format(R, G, B, W);

    /// <summary>The color components as a 3 or 4 element byte array, matching the WLED wire format.</summary>
    public byte[] ToBytes() => IsRgbw ? new[] { R, G, B, W!.Value } : new[] { R, G, B };

    public bool Equals(Color other) => R == other.R && G == other.G && B == other.B && W == other.W;

    public override bool Equals(object? obj) => obj is Color other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = (hash * 31) + R;
            hash = (hash * 31) + G;
            hash = (hash * 31) + B;
            hash = (hash * 31) + (W ?? -1);
            return hash;
        }
    }

    public override string ToString() => ToHex();

    public static bool operator ==(Color left, Color right) => left.Equals(right);

    public static bool operator !=(Color left, Color right) => !left.Equals(right);
}
