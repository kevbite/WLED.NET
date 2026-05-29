# Plan 10 — Configuration API (`/json/cfg`) & node discovery (`/json/nodes`)

**Theme:** Feature · **Depends on:** Plans 1, 5

This plan covers the two remaining read/write endpoints: device configuration and LAN
node discovery. They're grouped because both are self-contained and lower-traffic than the
state/segment work.

---

## Part A — Node discovery (`GET /json/nodes`)

### Why
Lets an app find other WLED devices on the network (since 0.12.0). Pure read, easy win.

### Model (cf. `paul-fornage` `nodes.rs`)

```csharp
public sealed record WledNode(
    string Name,
    string Ip,            // "ip"
    string Type,          // "type" (board type id) → optional NodeType enum overlay
    int BuildId,          // "vid"
    string? MacAddress);  // when present
```

### Client method
```csharp
Task<IReadOnlyList<WledNode>> GetNodes(CancellationToken ct = default);
```
Response is `{ "nodes": [ ... ] }`; map to the list. Empty/`-1` discovery → empty list.

### Tests
Parse a real `/json/nodes` payload; empty payload → empty list.

---

## Part B — Configuration (`GET`/`POST /json/cfg`)

### Why
`/json/cfg` exposes the full device configuration (Wi-Fi, hardware/LED bus setup, sync,
time, usermods, security, etc.). It's the largest and most safety-critical surface: a bad
write can knock a device off the network. So ergonomics here means **typed sections,
partial updates, and guard rails**, not a single opaque blob.

### Strategy (phased, because `cfg` is huge)

1. **Phase 1 — typed read, safe partial write.** Model the well-documented top-level
   sections as nested types, but keep a `JsonExtensionData` catch-all so unknown/firmware-
   specific keys round-trip losslessly (critical — never drop config you don't understand).

   ```csharp
   public sealed class DeviceConfig
   {
       [JsonPropertyName("id")]  public IdentityConfig? Identity { get; init; }   // name, etc.
       [JsonPropertyName("nw")]  public NetworkConfig? Network { get; init; }
       [JsonPropertyName("ap")]  public AccessPointConfig? AccessPoint { get; init; }
       [JsonPropertyName("hw")]  public HardwareConfig? Hardware { get; init; }   // LED buses
       [JsonPropertyName("if")]  public InterfacesConfig? Interfaces { get; init; }// sync/MQTT/etc.
       [JsonPropertyName("light")] public LightConfig? Light { get; init; }
       [JsonPropertyName("def")] public DefaultsConfig? Defaults { get; init; }
       [JsonExtensionData] public Dictionary<string, JsonElement> Unknown { get; init; }
   }
   ```

2. **Phase 2 — deepen high-value sections** (LED bus layout under `hw.led.ins`, sync
   under `if.sync`, time under `if.ntp`) into fully typed models as demand warrants.

   Use `paul-fornage` `src/structures/cfg/*` as the authoritative field reference (it's
   the most complete public mapping of `cfg`).

### Client methods

```csharp
Task<DeviceConfig> GetConfig(CancellationToken ct = default);
Task UpdateConfig(DeviceConfig partial, CancellationToken ct = default);   // POST /json/cfg
```

### Guard rails (impossible-to-brick ethos)

- `UpdateConfig` serializes only set sections (nullable sections + `WhenWritingNull`), so a
  partial update never blanks untouched config.
- Doc-comment loudly that changing `nw`/`ap` can disconnect the device; consider an opt-in
  `AllowNetworkChanges` flag on a config-update options object so a network change can't be
  sent by accident.
- Preserve `Unknown` extension data on write so firmware-specific keys survive a
  read-modify-write cycle.

### Files
- `src/Kevsoft.WLED/Config/*.cs` (sections), `DeviceConfig.cs`
- `src/Kevsoft.WLED/WledNode.cs`
- `IWLedClient` + `WLedClient`: `GetNodes`, `GetConfig`, `UpdateConfig`

### Tests
- Round-trip a real `/json/cfg` payload with `JsonExtensionData` preserving unknown keys.
- Partial `UpdateConfig` emits only the touched section.
- Network-change guard: a `nw` change without the opt-in flag throws.

## Acceptance
Nodes are discoverable as a typed list; configuration is readable as typed sections with
lossless round-tripping of unknown keys, and partial writes are safe by construction with
explicit guards around network-affecting changes.
