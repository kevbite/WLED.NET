namespace Kevsoft.WLED;

/// <summary>
/// Another WLED device discovered on the local network via <c>/json/nodes</c>.
/// </summary>
public sealed class WledNode
{
    /// <summary>The friendly name of the device.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>The device IP address.</summary>
    [JsonPropertyName("ip")]
    public string IpAddress { get; set; } = null!;

    /// <summary>The board/hardware type identifier.</summary>
    [JsonPropertyName("type")]
    public int BoardType { get; set; }

    /// <summary>The build id (<c>vid</c>, format YYMMDDB).</summary>
    [JsonPropertyName("vid")]
    public uint BuildId { get; set; }

    /// <summary>Seconds since the node was last seen.</summary>
    [JsonPropertyName("age")]
    public int Age { get; set; }
}

/// <summary>Wrapper for the <c>/json/nodes</c> response.</summary>
internal sealed class NodesResponse
{
    [JsonPropertyName("nodes")]
    public WledNode[] Nodes { get; set; } = Array.Empty<WledNode>();
}
