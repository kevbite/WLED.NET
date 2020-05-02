using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace Kevsoft.WLED.Tests
{
    public class WLedClientTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task GetsAllData()
        {
            var expected = _fixture.Create<WLedRootResponse>();
            var baseUri = $"http://{Guid.NewGuid():N}.com";
            var mockHttpMessageHandler = new MockHttpMessageHandler();
            var json = $@"{{
  ""state"": {CreateStateJson(expected.State)},
  ""info"": {{
    ""ver"": ""{expected.Information.VersionName}"",
    ""vid"": {expected.Information.BuildId},
    ""leds"": {{
      ""count"": {expected.Information.Leds.Count},
      ""rgbw"": {expected.Information.Leds.Rgbw.ToString().ToLower()},
      ""pin"": [{string.Join(",", expected.Information.Leds.Pin)}],
      ""pwr"": {expected.Information.Leds.PowerUsage},
      ""maxpwr"": {expected.Information.Leds.MaximumPower},
      ""maxseg"": {expected.Information.Leds.MaximumSegments}
    }},
    ""name"": ""{expected.Information.Name}"",
    ""udpport"": {expected.Information.UdpPort},
    ""live"": {expected.Information.Live.ToString().ToLower()},
    ""fxcount"": {expected.Information.EffectsCount},
    ""palcount"": {expected.Information.PalettesCount},
    ""arch"": ""{expected.Information.Arch}"",
    ""core"": ""{expected.Information.Core}"",
    ""freeheap"": {expected.Information.FreeHeapMemory},
    ""uptime"": {expected.Information.UpTime},
    ""opt"": {expected.Information.Opt},
    ""brand"": ""{expected.Information.Brand}"",
    ""product"": ""{expected.Information.Product}"",
    ""btype"": ""{expected.Information.BuildType}"",
    ""mac"": ""{expected.Information.MacAddress}""
  }},
  ""effects"": [
    {string.Join(", ", expected.Effects.Select(x => $@"""{x}"""))}
  ],
  ""palettes"": [
    {string.Join(", ", expected.Palettes.Select(x => $@"""{x}"""))}
  ]
}}";
            mockHttpMessageHandler.AppendResponse($"{baseUri}/json", json);
            var client = new WLedClient(mockHttpMessageHandler, baseUri);

            var root = await client.Get();

            root.Should().BeEquivalentTo(expected);
        }

        private string CreateStateJson(State state)
        {
            return $@"{{
    ""on"": {state.On.ToString().ToLower()},
    ""bri"": {state.Brightness},
    ""transition"": {state.Transition},
    ""ps"": {state.PresetId},
    ""pl"": {state.PlaylistId},
    ""nl"": {{
      ""on"": {state.Nightlight.On.ToString().ToLower()},
      ""dur"": {state.Nightlight.Duration},
      ""fade"": {state.Nightlight.Fade.ToString().ToLower()},
      ""tbri"": {state.Nightlight.TargetBrightness}
    }},
    ""udpn"": {{
      ""send"": {state.UdpPackets.Send.ToString().ToLower()},
      ""recv"": {state.UdpPackets.Receive.ToString().ToLower()}
    }},
    ""seg"": [{string.Join(", ", state.Segments.Select(seg => {
                return $@"{{
      ""start"": {seg.Start},
      ""stop"": {seg.Stop},
      ""len"": {seg.Length},
      ""col"": [
        {string.Join(", ", seg.Colors.Select(col => $"[{string.Join<int>(",", col)}]"))}
      ],
      ""fx"": {seg.EffectId},
      ""sx"": {seg.EffectSpeed},
      ""ix"": {seg.EffectIntensity},
      ""pal"": {seg.ColorPaletteId},
      ""sel"": {seg.Selected.ToString().ToLower()},
      ""rev"": {seg.Reverse.ToString().ToLower()},
      ""cln"": -1
    }}";
            }))}]
  }}";
        }

        [Fact]
        public async Task GetStateData()
        {
            var expected = _fixture.Create<State>();
            var baseUri = $"http://{Guid.NewGuid():N}.com";

            var mockHttpMessageHandler = new MockHttpMessageHandler();
            var json = CreateStateJson(expected);
            mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state", json);

            var client = new WLedClient(mockHttpMessageHandler, baseUri);

            var root = await client.GetState();

            root.Should().BeEquivalentTo(expected);
        }
    }
}