namespace Kevsoft.WLED;

internal static class HexColor
{
    public static (byte R, byte G, byte B, byte? W) Parse(string hex)
    {
        if (hex is null)
        {
            throw new ArgumentNullException(nameof(hex));
        }

        var value = hex.StartsWith("#", StringComparison.Ordinal) ? hex.Substring(1) : hex;

        if (value.Length != 6 && value.Length != 8)
        {
            throw new FormatException($"'{hex}' is not a valid hex color; expected 6 (RGB) or 8 (RGBW) hex digits.");
        }

        var r = ParseByte(hex, value, 0);
        var g = ParseByte(hex, value, 2);
        var b = ParseByte(hex, value, 4);
        byte? w = value.Length == 8 ? ParseByte(hex, value, 6) : null;

        return (r, g, b, w);
    }

    public static string Format(byte r, byte g, byte b, byte? w)
    {
        var rgb = $"{r:X2}{g:X2}{b:X2}";
        return w.HasValue ? rgb + w.Value.ToString("X2") : rgb;
    }

    private static byte ParseByte(string original, string value, int index)
    {
        var pair = value.Substring(index, 2);
        if (!byte.TryParse(pair, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            throw new FormatException($"'{original}' contains invalid hex digits ('{pair}').");
        }

        return result;
    }
}
