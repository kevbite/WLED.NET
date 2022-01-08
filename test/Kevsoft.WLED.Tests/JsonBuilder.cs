namespace Kevsoft.WLED.Tests;

public class JsonBuilder
{
    public static string CreateStateJson(StateResponse state)
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
                    ""mode"": {state.Nightlight.Mode},
                    ""tbri"": {state.Nightlight.TargetBrightness}
                    }},
                ""udpn"": {{
                    ""send"": {state.UdpPackets.Send.ToString().ToLower()},
                    ""recv"": {state.UdpPackets.Receive.ToString().ToLower()}
                    }},
                ""lor"": {state.LiveDataOverride},
                ""mainseg"": {state.MainSegment},
                ""seg"": [{String.Join(", ", state.Segments.Select(seg =>
                {
                    return $@"{{
                            ""id"": {seg.Id},
                            ""start"": {seg.Start},
                            ""stop"": {seg.Stop},
                            ""len"": {seg.Length},
                            ""grp"": {seg.Group},
                            ""spc"": {seg.Spacing},
                            ""of"": {seg.Offset},
                            ""col"": [
                              {String.Join(", ", seg.Colors.Select(col => $"[{String.Join(",", col)}]"))}
                            ],
                            ""fx"": {seg.EffectId},
                            ""sx"": {seg.EffectSpeed},
                            ""ix"": {seg.EffectIntensity},
                            ""pal"": {seg.ColorPaletteId},
                            ""sel"": {seg.Selected.ToString().ToLower()},
                            ""rev"": {seg.Reverse.ToString().ToLower()},
                            ""on"": {seg.SegmentState.ToString().ToLower()},
                            ""bri"": {seg.Brightness}
                            }}";
                }))}]
            }}";
    }

    public static string CreateInformationJson(InformationResponse information)
    {
        return $@"{{
                ""ver"": ""{information.VersionName}"",
                ""vid"": {information.BuildId},
                ""leds"": {{
                    ""count"": {information.Leds.Count},
                    ""fps"": {information.Leds.Fps},
                    ""rgbw"": {information.Leds.Rgbw.ToString().ToLower()},
                    ""wv"": {information.Leds.DisplayWhiteSlider.ToString().ToLower()},
                    ""pwr"": {information.Leds.PowerUsage},
                    ""maxpwr"": {information.Leds.MaximumPower},
                    ""maxseg"": {information.Leds.MaximumSegments}
                    }},
                ""str"": {information.ToggleSendReceive.ToString().ToLower()},
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
                ""mac"": ""{information.MacAddress}"",
                ""ip"": ""{information.NetworkAddress}""
                }}";
    }

    public static string CreateRootResponse(WLedRootResponse expected)
    {
        return $@"{{
                ""state"": {CreateStateJson(expected.State)},
                ""info"": {CreateInformationJson(expected.Information)},
                ""effects"": [
                    {String.Join(", ", expected.Effects.Select(x => $@"""{x}"""))}
                    ],
                ""palettes"": [
                    {String.Join(", ", expected.Palettes.Select(x => $@"""{x}"""))}
                    ]
                }}";
    }
}