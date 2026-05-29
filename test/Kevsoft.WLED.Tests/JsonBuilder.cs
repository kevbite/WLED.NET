namespace Kevsoft.WLED.Tests;

public class JsonBuilder
{
    public static string CreateStateJson(StateResponse state)
    {
        return $@"{{
                ""on"": {state.On.ToString().ToLower()},
                ""bri"": {state.Brightness},
                ""transition"": {state.Transition},
                ""ps"": {state.PresetId ?? -1},
                ""pl"": {state.PlaylistId ?? -1},
                ""nl"": {{
                    ""on"": {state.Nightlight.On.ToString().ToLower()},
                    ""dur"": {state.Nightlight.Duration},
                    ""mode"": {(byte)state.Nightlight.Mode},
                    ""tbri"": {state.Nightlight.TargetBrightness},
                    ""rem"": {state.Nightlight.Remaining ?? -1}
                    }},
                ""udpn"": {{
                    ""send"": {state.UdpPackets.Send.ToString().ToLower()},
                    ""recv"": {state.UdpPackets.Receive.ToString().ToLower()},
                    ""sgrp"": {(byte)state.UdpPackets.SendGroups},
                    ""rgrp"": {(byte)state.UdpPackets.ReceiveGroups}
                    }},
                ""lor"": {(byte)state.LiveDataOverride},
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
                              {String.Join(", ", seg.Colors.Slots.Select(col => $"[{String.Join(",", col.ToBytes())}]"))}
                            ],
                            ""fx"": {seg.EffectId},
                            ""sx"": {seg.EffectSpeed},
                            ""ix"": {seg.EffectIntensity},
                            ""pal"": {seg.ColorPaletteId},
                            ""sel"": {seg.Selected.ToString().ToLower()},
                            ""rev"": {seg.Reverse.ToString().ToLower()},
                            ""frz"": {seg.Freeze.ToString().ToLower()},
                            ""on"": {seg.SegmentState.ToString().ToLower()},
                            ""bri"": {seg.Brightness},
                            ""mi"": {seg.Mirror.ToString().ToLower()},
                            ""n"": ""{seg.Name}"",
                            ""cct"": {seg.Cct.Value},
                            ""c1"": {seg.CustomSlider1},
                            ""c2"": {seg.CustomSlider2},
                            ""c3"": {seg.CustomSlider3},
                            ""o1"": {seg.Option1.ToString().ToLower()},
                            ""o2"": {seg.Option2.ToString().ToLower()},
                            ""o3"": {seg.Option3.ToString().ToLower()},
                            ""m12"": {(byte)seg.Expand1D},
                            ""si"": {(byte)seg.SoundSimulation},
                            ""set"": {seg.Set},
                            ""cln"": {seg.Clones ?? -1},
                            ""startY"": {seg.StartY},
                            ""stopY"": {seg.StopY},
                            ""rY"": {seg.ReverseY.ToString().ToLower()},
                            ""mY"": {seg.MirrorY.ToString().ToLower()},
                            ""tp"": {seg.Transpose.ToString().ToLower()}
                            }}";
                }))}],
                ""tb"": {state.Timebase}
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
                    ""lc"": {(byte)information.Leds.LightCapabilities},
                    ""seglc"": [{String.Join(",", information.Leds.SegmentLightCapabilities.Select(x => (byte)x))}],
                    ""rgbw"": {information.Leds.Rgbw.ToString().ToLower()},
                    ""wv"": {information.Leds.WhiteValueSlider.ToString().ToLower()},
                    ""cct"": {information.Leds.SupportsColorTemperature.ToString().ToLower()},
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
                ""lm"": ""{information.LiveMode}"",
                ""lip"": ""{information.LiveIp}"",
                ""ws"": {information.WebSocketClients ?? -1},
                ""wifi"": {{
                    ""bssid"": ""{information.Wifi.Bssid}"",
                    ""signal"": {information.Wifi.Signal},
                    ""channel"": {information.Wifi.Channel}
                    }},
                ""fs"": {{
                    ""u"": {information.Filesystem.Used},
                    ""t"": {information.Filesystem.Total},
                    ""pmt"": {information.Filesystem.PresetsModifiedTimestamp}
                    }},
                ""ndc"": {information.DiscoveredDevices ?? -1},
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