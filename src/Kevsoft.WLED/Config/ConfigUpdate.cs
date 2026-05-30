namespace Kevsoft.WLED;

/// <summary>
/// A fluent builder for partial device configuration updates. Only the sections you touch are
/// emitted, so an update never blanks configuration you did not set.
/// </summary>
public sealed class ConfigUpdate
{
    private readonly DeviceConfig _config = new();

    /// <summary>Set device identity values (<c>cfg.id</c>).</summary>
    public ConfigUpdate Identity(string? name = null, string? mdnsName = null)
    {
        var identity = _config.Identity ??= new IdentityConfig();

        if (name is not null)
        {
            identity.Name = name;
        }

        if (mdnsName is not null)
        {
            identity.MdnsName = mdnsName;
        }

        return this;
    }

    /// <summary>Set MQTT integration values (<c>cfg.if.mqtt</c>).</summary>
    public ConfigUpdate Mqtt(
        bool? enabled = null,
        string? broker = null,
        int? port = null,
        string? user = null,
        string? clientId = null)
    {
        var interfaces = _config.Interfaces ??= new InterfacesConfig();
        var mqtt = interfaces.Mqtt ??= new MqttConfig();

        if (enabled is not null)
        {
            mqtt.Enabled = enabled;
        }

        if (broker is not null)
        {
            mqtt.Broker = broker;
        }

        if (port is not null)
        {
            mqtt.Port = port;
        }

        if (user is not null)
        {
            mqtt.User = user;
        }

        if (clientId is not null)
        {
            mqtt.ClientId = clientId;
        }

        return this;
    }

    /// <summary>Set boot-time defaults (<c>cfg.def</c>).</summary>
    public ConfigUpdate BootDefaults(bool? on = null, byte? brightness = null, int? presetId = null)
    {
        var defaults = _config.Defaults ??= new DefaultsConfig();

        if (on is not null)
        {
            defaults.On = on;
        }

        if (brightness is not null)
        {
            defaults.Brightness = brightness;
        }

        if (presetId is not null)
        {
            defaults.PresetId = presetId;
        }

        return this;
    }

    /// <summary>Builds the <see cref="DeviceConfig"/> represented by this update.</summary>
    public DeviceConfig Build() => _config;
}
