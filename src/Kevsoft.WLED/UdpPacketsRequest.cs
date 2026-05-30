namespace Kevsoft.WLED;

public sealed class UdpPacketsRequest
{
    /// <inheritdoc cref="UdpPacketsResponse.Send"/>
    [JsonPropertyName("send")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Send { get; set; }

    /// <inheritdoc cref="UdpPacketsResponse.Receive"/>
    [JsonPropertyName("recv")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Receive { get; set; }

    /// <inheritdoc cref="UdpPacketsResponse.SendGroups"/>
    [JsonPropertyName("sgrp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SyncGroup? SendGroups { get; set; }

    /// <inheritdoc cref="UdpPacketsResponse.ReceiveGroups"/>
    [JsonPropertyName("rgrp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SyncGroup? ReceiveGroups { get; set; }

    /// <summary>
    /// Suppress sending a broadcast packet for this call only (the <c>nn</c> field).
    /// </summary>
    [JsonPropertyName("nn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? NoNotify { get; set; }

    public static UdpPacketsRequest From(UdpPacketsResponse udpPacketsResponse)
    {
        return new UdpPacketsRequest
        {
            Send = udpPacketsResponse.Send,
            Receive = udpPacketsResponse.Receive,
            SendGroups = udpPacketsResponse.SendGroups,
            ReceiveGroups = udpPacketsResponse.ReceiveGroups
        };
    }

    public static implicit operator UdpPacketsRequest(UdpPacketsResponse rhs) => From(rhs);
}