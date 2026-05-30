# Plan 3 — Complete the State object

**Theme:** Data model · **Depends on:** Plans 1, 2

## Why

`StateResponse`/`StateRequest` only map a subset of the documented state keys, and the
ones they do map use raw primitives. This plan closes the field gap *using the typed
foundations* from Plans 1–2 so the state surface is both complete and safe.

## Current vs. API

Mapped today: `on`, `bri`, `transition`, `ps`, `pl`, `nl`, `udpn`, `lor`, `mainseg`,
`seg`, `tb`.

Missing / mistyped (see [JSON API → State object](https://kno.wled.ge/interfaces/json-api/)):

| Key | Meaning | Modeled as |
|-----|---------|-----------|
| `tt` | transition for *this call only* | `StateUpdate.TransitionOnce(TimeSpan)` (write-only) |
| `psave` | save current state to preset slot | Plan 6 (write-only command) |
| `sb`,`ib`,`sc` | what to save with `psave` | Plan 6 |
| `pdel` | delete preset id | Plan 6 |
| `nl.rem` | remaining nightlight seconds (read-only) | `Nightlight.Remaining` (`int?`, -1→null) |
| `nl.mode` | already `byte` → use `NightlightMode` enum (Plan 1) | enum |
| `udpn.sgrp`/`rgrp` | send/receive sync groups | `SyncGroup` `[Flags]` (Plan 1) |
| `udpn.nn` | suppress broadcast for this call (write-only) | `UdpSyncUpdate.NoNotify` |
| `lor` | live data override → `LiveDataOverride` enum (Plan 1) | enum |
| `v` | echo full state in POST response | client option (Plan 11) |
| `rb` | reboot now (write-only) | `client.Reboot()` (Plan 11) |
| `live` | enter realtime/blank (write-only) | `StateUpdate.EnterLiveMode(bool)` |
| `time` | set device unix time (write-only) | `StateUpdate.SetTime(DateTimeOffset)` |
| `playlist` | inline playlist | Plan 7 |
| `ledmap` | load ledmap 0–9 (write-only) | `StateUpdate.LoadLedMap(byte)` w/ 0–9 guard |
| `rmcpal` | remove last custom palette (write-only) | `StateUpdate.RemoveLastCustomPalette()` |
| `np` | advance to next preset in playlist (write-only) | `StateUpdate.NextPreset()` |
| `mainseg` | already mapped | keep |

## What we build

### Response (read model) — additions to `StateResponse`

```csharp
public LiveDataOverride LiveDataOverride { get; init; }   // was byte
public byte LedMap { get; init; }                          // ledmap
public int? PresetId { get; init; }                        // -1 → null
public int? PlaylistId { get; init; }                      // -1 → null
```

`NightlightResponse` gains `Remaining` (`int?`) and `Mode` becomes `NightlightMode`.
`UdpPacketsResponse` gains `SendGroups`/`ReceiveGroups` (`SyncGroup`).

### Write model — fold the write-only commands into `StateUpdate` (Plan 2)

```csharp
client.UpdateState(s => s
    .EnterLiveMode()
    .SetTime(DateTimeOffset.UtcNow)
    .LoadLedMap(2)          // throws if > 9
    .NextPreset());
```

Write-only keys (`tt`, `rb`, `live`, `time`, `ledmap`, `rmcpal`, `np`, `udpn.nn`) appear
**only** on the builder/`Request`, never on the response — enforcing read/write split.

## Files

- Edit `StateResponse.cs`, `StateRequest.cs`, `NightlightResponse/Request.cs`,
  `UdpPacketsResponse/Request.cs`
- Extend `Fluent/StateUpdate.cs` (Plan 2) with the write-only commands

## Tests

- Round-trip a full real `/json/state` sample (grab one from a device or the docs) and
  assert every documented key maps.
- `-1` preset/playlist → `null`; `nl.rem = -1` → `null`.
- Write-only commands serialize correctly and are absent from responses.
- Extend `JsonBuilder.CreateStateJson` to include the new keys.

## Acceptance

`StateResponse` represents every readable state key with a correct type, and every
writable/command-only key is reachable through `StateUpdate` with range guards. No bare
`byte` "modes" remain on state.
