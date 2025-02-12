namespace Kevsoft.WLED;

public class MatrixResponse
{
    /// <summary> The number of LEDs in the width of the matrix </summary>
    [JsonPropertyName("w")]
    public int Width { get; set; }

    /// <summary> The number of LEDs in the Height of the matrix </summary>
    [JsonPropertyName("h")]
    public int Height { get; set; }
}