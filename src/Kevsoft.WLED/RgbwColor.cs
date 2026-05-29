namespace Kevsoft.WLED;

/// <summary>
/// A 32-bit RGBW color (RGB plus a dedicated white channel).
/// </summary>
public readonly record struct RgbwColor(byte R, byte G, byte B, byte W)
{
    /// <summary>
    /// Parses an 8 digit hex color such as <c>"FFAA0040"</c> or <c>"#FFAA0040"</c>.
    /// </summary>
    public static RgbwColor FromHex(string hex)
    {
        var (r, g, b, w) = HexColor.Parse(hex);
        if (!w.HasValue)
        {
            throw new FormatException($"'{hex}' is an RGB hex value; use {nameof(RgbColor)}.{nameof(RgbColor.FromHex)} instead.");
        }

        return new RgbwColor(r, g, b, w.Value);
    }

    /// <summary>
    /// Returns the color as an upper-case 8 digit hex string (no leading <c>#</c>).
    /// </summary>
    public string ToHex() => HexColor.Format(R, G, B, W);

    public static implicit operator Color(RgbwColor color) => Color.Rgbw(color.R, color.G, color.B, color.W);
}
