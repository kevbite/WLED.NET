using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace Kevsoft.WLED.Tests
{
    public class WLedClientTests
    {
        [Fact]
        public async Task GetsAllData()
        {
            var fixture = new Fixture();
            var expected = fixture.Create<WLedRootResponse>();
            var baseUri = "http://test123.com";
            var mockHttpMessageHandler = new MockHttpMessageHandler();
            var json = $@"{{
  ""state"": {{
    ""on"": {expected.State.On.ToString().ToLower()},
    ""bri"": {expected.State.Brightness},
    ""transition"": {expected.State.Transition},
    ""ps"": {expected.State.PresetId},
    ""pl"": {expected.State.PlaylistId},
    ""nl"": {{
      ""on"": {expected.State.Nightlight.On.ToString().ToLower()},
      ""dur"": {expected.State.Nightlight.Duration},
      ""fade"": {expected.State.Nightlight.Fade.ToString().ToLower()},
      ""tbri"": {expected.State.Nightlight.TargetBrightness}
    }},
    ""udpn"": {{
      ""send"": {expected.State.UdpPackets.Send.ToString().ToLower()},
      ""recv"": {expected.State.UdpPackets.Receive.ToString().ToLower()}
    }},
    ""seg"": [{string.Join(", ", expected.State.Segments.Select(seg => {
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
  }},
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
    }
}