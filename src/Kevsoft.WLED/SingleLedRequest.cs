namespace Kevsoft.WLED;

public sealed class SingleLedRequest
{
    /// <summary> The position of the LED in the segment </summary>
    public int LedPosition { get; set; }

    /// <summary> The color of the LED as HEX (e.g. FF0000 for red) </summary>
    public string Color { get; set; } = string.Empty;
}
