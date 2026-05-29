namespace Kevsoft.WLED;

/// <summary>
/// Information about the embedded LittleFS filesystem (available since 0.11.0).
/// </summary>
public sealed class FilesystemResponse
{
    /// <summary>Estimated used filesystem space, in kilobytes.</summary>
    [JsonPropertyName("u")]
    public uint Used { get; set; }

    /// <summary>Total filesystem size, in kilobytes.</summary>
    [JsonPropertyName("t")]
    public uint Total { get; set; }

    /// <summary>
    /// Raw unix timestamp of the last modification to <c>presets.json</c>. Not accurate
    /// after boot or after using <c>/edit</c>. <c>0</c> when unknown.
    /// </summary>
    [JsonPropertyName("pmt")]
    public long PresetsModifiedTimestamp { get; set; }

    /// <summary>Free filesystem space, in kilobytes.</summary>
    [JsonIgnore]
    public long Free => Total - (long)Used;

    /// <summary>Used filesystem space as a percentage of the total (0–100).</summary>
    [JsonIgnore]
    public double UsedPercentage => Total == 0 ? 0 : (double)Used / Total * 100;

    /// <summary>Free filesystem space as a percentage of the total (0–100).</summary>
    [JsonIgnore]
    public double FreePercentage => Total == 0 ? 0 : (double)Free / Total * 100;

    /// <summary>
    /// The last modification time of <c>presets.json</c>, or <c>null</c> when unknown.
    /// </summary>
    [JsonIgnore]
    public DateTimeOffset? LastModified =>
        PresetsModifiedTimestamp > 0 ? DateTimeOffset.FromUnixTimeSeconds(PresetsModifiedTimestamp) : null;
}
