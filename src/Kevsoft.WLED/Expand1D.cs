namespace Kevsoft.WLED;

/// <summary>
/// How a 1D effect is expanded onto a 2D matrix.
/// </summary>
public enum Expand1D : byte
{
    /// <summary>Map pixel-for-pixel.</summary>
    Pixels = 0,

    /// <summary>Expand as a bar.</summary>
    Bar = 1,

    /// <summary>Expand as an arc.</summary>
    Arc = 2,

    /// <summary>Expand from a corner.</summary>
    Corner = 3,
}
