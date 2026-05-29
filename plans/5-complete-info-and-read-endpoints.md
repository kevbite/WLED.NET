# Plan 5 — Complete Info & new read endpoints (si / net / nodes / live)

**Theme:** Read APIs · **Depends on:** Plan 1

## Why

`InformationResponse`/`LedsResponse` omit many diagnostic fields, and several read-only
endpoints aren't exposed at all. These are pure reads, so they're low-risk and high-value
for monitoring/diagnostics integrations.

## Part A — Complete `InformationResponse` / `LedsResponse`

Missing info keys (see [JSON API → Info object](https://kno.wled.ge/interfaces/json-api/)
and `frenck` `Info`/`Leds`/`Wifi`/`Filesystem`):

| Key | Modeled as |
|-----|-----------|
| `leds.rgbw` | `bool Rgbw` |
| `leds.wv` | `bool WhiteValueSlider` |
| `leds.cct` | `bool SupportsColorTemperature` |
| `leds.seglc` | `LightCapability[] SegmentLightCapabilities` (Plan 1 flags) |
| `leds.lc` | retype to `LightCapability` (Plan 1) |
| `lm` | `string LiveMode` |
| `lip` | `string LiveIp` |
| `ws` | `int? WebSocketClients` (-1 → null, unsupported) |
| `wifi` | `WifiResponse { Bssid, Signal, Channel, Rssi }` |
| `fs` | `FilesystemResponse { Used, Total, LastModified }` + computed `Free`/`FreePercentage` |
| `ndc` | `int DiscoveredDevices` |
| `cpalcount`/`umpalcount`/`umpalnames` | custom/usermod palette counts + names |

`fs.pmt` is a unix timestamp → expose as `DateTimeOffset?` (0/absent → null).
`FilesystemResponse` gets computed `Free`, `FreePercentage`, `UsedPercentage` properties
(mirrors `frenck.Filesystem`), so callers never divide by hand.

## Part B — New read endpoints

| Endpoint | Returns | New client method |
|----------|---------|-------------------|
| `GET /json/si` | `{state, info}` only (lighter than `/json`) | `Task<StateInfoResponse> GetStateInfo(CancellationToken)` |
| `GET /json/net` | nearby Wi-Fi networks (`networks[]`) | `Task<NetworkResponse[]> GetNetworks(...)` |
| `GET /json/nodes` | discovered WLED nodes on the LAN | Plan 10 *(covered there)* |
| `GET /json/live` | live LED color stream (if `WLED_ENABLE_JSONLIVE`) | `Task<LiveResponse?> GetLiveColors(...)` |

### `/json/si`

```csharp
public sealed class StateInfoResponse
{
    [JsonPropertyName("state")] public StateResponse State { get; init; }
    [JsonPropertyName("info")]  public InformationResponse Info { get; init; }
}
```

### `/json/net`

Array of `{ ssid, rssi, bssid, channel, enc }`. Map `enc` to an `WifiEncryption` enum
where the WLED values are known; otherwise keep an `int` plus enum overlay.

### `/json/live`

`{ leds: ["rrggbb", ...], n: <start>, ... }`. Expose `LiveResponse.Leds` as
`RgbColor[]` (reuse Plan 1 hex parsing). Return `null` when the build lacks JSON-live
(endpoint 404/empty) rather than throwing, so feature-detection is easy.

## Files

- Edit `InformationResponse.cs`, `LedsResponse.cs`
- `src/Kevsoft.WLED/{WifiResponse,FilesystemResponse,StateInfoResponse,NetworkResponse,LiveResponse}.cs`
- `IWLedClient` + `WLedClient`: `GetStateInfo`, `GetNetworks`, `GetLiveColors`

## Tests

- Full real `/json/info` sample round-trips incl. `wifi`, `fs`, `seglc`, `ws=-1 → null`.
- `FilesystemResponse` computed properties (`Free`, percentages) with sample numbers.
- `/json/si`, `/json/net`, `/json/live` happy-path + `/json/live` 404 → `null`.
- Extend `JsonBuilder` / add new builders; reuse `MockHttpMessageHandler` per-route.

## Acceptance

`InformationResponse` exposes the full documented info surface with typed Wi-Fi/FS/LED
capabilities, and `/json/si`, `/json/net`, `/json/live` are first-class read methods with
graceful feature-detection.
