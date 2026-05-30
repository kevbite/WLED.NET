# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

This release is a substantial, breaking overhaul that reworks the library to cover the full
WLED JSON API with a strongly-typed, hard-to-misuse design. Legacy members were removed
rather than deprecated, so consuming code must be updated.

### Added

- **Multi-targeting** for `net8.0`, `net9.0`, `net10.0` and `netstandard2.0`.
- **Strong value types and enums** for colours (`RgbColor`, `RgbwColor`, `Color`),
  brightness/relative adjustments (`ByteAdjust`), toggles (`Toggleable`), selectors and more.
- **Fluent builders** for state updates (`UpdateState`), segments, playlists and individual LEDs.
- **Intent methods**: `TurnOn`, `TurnOff`, `Toggle`, `SetBrightness`, `SetColor` (RGB/RGBW),
  `SetEffect`, `SetPalette` and `Reboot`.
- **Presets**: read, apply, save and delete (`GetPresets`, `ApplyPreset`, `SavePreset`, `DeletePreset`).
- **Playlists**: read, start and save (`GetPlaylists`, `StartPlaylist`, `SavePlaylist`).
- **Individual LED control** with automatic, sequential request chunking (`SetIndividualLeds`).
- **Effect metadata** parsing from `/json/fxdata` (`GetEffectMetadata`).
- **Node discovery** via `/json/nodes` (`GetNodes`).
- **Device configuration** read and safe partial writes via `/json/cfg`
  (`GetConfig`, `UpdateConfig`); network/access-point changes require explicit opt-in.
- **Typed exception hierarchy**: `WledException`, `WledConnectionException`,
  `WledResponseException` (with `StatusCode`/`Body`) and `WledUnsupportedVersionException`.
- **`CancellationToken`** support on every asynchronous method.
- **Dependency-injection integration** in a new `WLED.DependencyInjection` package via
  `services.AddWledClient(...)`, backed by `IHttpClientFactory`.
- A new `WLedClient(HttpClient)` constructor for DI / `IHttpClientFactory` scenarios.

### Changed

- Requests and responses are now modelled as separate immutable response types and mutable
  request types, preventing accidental round-tripping of read-only fields.
- Posting state is now done through intent methods or `UpdateState(...)` rather than mutating
  and re-posting a response object.
- **`SetColor`/`SetEffect`/`SetPalette` with no `segmentId` now target the *selected* segments**
  (the WLED `"seg":{…}` object form) instead of segment 0. The state `seg` field is modelled as
  a `SegmentPayload` union that serialises as either an object (selected segments) or an array
  (id-targeted).
- **Transition values (`transition`/`tt`) widened from `byte` to `ushort`** to support the
  documented `0–65535` range (~109 minutes) instead of clamping at 25.5 s.
- **`ColorTemperature.Kelvin` range widened to `1000–20000 K`** to match the docs' forward-
  compatible guidance, with a new `ColorTemperature.KelvinUnchecked(int)` escape hatch for
  values outside that range.

### Added

- **Ranged-random effect/palette selection** via `Selector.RandomInRange(from, to)`
  (the WLED `"from~tor"` token).
- **Typed device-configuration fields** for `id.mdns`, `if.mqtt` (`en`/`broker`/`port`/`user`/`cid`)
  and `def` (`on`/`bri`/`ps`), while preserving all other keys through `JsonExtensionData`.

### Removed

- Legacy members were removed outright (no `[Obsolete]` shims). Update to the new API surface.
