# Plan 6 — Presets API

**Theme:** Feature · **Depends on:** Plans 1–4

## Why

Presets are one of WLED's headline features and are completely absent today. The JSON API
exposes presets through a mix of state-object command keys (`psave`, `ps`, `pdel`, `sb`,
`ib`, `sc`) and the `presets.json` file (readable via `/presets.json`). We want a clean,
intention-revealing API that hides this awkwardness.

## API surface (WLED)

- **Apply** a preset: `{"ps": <id>}` (also `"1~6~"` cycle / `"4~10r"` random — Plan 2
  `PresetSelector`).
- **Save** current state to a slot: `{"psave": <id>, "n":"name", "ql":"label",
  "sb":bool, "ib":bool, "sc":bool}` (segment-bounds / include-brightness /
  selected-segments flags).
- **Delete**: `{"pdel": <id>}`.
- **Read** all presets: `GET /presets.json` → object keyed by id, where each value is a
  preset *or* a playlist (a preset whose `playlist.ps` is non-empty). `frenck` splits
  these in `Device.__pre_deserialize__` — we do the same.

## What we build

### Read model

```csharp
public sealed record Preset(
    int Id,
    string Name,
    string? QuickLabel,
    bool On,
    int MainSegmentId,
    IReadOnlyList<SegmentResponse> Segments);
```

`GET /presets.json` is parsed into `IReadOnlyDictionary<int, Preset>` (playlists filtered
out into Plan 7's model). Preset `0` is dropped (it's the scratch slot).

### Client methods (intention-revealing, no magic keys)

```csharp
Task<IReadOnlyDictionary<int, Preset>> GetPresets(CancellationToken ct = default);
Task ApplyPreset(PresetSelector preset, CancellationToken ct = default);
Task SavePreset(int id, SavePresetOptions? options = null, CancellationToken ct = default);
Task DeletePreset(int id, CancellationToken ct = default);
```

```csharp
public sealed class SavePresetOptions
{
    public string? Name { get; init; }
    public string? QuickLabel { get; init; }
    public bool SaveSegmentBounds { get; init; } = true;   // sb
    public bool IncludeBrightness { get; init; } = true;   // ib
    public bool SaveSelectedSegments { get; init; } = true;// sc
}
```

Internally these compose `StateRequest` (`Ps`, `Psave`, `Pdel`, plus the new `n`/`ql`/
`sb`/`ib`/`sc` request fields) — so presets ride the existing serialization layer.
`ApplyPreset` takes a `PresetSelector`, making cycle/random first-class and impossible to
mistype.

> Note: saving a preset persists the device's *current* live state. Document this clearly
> (a frequent source of confusion) and offer `SavePresetFrom(StateUpdate, ...)` later if
> WLED's "save arbitrary state" path is confirmed.

## Files

- `src/Kevsoft.WLED/Preset.cs`, `SavePresetOptions.cs`
- Add `Psave`/`Pdel`/`n`/`ql`/`sb`/`ib`/`sc` to `StateRequest.cs`
- `IWLedClient` + `WLedClient`: the four methods above
- `src/Kevsoft.WLED/Json/PresetsConverter.cs` (split presets vs playlists)

## Tests

- Parse a real `presets.json` containing both presets and a playlist; assert playlists
  are excluded and slot `0` dropped.
- `ApplyPreset(PresetSelector.Cycle(1,6))` → `{"ps":"1~6~"}`.
- `SavePreset(3, new(){Name="X"})` → `{"psave":3,"n":"X","sb":true,...}`.
- `DeletePreset(3)` → `{"pdel":3}`.

## Acceptance

Callers can list, apply (incl. cycle/random), save (with documented flags), and delete
presets through dedicated methods, never touching `psave`/`pdel`/`ps` directly.
