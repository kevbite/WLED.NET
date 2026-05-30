namespace Kevsoft.WLED.Tests;

public class IndividualLedTests
{
    [Fact]
    public async Task SequentialFormEmitsBareHexColours()
    {
        var body = await CaptureSingle(client => client.SetIndividualLeds(0, b => b
            .Set(Color.Rgb(255, 0, 0), Color.Rgb(0, 255, 0), Color.Rgb(0, 0, 255))));

        var i = SegmentI(body);
        i.Select(x => x.GetString()).Should().Equal("FF0000", "00FF00", "0000FF");
    }

    [Fact]
    public async Task IndexedFormEmitsIndexColourPairs()
    {
        var body = await CaptureSingle(client => client.SetIndividualLeds(0, b => b
            .Set(0, Color.Rgb(255, 0, 0))
            .Set(2, Color.Rgb(0, 255, 0))
            .Set(4, Color.Rgb(0, 0, 255))));

        var i = SegmentI(body);
        i[0].GetInt32().Should().Be(0);
        i[1].GetString().Should().Be("FF0000");
        i[2].GetInt32().Should().Be(2);
        i[3].GetString().Should().Be("00FF00");
        i[4].GetInt32().Should().Be(4);
        i[5].GetString().Should().Be("0000FF");
    }

    [Fact]
    public async Task RangeFormEmitsStartStopColourTriples()
    {
        var body = await CaptureSingle(client => client.SetIndividualLeds(0, b => b
            .SetRange(0, 8, Color.Rgb(255, 0, 0))
            .SetRange(10, 18, Color.Rgb(0, 0, 255))));

        var i = SegmentI(body);
        i[0].GetInt32().Should().Be(0);
        i[1].GetInt32().Should().Be(8);
        i[2].GetString().Should().Be("FF0000");
        i[3].GetInt32().Should().Be(10);
        i[4].GetInt32().Should().Be(18);
        i[5].GetString().Should().Be("0000FF");
    }

    [Fact]
    public async Task RgbwColoursFallBackToByteArrays()
    {
        var body = await CaptureSingle(client => client.SetIndividualLeds(0, b => b
            .Set(Color.Rgbw(255, 0, 0, 128))));

        var i = SegmentI(body);
        i[0].ValueKind.Should().Be(JsonValueKind.Array);
        i[0].EnumerateArray().Select(x => x.GetInt32()).Should().Equal(255, 0, 0, 128);
    }

    [Fact]
    public async Task LargeSequentialSetIsSplitIntoSequentialRequests()
    {
        var colors = Enumerable.Range(0, 600).Select(n => Color.Rgb((byte)(n % 256), 0, 0)).ToArray();

        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await client.SetIndividualLeds(0, b => b.Set(colors));

        var requests = mockHttpMessageHandler.CapturedRequestList;
        requests.Should().HaveCount(3);

        var first = SegmentI(requests[0].Body);
        first[0].GetString().Should().Be("000000");
        first.Length.Should().Be(256);

        var second = SegmentI(requests[1].Body);
        second[0].GetInt32().Should().Be(256);
        second.Length.Should().Be(257);

        var third = SegmentI(requests[2].Body);
        third[0].GetInt32().Should().Be(512);
        third.Length.Should().Be(89);
    }

    [Fact]
    public void NegativeIndexThrows()
    {
        var builder = new IndividualLedBuilder();
        Action act = () => builder.Set(-1, Color.Rgb(0, 0, 0));
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void RangeStopNotGreaterThanStartThrows()
    {
        var builder = new IndividualLedBuilder();
        Action act = () => builder.SetRange(5, 5, Color.Rgb(0, 0, 0));
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    private static JsonElement[] SegmentI(string? body)
    {
        var seg = JsonDocument.Parse(body!).RootElement.GetProperty("seg")[0];
        return seg.GetProperty("i").EnumerateArray().ToArray();
    }

    private static async Task<string?> CaptureSingle(Func<WLedClient, Task> act)
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await act(client);

        return mockHttpMessageHandler.CapturedRequestList.Single().Body;
    }
}
