namespace Kevsoft.WLED;

/// <summary>
/// The built-in audio simulation used when no real sound input is available.
/// </summary>
public enum SoundSimulation : byte
{
    /// <summary>Beat-synced sine wave.</summary>
    BeatSin = 0,

    /// <summary>"We Will Rock You" rhythm.</summary>
    WeWillRockYou = 1,

    /// <summary>Simulation mode 10_3.</summary>
    Mode10_3 = 2,

    /// <summary>Simulation mode 14_3.</summary>
    Mode14_3 = 3,
}
