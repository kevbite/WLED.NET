namespace Kevsoft.WLED;

/// <summary>
/// A 24-bit RGB color.
/// </summary>
public readonly record struct RgbColor(byte R, byte G, byte B)
{
    /// <summary>
    /// Parses a hex color such as <c>"FFAA00"</c> or <c>"#FFAA00"</c>.
    /// </summary>
    public static RgbColor FromHex(string hex)
    {
        var (r, g, b, w) = HexColor.Parse(hex);
        if (w.HasValue)
        {
            throw new FormatException($"'{hex}' is an RGBW hex value; use {nameof(RgbwColor)}.{nameof(RgbwColor.FromHex)} instead.");
        }

        return new RgbColor(r, g, b);
    }

    /// <summary>
    /// Returns the color as an upper-case 6 digit hex string (no leading <c>#</c>).
    /// </summary>
    public string ToHex() => HexColor.Format(R, G, B, null);

    public static implicit operator Color(RgbColor color) => Color.Rgb(color.R, color.G, color.B);
}
