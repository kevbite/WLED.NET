using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.VisualBasic;
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
  ""info"": {CreateInformationJson(expected.Information)},
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
    ""seg"": [{string.Join(", ", state.Segments.Select(seg =>
            {
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

        [Fact]
        public async Task GetInformationData()
        {
            var expected = _fixture.Create<Information>();
            var baseUri = $"http://{Guid.NewGuid():N}.com";

            var mockHttpMessageHandler = new MockHttpMessageHandler();
            var json = CreateInformationJson(expected);
            mockHttpMessageHandler.AppendResponse($"{baseUri}/json/info", json);

            var client = new WLedClient(mockHttpMessageHandler, baseUri);

            var root = await client.GetInformation();

            root.Should().BeEquivalentTo(expected);
        }


        [Fact]
        public async Task GetEffectsData()
        {
            var baseUri = $"http://{Guid.NewGuid():N}.com";

            var mockHttpMessageHandler = new MockHttpMessageHandler();
            var json = @"[""Effect A"", ""Effect B"" ]";
            mockHttpMessageHandler.AppendResponse($"{baseUri}/json/eff", json);

            var client = new WLedClient(mockHttpMessageHandler, baseUri);

            var root = await client.GetEffects();

            root.Should().BeEquivalentTo(new[]
                {"Effect A", "Effect B"}
            );
        }

        [Fact]
        public async Task GetPalettesData()
        {;
            var baseUri = $"http://{Guid.NewGuid():N}.com";

            var mockHttpMessageHandler = new MockHttpMessageHandler();
            var json = @"[""Palette A"", ""Palette B"" ]";
            mockHttpMessageHandler.AppendResponse($"{baseUri}/json/pal", json);

            var client = new WLedClient(mockHttpMessageHandler, baseUri);

            var root = await client.GetPalettes();

            root.Should().BeEquivalentTo(new[]
                {"Palette A", "Palette B"}
            );
        }

        private string CreateInformationJson(Information information)
        {
            return $@"{{
    ""ver"": ""{information.VersionName}"",
    ""vid"": {information.BuildId},
    ""leds"": {{
      ""count"": {information.Leds.Count},
      ""rgbw"": {information.Leds.Rgbw.ToString().ToLower()},
      ""pin"": [{string.Join(",", information.Leds.Pin)}],
      ""pwr"": {information.Leds.PowerUsage},
      ""maxpwr"": {information.Leds.MaximumPower},
      ""maxseg"": {information.Leds.MaximumSegments}
    }},
    ""name"": ""{information.Name}"",
    ""udpport"": {information.UdpPort},
    ""live"": {information.Live.ToString().ToLower()},
    ""fxcount"": {information.EffectsCount},
    ""palcount"": {information.PalettesCount},
    ""arch"": ""{information.Arch}"",
    ""core"": ""{information.Core}"",
    ""freeheap"": {information.FreeHeapMemory},
    ""uptime"": {information.UpTime},
    ""opt"": {information.Opt},
    ""brand"": ""{information.Brand}"",
    ""product"": ""{information.Product}"",
    ""btype"": ""{information.BuildType}"",
    ""mac"": ""{information.MacAddress}""
  }}";
        }
    }
}