namespace Kevsoft.WLED;

public sealed class UdpPacketsResponse
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

    /// <summary>
    /// Sync groups this device sends to.
    /// </summary>
    [JsonPropertyName("sgrp")]
    public SyncGroup SendGroups { get; set; }

    /// <summary>
    /// Sync groups this device receives from.
    /// </summary>
    [JsonPropertyName("rgrp")]
    public SyncGroup ReceiveGroups { get; set; }
}