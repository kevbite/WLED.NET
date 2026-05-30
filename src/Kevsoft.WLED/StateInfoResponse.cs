namespace Kevsoft.WLED;

/// <summary>
/// The lighter <c>/json/si</c> response, containing only the state and info objects.
/// </summary>
public sealed class StateInfoResponse
{
    /// <summary>The current device state.</summary>
    [JsonPropertyName("state")]
    public StateResponse State { get; set; } = null!;

    /// <summary>The device information.</summary>
    [JsonPropertyName("info")]
    public InformationResponse Info { get; set; } = null!;
}
