namespace Kevsoft.WLED;

/// <summary>
/// How the nightlight fades the light over its duration.
/// </summary>
public enum NightlightMode : byte
{
    /// <summary>Instantly switch to the target brightness at the end.</summary>
    Instant = 0,

    /// <summary>Linearly fade to the target brightness.</summary>
    Fade = 1,

    /// <summary>Fade following a colour-temperature curve.</summary>
    ColorFade = 2,

    /// <summary>Sunrise/sunset simulation.</summary>
    Sunrise = 3,
}
