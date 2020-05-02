using System;
using System.Linq;

namespace Kevsoft.WLED.Tests
{
    public class JsonBuilder
    {
        public static string CreateStateJson(State state)
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
    ""seg"": [{String.Join(", ", state.Segments.Select(seg =>
            {
                return $@"{{
      ""start"": {seg.Start},
      ""stop"": {seg.Stop},
      ""len"": {seg.Length},
      ""col"": [
        {String.Join(", ", seg.Colors.Select(col => $"[{String.Join(",", col)}]"))}
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

        public static string CreateInformationJson(Information information)
        {
            return $@"{{
    ""ver"": ""{information.VersionName}"",
    ""vid"": {information.BuildId},
    ""leds"": {{
      ""count"": {information.Leds.Count},
      ""rgbw"": {information.Leds.Rgbw.ToString().ToLower()},
      ""pin"": [{String.Join(",", information.Leds.Pin)}],
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

        public static string CreateRootResponse(WLedRootResponse expected)
        {
            return $@"{{
  ""state"": {JsonBuilder.CreateStateJson(expected.State)},
  ""info"": {JsonBuilder.CreateInformationJson(expected.Information)},
  ""effects"": [
    {String.Join(", ", expected.Effects.Select(x => $@"""{x}"""))}
  ],
  ""palettes"": [
    {String.Join(", ", expected.Palettes.Select(x => $@"""{x}"""))}
  ]
}}";
        }
    }
}