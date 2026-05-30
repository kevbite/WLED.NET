namespace Kevsoft.WLED;

/// <summary>
/// The colour capabilities a light/segment supports, as a bitfield.
/// </summary>
[Flags]
public enum LightCapability : byte
{
    /// <summary>No special capabilities.</summary>
    None = 0,

    /// <summary>Supports RGB colour.</summary>
    Rgb = 1,

    /// <summary>Has a dedicated white channel.</summary>
    WhiteChannel = 2,

    /// <summary>Supports colour temperature (CCT).</summary>
    ColorTemperature = 4,

    /// <summary>White channel can be controlled manually.</summary>
    ManualWhite = 8,
}
