namespace Kevsoft.WLED;

/// <summary>
/// Controls how incoming realtime / live data overrides the normal effect output.
/// </summary>
public enum LiveDataOverride : byte
{
    /// <summary>Live data is shown while it is being received.</summary>
    Off = 0,

    /// <summary>Ignore live data until the live source stops sending.</summary>
    UntilLiveEnds = 1,

    /// <summary>Ignore live data until the device reboots.</summary>
    UntilReboot = 2,
}
