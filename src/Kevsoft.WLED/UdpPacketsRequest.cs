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

    public static UdpPacketsRequest From(UdpPacketsResponse udpPacketsResponse)
    {
        return new UdpPacketsRequest
        {
            Send = udpPacketsResponse.Send,
            Receive = udpPacketsResponse.Receive
        };
    }

    public static implicit operator UdpPacketsRequest(UdpPacketsResponse rhs) => From(rhs);
}