namespace Kevsoft.WLED;

/// <summary>
/// Base for a device configuration section. Any keys not explicitly modelled are preserved in
/// <see cref="Unknown"/> so a read-modify-write cycle never drops firmware-specific configuration.
/// </summary>
public abstract class ConfigSection
{
    /// <summary>Configuration keys within this section that are not explicitly modelled.</summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> Unknown { get; set; } = new();
}

/// <summary>Device identity configuration (<c>cfg.id</c>).</summary>
public sealed class IdentityConfig : ConfigSection
{
    /// <summary>The friendly device name.</summary>
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    /// <summary>The mDNS hostname (the <c>.local</c> address), without the domain suffix.</summary>
    [JsonPropertyName("mdns")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MdnsName { get; set; }
}

/// <summary>Network/Wi-Fi client configuration (<c>cfg.nw</c>). Changing this can disconnect the device.</summary>
public sealed class NetworkConfig : ConfigSection
{
}

/// <summary>Access-point configuration (<c>cfg.ap</c>). Changing this can disconnect the device.</summary>
public sealed class AccessPointConfig : ConfigSection
{
}

/// <summary>Hardware configuration including LED bus layout (<c>cfg.hw</c>).</summary>
public sealed class HardwareConfig : ConfigSection
{
}

/// <summary>Interface configuration such as sync, MQTT and time (<c>cfg.if</c>).</summary>
public sealed class InterfacesConfig : ConfigSection
{
    /// <summary>MQTT configuration (<c>mqtt</c>).</summary>
    [JsonPropertyName("mqtt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MqttConfig? Mqtt { get; set; }
}

/// <summary>MQTT broker configuration (<c>cfg.if.mqtt</c>).</summary>
public sealed class MqttConfig : ConfigSection
{
    /// <summary>Whether the MQTT integration is enabled.</summary>
    [JsonPropertyName("en")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Enabled { get; set; }

    /// <summary>The MQTT broker host.</summary>
    [JsonPropertyName("broker")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Broker { get; set; }

    /// <summary>The MQTT broker port.</summary>
    [JsonPropertyName("port")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Port { get; set; }

    /// <summary>The MQTT username.</summary>
    [JsonPropertyName("user")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? User { get; set; }

    /// <summary>The MQTT client id.</summary>
    [JsonPropertyName("cid")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ClientId { get; set; }
}

/// <summary>Light/behaviour configuration (<c>cfg.light</c>).</summary>
public sealed class LightConfig : ConfigSection
{
}

/// <summary>Boot default configuration (<c>cfg.def</c>).</summary>
public sealed class DefaultsConfig : ConfigSection
{
    /// <summary>The on/off state applied at boot.</summary>
    [JsonPropertyName("on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? On { get; set; }

    /// <summary>The brightness applied at boot (0–255).</summary>
    [JsonPropertyName("bri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte? Brightness { get; set; }

    /// <summary>The preset id applied at boot.</summary>
    [JsonPropertyName("ps")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PresetId { get; set; }
}

/// <summary>
/// The device configuration exposed by <c>/json/cfg</c>.
/// </summary>
/// <remarks>
/// Only the sections you set are serialised, so a partial update never blanks untouched configuration.
/// Unknown top-level keys round-trip losslessly through <see cref="Unknown"/>.
/// </remarks>
public sealed class DeviceConfig
{
    /// <summary>Identity configuration (<c>id</c>).</summary>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IdentityConfig? Identity { get; set; }

    /// <summary>Network/Wi-Fi client configuration (<c>nw</c>).</summary>
    [JsonPropertyName("nw")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public NetworkConfig? Network { get; set; }

    /// <summary>Access-point configuration (<c>ap</c>).</summary>
    [JsonPropertyName("ap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AccessPointConfig? AccessPoint { get; set; }

    /// <summary>Hardware configuration (<c>hw</c>).</summary>
    [JsonPropertyName("hw")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public HardwareConfig? Hardware { get; set; }

    /// <summary>Interface configuration (<c>if</c>).</summary>
    [JsonPropertyName("if")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InterfacesConfig? Interfaces { get; set; }

    /// <summary>Light/behaviour configuration (<c>light</c>).</summary>
    [JsonPropertyName("light")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public LightConfig? Light { get; set; }

    /// <summary>Boot default configuration (<c>def</c>).</summary>
    [JsonPropertyName("def")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DefaultsConfig? Defaults { get; set; }

    /// <summary>Top-level configuration keys that are not explicitly modelled.</summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> Unknown { get; set; } = new();
}
