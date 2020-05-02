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
        /// If true, the light will gradually dim over the course of the nightlight duration. If false, it will instantly turn to the target brightness once the duration has elapsed.
        /// </summary>
        [JsonPropertyName("fade")]
        public bool Fade { get; set; }

        /// <summary>
        /// Target brightness.
        /// </summary>
        [JsonPropertyName("tbri")]
        public int TargetBrightness { get; set; }
    }
}