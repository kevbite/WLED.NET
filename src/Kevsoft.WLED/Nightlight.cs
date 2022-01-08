using System.Text.Json.Serialization;

namespace Kevsoft.WLED
{
    /// <summary>
    /// Nightlight
    /// </summary>
    public class Nightlight
    {
        /// <summary>
        /// Nightlight currently active.
        /// </summary>
        [JsonPropertyName("on")]
        public bool On { get; set; }

        /// <summary>
        /// Duration of nightlight in minutes
        /// </summary>
        [JsonPropertyName("dur")]
        public int Duration { get; set; }

        /// <summary>
        /// Nightlight mode (0: instant, 1: fade, 2: color fade, 3: sunrise) (available since 0.10.2).
        /// </summary>
        [JsonPropertyName("mode")]
        public byte Mode { get; set; }

        /// <summary>
        /// Target brightness.
        /// </summary>
        [JsonPropertyName("tbri")]
        public int TargetBrightness { get; set; }
    }
}