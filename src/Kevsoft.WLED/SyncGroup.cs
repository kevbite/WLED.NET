namespace Kevsoft.WLED;

/// <summary>
/// The sync groups a segment belongs to, as a bitfield (groups 1–8).
/// </summary>
[Flags]
public enum SyncGroup : byte
{
    /// <summary>No groups.</summary>
    None = 0,
    Group1 = 1,
    Group2 = 2,
    Group3 = 4,
    Group4 = 8,
    Group5 = 16,
    Group6 = 32,
    Group7 = 64,
    Group8 = 128,
}
