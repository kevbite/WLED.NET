using System.Text.Json.Serialization;

namespace Kevsoft.WLED
{
    public class WLedRootResponse
    {
        /// <summary>
        /// Current state of the light. All values may be modified by the client
        /// </summary>
        [JsonPropertyName("state")]
        public State State { get; set; } = null!;

        /// <summary>
        /// General information about the device. No value can be modified using this API.
        /// </summary>
        [JsonPropertyName("info")]
        public Information Information { get; set; } = null!;

        /// <summary>
        /// An array of the effect mode names.
        /// </summary>
        [JsonPropertyName("effects")]
        public string[] Effects { get; set; } = null!;

        /// <summary>
        /// An array of the palette names
        /// </summary>
        [JsonPropertyName("palettes")]
        public string[] Palettes { get; set; } = null!;
    }
}