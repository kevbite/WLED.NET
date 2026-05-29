namespace Kevsoft.WLED;

public interface IWLedClient
{
    Task<WLedRootResponse> Get();

    Task<StateResponse> GetState();

    Task<InformationResponse> GetInformation();

    /// <summary>
    /// Gets the lighter <c>/json/si</c> response, containing only the state and info objects.
    /// </summary>
    Task<StateInfoResponse> GetStateInfo();

    /// <summary>
    /// Gets the nearby Wi-Fi networks reported by <c>/json/net</c>.
    /// </summary>
    Task<NetworkResponse[]> GetNetworks();

    /// <summary>
    /// Gets the live LED colour stream from <c>/json/live</c>, or <c>null</c> if the firmware
    /// was not built with JSON-live support.
    /// </summary>
    Task<LiveResponse?> GetLiveColors();

    Task<string[]> GetEffects();

    Task<string[]> GetPalettes();

    Task Post(WLedRootRequest request);

    Task Post(StateRequest request);

    /// <summary>
    /// Builds and posts a sparse state update using a fluent builder.
    /// </summary>
    Task UpdateState(Action<StateUpdate> configure);
}