namespace Kevsoft.WLED;

public sealed class WLedClient
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

    public async Task Post(List<SingleLed> ledList)
    {
        // Eliminate duplicate positions
        ledList = ledList.GroupBy(x => x.LedPosition).Select(x => x.Last()).ToList();

        List<object> list = [];
        int counter = 0;

        //Attempt to group colors together to reduce the number of packets sent as there is a 256 color at a time limit
        foreach (IGrouping<string, SingleLed>? leds in ledList.GroupBy(x => x.Color))
        {
            if (counter >=255)
            {
                await Post(new StateRequest { On = true, Segments = [new() { Id = 0, IndividualLedControl = [.. list] }] });
                list = [];
                counter = 0;
            }
            // If there is only one LED in the group, add it to the list
            if (leds.Count() == 1)
            {
                list.Add(leds.First().LedPosition);
                list.Add(leds.First().Color);
                counter++;
                continue;
            }

            // If there are multiple LEDs in the group, find the sequential LED's and group them up
            // to make the next step easier
            List<List<int>> grouped = leds.Select(x => x.LedPosition).OrderBy(x => x)
                .Aggregate(new List<List<int>> { new() },
                    (acc, curr) =>
                    {
                        if (!acc.Last().Any() || curr - acc.Last().Last() == 1)
                            acc.Last().Add(curr);
                        else
                            acc.Add([curr]);
                        return acc;
                    });

            foreach (List<int> group in grouped)
            {
                //Another round of sending the colors if we are at the limit
                if (counter >= 255)
                {
                    await Post(new StateRequest { On = true, Segments = [new() { Id = 0, IndividualLedControl = [.. list] }] });
                    list = [];
                    counter = 0;
                }

                // If there is only one LED in the group, add it to the list
                if (group.Count == 1)
                {
                    list.Add(group.First());
                    list.Add(leds.First().Color);
                    counter++;
                    continue;
                }

                //And if there are multiple LED's, Add them to the list, but when displaying max
                //is not displayed so add 1 to the max to get it to display properly
                list.Add(group.Min());
                list.Add(group.Max() + 1);
                list.Add(leds.First().Color);
                counter++;
            }
        }
        //And finally send the last packet
        await Post(new StateRequest { On = true, Segments = [new() { Id = 0, IndividualLedControl = [.. list] }] });
    }
}
