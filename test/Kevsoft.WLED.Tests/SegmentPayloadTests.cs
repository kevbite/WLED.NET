namespace Kevsoft.WLED.Tests;

public class SegmentPayloadTests
{
    private static readonly JsonSerializerOptions Options = new();

    [Fact]
    public void SelectedSerializesAsObject()
    {
        var payload = SegmentPayload.Selected(new SegmentRequest { EffectId = 5 });

        var json = JsonSerializer.Serialize(payload, Options);

        var root = JsonDocument.Parse(json).RootElement;
        root.ValueKind.Should().Be(JsonValueKind.Object);
        root.GetProperty("fx").GetInt32().Should().Be(5);
        root.TryGetProperty("id", out _).Should().BeFalse();
    }

    [Fact]
    public void ListSerializesAsArray()
    {
        var payload = SegmentPayload.List(
            new SegmentRequest { Id = 0, EffectId = 1 },
            new SegmentRequest { Id = 1, EffectId = 2 });

        var json = JsonSerializer.Serialize(payload, Options);

        var root = JsonDocument.Parse(json).RootElement;
        root.ValueKind.Should().Be(JsonValueKind.Array);
        root.GetArrayLength().Should().Be(2);
        root[0].GetProperty("id").GetInt32().Should().Be(0);
    }

    [Fact]
    public void ImplicitFromSingleSegmentWithoutIdIsObject()
    {
        SegmentPayload payload = new SegmentRequest { EffectId = 9 };

        JsonDocument.Parse(JsonSerializer.Serialize(payload, Options)).RootElement
            .ValueKind.Should().Be(JsonValueKind.Object);
    }

    [Fact]
    public void ImplicitFromSingleSegmentWithIdIsArray()
    {
        SegmentPayload payload = new SegmentRequest { Id = 2, EffectId = 9 };

        JsonDocument.Parse(JsonSerializer.Serialize(payload, Options)).RootElement
            .ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public void ImplicitFromArrayIsArray()
    {
        SegmentPayload payload = new[] { new SegmentRequest { Id = 3 } };

        JsonDocument.Parse(JsonSerializer.Serialize(payload, Options)).RootElement
            .ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public void ReadsBothObjectAndArrayForms()
    {
        var single = JsonSerializer.Deserialize<SegmentPayload>("{\"fx\":4}", Options)!;
        var many = JsonSerializer.Deserialize<SegmentPayload>("[{\"id\":0},{\"id\":1}]", Options)!;

        JsonDocument.Parse(JsonSerializer.Serialize(single, Options)).RootElement
            .ValueKind.Should().Be(JsonValueKind.Object);
        JsonDocument.Parse(JsonSerializer.Serialize(many, Options)).RootElement
            .ValueKind.Should().Be(JsonValueKind.Array);
    }
}
