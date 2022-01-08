namespace Kevsoft.WLED;

public sealed class WLedRootRequest
{
    /// <inheritdoc cref="WLedRootRequest.State" />
    [JsonPropertyName("state")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public StateRequest? State { get; set; }

    public static WLedRootRequest From(WLedRootResponse root)
    {
        return new WLedRootRequest
        {
            State = root.State
        };
    }

    public static implicit operator WLedRootRequest(WLedRootResponse rhs) => From(rhs);
}