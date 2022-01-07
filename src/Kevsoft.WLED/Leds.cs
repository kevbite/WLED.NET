using System.Text.Json.Serialization;

namespace Kevsoft.WLED
{
    public class Leds
    {
        /// <summary>
        /// Total LED count.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }

        /// <summary>
        /// Current frames per second. (available since 0.12.0)
        /// </summary>
        [JsonPropertyName("fps")]
        public byte Fps { get; set; }

        /// <summary>
        /// true if LEDs are 4-channel (RGBW).
        /// </summary>
        [JsonPropertyName("rgbw")]
        public bool Rgbw { get; set; }

        /// <summary>
        /// true if a white channel slider should be displayed. (available since 0.10.0)
        /// </summary>
        [JsonPropertyName("wv")]
        public bool DisplayWhiteSlider { get; set; }

        /// <summary>
        /// Current LED power usage in milliamps as determined by the ABL. 0 if ABL is disabled.
        /// </summary>
        [JsonPropertyName("pwr")]
        public int PowerUsage { get; set; }

        /// <summary>
        /// Maximum power budget in milliamps for the ABL. 0 if ABL is disabled.
        /// </summary>
        [JsonPropertyName("maxpwr")]
        public int MaximumPower { get; set; }

        /// <summary>
        /// Maximum number of segments supported by this version.
        /// </summary>
        [JsonPropertyName("maxseg")]
        public byte MaximumSegments { get; set; }
    }
}