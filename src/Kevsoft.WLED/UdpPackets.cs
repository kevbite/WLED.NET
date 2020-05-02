using System.Text.Json.Serialization;

namespace Kevsoft.WLED
{
    public class UdpPackets
    {
        /// <summary>
        /// Send WLED broadcast (UDP sync) packet on state change
        /// </summary>
        [JsonPropertyName("send")]
        public bool Send { get; set; }

        /// <summary>
        /// Receive broadcast packets
        /// </summary>
        [JsonPropertyName("recv")]
        public bool Receive { get; set; }
    }
}