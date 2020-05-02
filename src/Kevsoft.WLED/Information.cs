using System.Text.Json.Serialization;

namespace Kevsoft.WLED
{
    public class Information
    {
        /// <summary>
        /// Version name.
        /// </summary>
        [JsonPropertyName("ver")]
        public string VersionName { get; set; } = null!;

        /// <summary>
        /// Build ID (YYMMDDB, B = daily build index).
        /// </summary>
        [JsonPropertyName("vid")]
        public int BuildId { get; set; }

        /// <summary>
        /// LEDs Information
        /// </summary>
        [JsonPropertyName("leds")]
        public Leds Leds { get; set; } = null!;

        /// <summary>
        /// Friendly name of the light.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The UDP port for realtime packets and WLED broadcast.
        /// </summary>
        [JsonPropertyName("udpport")]
        public int UdpPort { get; set; }

        /// <summary>
        /// If true, the software is currently receiving realtime data via UDP or E1.31.
        /// </summary>
        [JsonPropertyName("live")]
        public bool Live { get; set; }

        /// <summary>
        /// Number of effects included.
        /// </summary>
        [JsonPropertyName("fxcount")]
        public byte EffectsCount { get; set; }

        /// <summary>
        /// Number of palettes configured.
        /// </summary>
        [JsonPropertyName("palcount")]
        public ushort PalettesCount { get; set; }

        /// <summary>
        /// Name of the platform.
        /// </summary>
        [JsonPropertyName("arch")]
        public string Arch { get; set; } = null!;

        /// <summary>
        /// Version of the underlying (Arduino core) SDK.
        /// </summary>
        [JsonPropertyName("core")]
        public string Core { get; set; } = null!;

        /// <summary>
        /// Bytes of heap memory (RAM) currently available. Problematic if less than 10k.
        /// </summary>
        [JsonPropertyName("freeheap")]
        public uint FreeHeapMemory { get; set; }

        /// <summary>
        /// Time since the last boot/reset in seconds.
        /// </summary>
        [JsonPropertyName("uptime")]
        public uint UpTime { get; set; }

        /// <summary>
        /// Used for debugging purposes only.
        /// </summary>
        [JsonPropertyName("opt")]
        public ushort Opt { get; set; }

        /// <summary>
        /// The producer/vendor of the light. Always WLED for standard installations.
        /// </summary>
        [JsonPropertyName("brand")]
        public string Brand { get; set; } = null!;

        /// <summary>
        /// The product name. Always DIY light for standard installations.
        /// </summary>
        [JsonPropertyName("product")]
        public string Product { get; set; } = null!;

        /// <summary>
        /// The origin of the build. src if a release version is compiled from source, bin for an official release image, dev for a development build (regardless of src/bin origin) and exp for experimental versions. ogn if the image is flashed to hardware by the vendor.
        /// </summary>
        [JsonPropertyName("btype")]
        public string BuildType { get; set; } = null!;

        /// <summary>
        /// The hexadecimal hardware MAC address of the light, lowercase and without colons.
        /// </summary>
        [JsonPropertyName("mac")]
        public string MacAddress { get; set; } = null!;
    }
}