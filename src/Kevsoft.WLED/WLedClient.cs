namespace Kevsoft.WLED;

public sealed class WLedClient : IWLedClient
{
    private readonly HttpClient _client;

    public WLedClient(HttpMessageHandler httpMessageHandler, string baseUri)
        : this(CreateClient(httpMessageHandler, baseUri))
    {
    }

    public WLedClient(string baseUri) : this(new HttpClientHandler(), baseUri)
    {
    }

    /// <summary>
    /// Creates a client over a pre-configured <see cref="HttpClient"/>. The client's
    /// <see cref="HttpClient.BaseAddress"/> must be set. Intended for <c>IHttpClientFactory</c>/DI use.
    /// </summary>
    public WLedClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));

        if (_client.BaseAddress is null)
        {
            throw new ArgumentException("The HttpClient must have a BaseAddress set.", nameof(client));
        }
    }

    private static HttpClient CreateClient(HttpMessageHandler httpMessageHandler, string baseUri)
    {
        var client = new HttpClient(httpMessageHandler)
        {
            BaseAddress = new Uri(baseUri, UriKind.Absolute)
        };

        client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        return client;
    }

    public Task<WLedRootResponse> Get(CancellationToken cancellationToken = default)
        => GetJson<WLedRootResponse>("json", cancellationToken);

    public Task<StateResponse> GetState(CancellationToken cancellationToken = default)
        => GetJson<StateResponse>("json/state", cancellationToken);

    public Task<InformationResponse> GetInformation(CancellationToken cancellationToken = default)
        => GetJson<InformationResponse>("json/info", cancellationToken);

    public Task<StateInfoResponse> GetStateInfo(CancellationToken cancellationToken = default)
        => GetJson<StateInfoResponse>("json/si", cancellationToken);

    public async Task<NetworkResponse[]> GetNetworks(CancellationToken cancellationToken = default)
    {
        var response = await GetJson<NetworksResponse?>("json/net", cancellationToken);
        return response?.Networks ?? Array.Empty<NetworkResponse>();
    }

    public async Task<LiveResponse?> GetLiveColors(CancellationToken cancellationToken = default)
    {
        var message = await SendGetAsync("json/live", cancellationToken);

        if (message.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccess(message);

        if (message.Content.Headers.ContentLength == 0)
        {
            return null;
        }

        return await message.Content.ReadFromJsonAsync<LiveResponse>(cancellationToken: cancellationToken);
    }

    public Task<string[]> GetEffects(CancellationToken cancellationToken = default)
        => GetJson<string[]>("json/eff", cancellationToken);

    public Task<string[]> GetPalettes(CancellationToken cancellationToken = default)
        => GetJson<string[]>("json/pal", cancellationToken);

    public Task Post(WLedRootRequest request, CancellationToken cancellationToken = default)
        => PostJson("/json", request, cancellationToken);

    public Task Post(StateRequest request, CancellationToken cancellationToken = default)
        => PostJson("/json/state", request, cancellationToken);

    public Task UpdateState(Action<StateUpdate> configure, CancellationToken cancellationToken = default)
    {
        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var update = new StateUpdate();
        configure(update);
        return Post(update.Build(), cancellationToken);
    }

    public Task TurnOn(CancellationToken cancellationToken = default)
        => Post(new StateRequest { On = Toggleable.On }, cancellationToken);

    public Task TurnOff(CancellationToken cancellationToken = default)
        => Post(new StateRequest { On = Toggleable.Off }, cancellationToken);

    public Task Toggle(CancellationToken cancellationToken = default)
        => Post(new StateRequest { On = Toggleable.Toggle }, cancellationToken);

    public Task SetBrightness(ByteAdjust brightness, CancellationToken cancellationToken = default)
        => Post(new StateRequest { Brightness = brightness }, cancellationToken);

    public Task SetColor(RgbColor color, int? segmentId = null, CancellationToken cancellationToken = default)
        => Post(SingleSegment(segmentId, segment => segment.Colors = new SegmentColors(color)), cancellationToken);

    public Task SetColor(RgbwColor color, int? segmentId = null, CancellationToken cancellationToken = default)
        => Post(SingleSegment(segmentId, segment => segment.Colors = new SegmentColors(color)), cancellationToken);

    public Task SetEffect(Selector effect, int? segmentId = null, CancellationToken cancellationToken = default)
        => Post(SingleSegment(segmentId, segment => segment.EffectId = effect), cancellationToken);

    public Task SetPalette(Selector palette, int? segmentId = null, CancellationToken cancellationToken = default)
        => Post(SingleSegment(segmentId, segment => segment.ColorPaletteId = palette), cancellationToken);

    public Task Reboot(CancellationToken cancellationToken = default)
        => Post(new StateRequest { Reboot = true }, cancellationToken);

    public async Task<IReadOnlyDictionary<int, Preset>> GetPresets(CancellationToken cancellationToken = default)
    {
        var json = await GetString("presets.json", cancellationToken);
        return PresetsParser.ParsePresets(json, new JsonSerializerOptions());
    }

    public Task ApplyPreset(PresetSelector preset, CancellationToken cancellationToken = default)
        => Post(new StateRequest { PresetId = preset }, cancellationToken);

    public Task SavePreset(int id, SavePresetOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= new SavePresetOptions();

        return Post(new StateRequest
        {
            SavePresetSlot = id,
            PresetName = options.Name,
            QuickLabel = options.QuickLabel,
            SaveSegmentBounds = options.SaveSegmentBounds,
            IncludeBrightness = options.IncludeBrightness,
            SaveSelectedSegments = options.SaveSelectedSegments
        }, cancellationToken);
    }

    public Task DeletePreset(int id, CancellationToken cancellationToken = default)
        => Post(new StateRequest { DeletePresetSlot = id }, cancellationToken);

    public async Task<IReadOnlyDictionary<int, Playlist>> GetPlaylists(CancellationToken cancellationToken = default)
    {
        var json = await GetString("presets.json", cancellationToken);
        return PlaylistsParser.ParsePlaylists(json);
    }

    public Task StartPlaylist(PlaylistDefinition playlist, CancellationToken cancellationToken = default)
    {
        if (playlist is null)
        {
            throw new ArgumentNullException(nameof(playlist));
        }

        return Post(new StateRequest { Playlist = PlaylistRequest.From(playlist) }, cancellationToken);
    }

    public Task StartPlaylist(Action<PlaylistBuilder> configure, CancellationToken cancellationToken = default)
    {
        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var builder = new PlaylistBuilder();
        configure(builder);
        return StartPlaylist(builder.Build(), cancellationToken);
    }

    public Task SavePlaylist(int id, PlaylistDefinition playlist, SavePresetOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (playlist is null)
        {
            throw new ArgumentNullException(nameof(playlist));
        }

        options ??= new SavePresetOptions();

        return Post(new StateRequest
        {
            SavePresetSlot = id,
            PresetName = options.Name,
            QuickLabel = options.QuickLabel,
            SaveSegmentBounds = options.SaveSegmentBounds,
            IncludeBrightness = options.IncludeBrightness,
            SaveSelectedSegments = options.SaveSelectedSegments,
            Playlist = PlaylistRequest.From(playlist)
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<EffectMetadata>> GetEffectMetadata(CancellationToken cancellationToken = default)
    {
        var fxdata = await GetJson<string[]>("json/fxdata", cancellationToken);
        var effects = await GetEffects(cancellationToken);

        return EffectMetadataParser.Parse(fxdata, effects);
    }

    public async Task<IReadOnlyList<WledNode>> GetNodes(CancellationToken cancellationToken = default)
    {
        var message = await SendGetAsync("json/nodes", cancellationToken);

        await EnsureSuccess(message);

        if (message.Content.Headers.ContentLength == 0)
        {
            return Array.Empty<WledNode>();
        }

        var response = await message.Content.ReadFromJsonAsync<NodesResponse>(cancellationToken: cancellationToken);
        return response?.Nodes ?? Array.Empty<WledNode>();
    }

    public Task<DeviceConfig> GetConfig(CancellationToken cancellationToken = default)
        => GetJson<DeviceConfig>("json/cfg", cancellationToken);

    public Task UpdateConfig(DeviceConfig partial, UpdateConfigOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (partial is null)
        {
            throw new ArgumentNullException(nameof(partial));
        }

        options ??= new UpdateConfigOptions();

        if (!options.AllowNetworkChanges && (partial.Network is not null || partial.AccessPoint is not null))
        {
            throw new InvalidOperationException(
                "Updating the network (nw) or access-point (ap) configuration can disconnect the device. " +
                "Set UpdateConfigOptions.AllowNetworkChanges to true to permit it.");
        }

        return PostJson("/json/cfg", partial, cancellationToken);
    }

    public async Task SetIndividualLeds(int segmentId, Action<IndividualLedBuilder> build, int maxColorsPerRequest = 256, CancellationToken cancellationToken = default)
    {
        if (build is null)
        {
            throw new ArgumentNullException(nameof(build));
        }

        var builder = new IndividualLedBuilder();
        build(builder);

        foreach (var request in builder.Build(maxColorsPerRequest))
        {
            await Post(new StateRequest
            {
                Segments = new[]
                {
                    new SegmentRequest { Id = segmentId, IndividualLeds = request }
                }
            }, cancellationToken);
        }
    }

    private static StateRequest SingleSegment(int? segmentId, Action<SegmentRequest> configure)
    {
        var segment = new SegmentRequest { Id = segmentId };
        configure(segment);

        // No id => target the selected segments via the object form ("seg":{...}).
        // An explicit id => target that segment via the array form ("seg":[{"id":N,...}]).
        return new StateRequest
        {
            Segments = segmentId is null
                ? SegmentPayload.Selected(segment)
                : SegmentPayload.List(segment)
        };
    }

    private async Task<T> GetJson<T>(string uri, CancellationToken cancellationToken)
    {
        var message = await SendGetAsync(uri, cancellationToken);
        await EnsureSuccess(message);
        return (await message.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken))!;
    }

    private async Task<string> GetString(string uri, CancellationToken cancellationToken)
    {
        var message = await SendGetAsync(uri, cancellationToken);
        await EnsureSuccess(message);
        return await message.Content.ReadAsStringAsync();
    }

    private async Task PostJson<T>(string uri, T payload, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(payload);

        using var content = new StringContentWithoutCharset(json, "application/json");
        var result = await SendPostAsync(uri, content, cancellationToken);
        await EnsureSuccess(result);
    }

    private async Task<HttpResponseMessage> SendGetAsync(string uri, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.GetAsync(uri, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new WledConnectionException($"Failed to reach the WLED device at '{_client.BaseAddress}'.", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new WledConnectionException("The request to the WLED device timed out.", ex);
        }
    }

    private async Task<HttpResponseMessage> SendPostAsync(string uri, HttpContent content, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.PostAsync(uri, content, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new WledConnectionException($"Failed to reach the WLED device at '{_client.BaseAddress}'.", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new WledConnectionException("The request to the WLED device timed out.", ex);
        }
    }

    private static async Task EnsureSuccess(HttpResponseMessage message)
    {
        if (message.IsSuccessStatusCode)
        {
            return;
        }

        string? body = null;
        if (message.Content is not null)
        {
            try
            {
                body = await message.Content.ReadAsStringAsync();
            }
            catch
            {
                // Best-effort body capture for diagnostics.
            }
        }

        throw new WledResponseException((int)message.StatusCode, body);
    }
}
