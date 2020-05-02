using System.Text.Json.Serialization;

namespace Kevsoft.WLED
{
    public class Seg
    {
        /// <summary>
        /// LED the segment starts at.
        /// </summary>
        [JsonPropertyName("start")]
        public int Start { get; set; }

        /// <summary>
        /// LED the segment stops at, not included in range. If stop is set to a lower or equal value than start (setting to 0 is recommended), the segment is invalidated and deleted.
        /// </summary>
        [JsonPropertyName("stop")]
        public int Stop { get; set; }

        /// <summary>
        /// Length of the segment (stop - start). stop has preference, so if it is included, len is ignored.
        /// </summary>
        [JsonPropertyName("len")]
        public int Length { get; set; }

        /// <summary>
        /// Array that has up to 3 color arrays as elements, the primary, secondary (background) and tertiary colors of the segment. Each color is an array of 3 or 4 bytes, which represent an RGB(W) color.
        /// </summary>
        [JsonPropertyName("col")]
        public int[][] Colors { get; set; } = null!;

        /// <summary>
        /// ID of the effect.
        /// </summary>
        [JsonPropertyName("fx")]
        public int EffectId { get; set; }

        /// <summary>
        /// Relative effect speed
        /// </summary>
        [JsonPropertyName("sx")]
        public int EffectSpeed { get; set; }

        [JsonPropertyName("ix")]
        public int EffectIntensity { get; set; }

        /// <summary>
        /// ID of the color palette
        /// </summary>
        [JsonPropertyName("pal")]
        public int ColorPaletteId { get; set; }

        /// <summary>
        /// true if the segment is selected. Selected segments will have their state (color/FX) updated by APIs that don't support segments.
        /// </summary>
        [JsonPropertyName("sel")]
        public bool Selected { get; set; }

        /// <summary>
        /// Flips the segment, causing animations to change direction.
        /// </summary>
        [JsonPropertyName("rev")]
        public bool Reverse { get; set; }
    }
}