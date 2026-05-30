namespace Kevsoft.WLED.Tests;

public class PlaylistTests
{
    [Fact]
    public async Task StartPlaylistUnzipsEntriesIntoParallelArrays()
    {
        var (_, body) = await Capture(client => client.StartPlaylist(p => p
            .Add(26, TimeSpan.FromSeconds(3))
            .Add(20, TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(700))
            .Add(18, TimeSpan.FromSeconds(1))
            .Add(20, TimeSpan.FromSeconds(5))
            .Repeat(10)
            .EndOn(21)));

        var playlist = JsonDocument.Parse(body!).RootElement.GetProperty("playlist");
        playlist.GetProperty("ps").EnumerateArray().Select(x => x.GetInt32()).Should().Equal(26, 20, 18, 20);
        playlist.GetProperty("dur").EnumerateArray().Select(x => x.GetInt32()).Should().Equal(30, 20, 10, 50);
        playlist.GetProperty("transition").EnumerateArray().Select(x => x.GetInt32()).Should().Equal(0, 7, 0, 0);
        playlist.GetProperty("repeat").GetInt32().Should().Be(10);
        playlist.GetProperty("end").GetInt32().Should().Be(21);
        playlist.TryGetProperty("r", out _).Should().BeFalse();
    }

    [Fact]
    public async Task StartPlaylistWithoutTransitionsOmitsTransitionArray()
    {
        var (_, body) = await Capture(client => client.StartPlaylist(p => p
            .Add(1, TimeSpan.FromSeconds(1))
            .Add(2, TimeSpan.FromSeconds(2))
            .Shuffle()));

        var playlist = JsonDocument.Parse(body!).RootElement.GetProperty("playlist");
        playlist.TryGetProperty("transition", out _).Should().BeFalse();
        playlist.GetProperty("repeat").GetInt32().Should().Be(0);
        playlist.GetProperty("r").GetBoolean().Should().BeTrue();
        playlist.TryGetProperty("end", out _).Should().BeFalse();
    }

    [Fact]
    public async Task SavePlaylistCombinesPlaylistWithPsave()
    {
        var definition = new PlaylistDefinition
        {
            Entries = new[] { new PlaylistEntry(1, TimeSpan.FromSeconds(2)) },
            Repeat = 3
        };

        var (_, body) = await Capture(client => client.SavePlaylist(7, definition, new SavePresetOptions { Name = "Loop" }));

        var root = JsonDocument.Parse(body!).RootElement;
        root.GetProperty("psave").GetInt32().Should().Be(7);
        root.GetProperty("n").GetString().Should().Be("Loop");
        root.GetProperty("playlist").GetProperty("repeat").GetInt32().Should().Be(3);
    }

    [Fact]
    public async Task GetPlaylistsZipsParallelArraysAndBroadcastsScalars()
    {
        var json = @"{
            ""0"": {},
            ""1"": {""on"":true,""n"":""Scene"",""seg"":[]},
            ""2"": {""n"":""Cycle"",""playlist"":{""ps"":[5,6,7],""dur"":20,""transition"":[0,10,0],""repeat"":4,""end"":1}}
        }";

        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/presets.json", json);
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var playlists = await client.GetPlaylists();

        playlists.Keys.Should().BeEquivalentTo(new[] { 2 });
        var definition = playlists[2].Definition;
        definition.Entries.Should().HaveCount(3);
        definition.Entries[0].PresetId.Should().Be(5);
        definition.Entries[0].Duration.Should().Be(TimeSpan.FromSeconds(2));
        definition.Entries[1].Duration.Should().Be(TimeSpan.FromSeconds(2));
        definition.Entries[1].Transition.Should().Be(TimeSpan.FromSeconds(1));
        definition.Entries[0].Transition.Should().Be(TimeSpan.Zero);
        definition.Repeat.Should().Be(4);
        definition.EndPresetId.Should().Be(1);
        playlists[2].Name.Should().Be("Cycle");
    }

    private static async Task<(string Uri, string? Body)> Capture(Func<WLedClient, Task> act)
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await act(client);

        var (uri, body) = mockHttpMessageHandler.CapturedRequests.Single();
        return (uri, body);
    }
}
