namespace Kevsoft.WLED;

/// <summary>
/// Options controlling how a <see cref="DeviceConfig"/> update is applied.
/// </summary>
public sealed class UpdateConfigOptions
{
    /// <summary>
    /// Must be explicitly set to <c>true</c> to allow updating the network (<c>nw</c>) or
    /// access-point (<c>ap</c>) sections, which can disconnect the device. Defaults to <c>false</c>.
    /// </summary>
    public bool AllowNetworkChanges { get; set; }
}
