namespace Kevsoft.WLED;

public interface IWLedClient
{
    Task<WLedRootResponse> Get(CancellationToken cancellationToken = default);

    Task<StateResponse> GetState(CancellationToken cancellationToken = default);

    Task<InformationResponse> GetInformation(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the lighter <c>/json/si</c> response, containing only the state and info objects.
    /// </summary>
    Task<StateInfoResponse> GetStateInfo(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the nearby Wi-Fi networks reported by <c>/json/net</c>.
    /// </summary>
    Task<NetworkResponse[]> GetNetworks(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the live LED colour stream from <c>/json/live</c>, or <c>null</c> if the firmware
    /// was not built with JSON-live support.
    /// </summary>
    Task<LiveResponse?> GetLiveColors(CancellationToken cancellationToken = default);

    Task<string[]> GetEffects(CancellationToken cancellationToken = default);

    Task<string[]> GetPalettes(CancellationToken cancellationToken = default);

    Task Post(WLedRootRequest request, CancellationToken cancellationToken = default);

    Task Post(StateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Builds and posts a sparse state update using a fluent builder.
    /// </summary>
    Task UpdateState(Action<StateUpdate> configure, CancellationToken cancellationToken = default);

    /// <summary>Turns the light on.</summary>
    Task TurnOn(CancellationToken cancellationToken = default);

    /// <summary>Turns the light off.</summary>
    Task TurnOff(CancellationToken cancellationToken = default);

    /// <summary>Toggles the light on/off.</summary>
    Task Toggle(CancellationToken cancellationToken = default);

    /// <summary>Sets, nudges or wraps the master brightness (0–255).</summary>
    Task SetBrightness(ByteAdjust brightness, CancellationToken cancellationToken = default);

    /// <summary>Sets an RGB colour on a segment, or the selected segments when no id is given.</summary>
    Task SetColor(RgbColor color, int? segmentId = null, CancellationToken cancellationToken = default);

    /// <summary>Sets an RGBW colour on a segment, or the selected segments when no id is given.</summary>
    Task SetColor(RgbwColor color, int? segmentId = null, CancellationToken cancellationToken = default);

    /// <summary>Sets the effect on a segment, or the selected segments when no id is given.</summary>
    Task SetEffect(Selector effect, int? segmentId = null, CancellationToken cancellationToken = default);

    /// <summary>Sets the palette on a segment, or the selected segments when no id is given.</summary>
    Task SetPalette(Selector palette, int? segmentId = null, CancellationToken cancellationToken = default);

    /// <summary>Reboots the device.</summary>
    Task Reboot(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the saved presets, keyed by slot id. Playlists and the scratch slot are excluded.
    /// </summary>
    Task<IReadOnlyDictionary<int, Preset>> GetPresets(CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies a preset by id, or cycles/randomises between presets.
    /// </summary>
    Task ApplyPreset(PresetSelector preset, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the device's current live state to the given preset slot.
    /// </summary>
    Task SavePreset(int id, SavePresetOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the preset in the given slot.
    /// </summary>
    Task DeletePreset(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the saved playlists, keyed by slot id.
    /// </summary>
    Task<IReadOnlyDictionary<int, Playlist>> GetPlaylists(CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts the given playlist immediately.
    /// </summary>
    Task StartPlaylist(PlaylistDefinition playlist, CancellationToken cancellationToken = default);

    /// <summary>
    /// Builds and starts a playlist using a fluent builder.
    /// </summary>
    Task StartPlaylist(Action<PlaylistBuilder> configure, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a playlist to the given preset slot.
    /// </summary>
    Task SavePlaylist(int id, PlaylistDefinition playlist, SavePresetOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets individual LEDs within a segment.
    /// </summary>
    /// <remarks>
    /// Setting LEDs freezes the segment, so brightness and power must be set in an earlier request.
    /// Large sets are transparently split into multiple sequential requests (never parallel), each
    /// carrying at most <paramref name="maxColorsPerRequest"/> colours.
    /// </remarks>
    Task SetIndividualLeds(int segmentId, Action<IndividualLedBuilder> build, int maxColorsPerRequest = 256, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets parsed effect metadata from <c>/json/fxdata</c>, describing which controls each effect uses.
    /// Reserved effects are excluded; <see cref="EffectMetadata.EffectId"/> stays aligned with the effects list.
    /// </summary>
    Task<IReadOnlyList<EffectMetadata>> GetEffectMetadata(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets other WLED devices discovered on the local network via <c>/json/nodes</c>.
    /// </summary>
    Task<IReadOnlyList<WledNode>> GetNodes(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the full device configuration from <c>/json/cfg</c>.
    /// </summary>
    Task<DeviceConfig> GetConfig(CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies a partial device configuration update to <c>/json/cfg</c>. Only the sections set on
    /// <paramref name="partial"/> are sent. Updating the network or access-point sections requires
    /// opting in via <see cref="UpdateConfigOptions.AllowNetworkChanges"/>.
    /// </summary>
    Task UpdateConfig(DeviceConfig partial, UpdateConfigOptions? options = null, CancellationToken cancellationToken = default);
}
