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

    /// <summary>
    /// Gets the saved presets, keyed by slot id. Playlists and the scratch slot are excluded.
    /// </summary>
    Task<IReadOnlyDictionary<int, Preset>> GetPresets();

    /// <summary>
    /// Applies a preset by id, or cycles/randomises between presets.
    /// </summary>
    Task ApplyPreset(PresetSelector preset);

    /// <summary>
    /// Saves the device's current live state to the given preset slot.
    /// </summary>
    Task SavePreset(int id, SavePresetOptions? options = null);

    /// <summary>
    /// Deletes the preset in the given slot.
    /// </summary>
    Task DeletePreset(int id);

    /// <summary>
    /// Gets the saved playlists, keyed by slot id.
    /// </summary>
    Task<IReadOnlyDictionary<int, Playlist>> GetPlaylists();

    /// <summary>
    /// Starts the given playlist immediately.
    /// </summary>
    Task StartPlaylist(PlaylistDefinition playlist);

    /// <summary>
    /// Builds and starts a playlist using a fluent builder.
    /// </summary>
    Task StartPlaylist(Action<PlaylistBuilder> configure);

    /// <summary>
    /// Saves a playlist to the given preset slot.
    /// </summary>
    Task SavePlaylist(int id, PlaylistDefinition playlist, SavePresetOptions? options = null);

    /// <summary>
    /// Sets individual LEDs within a segment.
    /// </summary>
    /// <remarks>
    /// Setting LEDs freezes the segment, so brightness and power must be set in an earlier request.
    /// Large sets are transparently split into multiple sequential requests (never parallel), each
    /// carrying at most <paramref name="maxColorsPerRequest"/> colours.
    /// </remarks>
    Task SetIndividualLeds(int segmentId, Action<IndividualLedBuilder> build, int maxColorsPerRequest = 256);
}