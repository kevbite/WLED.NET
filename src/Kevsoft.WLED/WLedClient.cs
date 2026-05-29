namespace Kevsoft.WLED;

public sealed class WLedClient : IWLedClient
{
    private readonly HttpClient _client;

    public WLedClient(HttpMessageHandler httpMessageHandler, string baseUri)
    {
        _client = new HttpClient(httpMessageHandler)
        {
            BaseAddress = new Uri(baseUri, UriKind.Absolute)
        };

        // Add the keep-alive flag to the header
        _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
    }

    public WLedClient(string baseUri) : this(new HttpClientHandler(), baseUri)
    {

    }

    public async Task<WLedRootResponse> Get()
    {
        var message = await _client.GetAsync("json");

        message.EnsureSuccessStatusCode();

        return (await message.Content.ReadFromJsonAsync<WLedRootResponse>())!;
    }

    public async Task<StateResponse> GetState()
    {
        var message = await _client.GetAsync("json/state");

        message.EnsureSuccessStatusCode();

        return (await message.Content.ReadFromJsonAsync<StateResponse>())!;
    }

    public async Task<InformationResponse> GetInformation()
    {
        var message = await _client.GetAsync("json/info");

        message.EnsureSuccessStatusCode();

        return (await message.Content.ReadFromJsonAsync<InformationResponse>())!;
    }

    public async Task<StateInfoResponse> GetStateInfo()
    {
        var message = await _client.GetAsync("json/si");

        message.EnsureSuccessStatusCode();

        return (await message.Content.ReadFromJsonAsync<StateInfoResponse>())!;
    }

    public async Task<NetworkResponse[]> GetNetworks()
    {
        var message = await _client.GetAsync("json/net");

        message.EnsureSuccessStatusCode();

        var response = await message.Content.ReadFromJsonAsync<NetworksResponse>();
        return response?.Networks ?? Array.Empty<NetworkResponse>();
    }

    public async Task<LiveResponse?> GetLiveColors()
    {
        var message = await _client.GetAsync("json/live");

        if (message.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        message.EnsureSuccessStatusCode();

        if (message.Content.Headers.ContentLength == 0)
        {
            return null;
        }

        return await message.Content.ReadFromJsonAsync<LiveResponse>();
    }

    public async Task<string[]> GetEffects()
    {
        var message = await _client.GetAsync("json/eff");

        message.EnsureSuccessStatusCode();

        return (await message.Content.ReadFromJsonAsync<string[]>())!;
    }

    public async Task<string[]> GetPalettes()
    {
        var message = await _client.GetAsync("json/pal");

        message.EnsureSuccessStatusCode();
            
        return (await message.Content.ReadFromJsonAsync<string[]>())!;
    }

    public async Task Post(WLedRootRequest request)
    {
        var stateString = JsonSerializer.Serialize(request);

        using var content = new StringContentWithoutCharset(stateString, "application/json");
        var result = await _client.PostAsync("/json", content);
        result.EnsureSuccessStatusCode();
    }
        
    public async Task Post(StateRequest request)
    {
        var stateString = JsonSerializer.Serialize(request);

        using var content = new StringContentWithoutCharset(stateString, "application/json");
        var result = await _client.PostAsync("/json/state", content);
        result.EnsureSuccessStatusCode();
    }

    public Task UpdateState(Action<StateUpdate> configure)
    {
        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var update = new StateUpdate();
        configure(update);
        return Post(update.Build());
    }

    public async Task<IReadOnlyDictionary<int, Preset>> GetPresets()
    {
        var message = await _client.GetAsync("presets.json");

        message.EnsureSuccessStatusCode();

        var json = await message.Content.ReadAsStringAsync();
        return PresetsParser.ParsePresets(json, new JsonSerializerOptions());
    }

    public Task ApplyPreset(PresetSelector preset) => Post(new StateRequest { PresetId = preset });

    public Task SavePreset(int id, SavePresetOptions? options = null)
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
        });
    }

    public Task DeletePreset(int id) => Post(new StateRequest { DeletePresetSlot = id });
}