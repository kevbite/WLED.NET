# Plan 13 — Device ergonomics, catalogs & agent guidance

**Theme:** Usability · Developer experience · Agent readiness

## Why

After implementing the API coverage and correctness work in Plans 0–12, the next best
upgrade is not broad endpoint expansion. The remaining usability gap is that consumers
still need to stitch together state, info, effects, palettes, metadata, and config pieces
themselves.

This plan focuses on making the library feel like a cohesive .NET SDK:

- model a WLED device as a first-class aggregate,
- make effects and palettes discoverable by id/name rather than raw arrays,
- expose selected-segment updates ergonomically,
- make effect metadata actionable,
- reduce raw primitive ids/ranges where they invite mistakes, and
- add a root `AGENTS.md` so future coding agents have the project rules, commands, and
  gotchas in one place.

New endpoints are only included if they directly improve ergonomics.

---

## 1. Create root `AGENTS.md`

Add `AGENTS.md` at the repository root with the information agents need before changing
this project.

It should include:

- **Project purpose:** WLED.NET is a strongly typed, hard-to-misuse .NET wrapper for the
  WLED JSON API.
- **Design principle:** prefer expressive types/builders over primitive DTO bags; make
  invalid states hard or impossible to represent.
- **Target frameworks:** library multi-targets `netstandard2.0;net8.0;net9.0;net10.0`;
  tests run on net8/net9/net10. Mention netstandard2.0 constraints such as no
  `Math.Clamp`, no `System.HashCode`, and older async API shapes.
- **Commands:** `dotnet build WLED.NET.sln -c Release` and
  `dotnet test WLED.NET.sln -c Release`.
- **Repository conventions:** response DTOs are total/immutable where practical; request
  DTOs are sparse and omit nulls; raw WLED JSON keys stay in `[JsonPropertyName]`; public
  happy paths use builders/value types.
- **Breaking-change stance:** no `[Obsolete]` compatibility shims unless explicitly
  requested; document breaking changes in `CHANGELOG.md`.
- **Docs/sample expectations:** README feature matrix, CHANGELOG, and
  `samples/BasicConsole` stay current with public API changes.
- **Current gotchas:** `seg` object form targets selected segments; `seg` array form
  targets explicit segment ids; `transition`/`tt` are `ushort`; `JsonExtensionData`
  preserves unknown config fields.

Definition of Done:

- `AGENTS.md` exists in the root and is specific enough that a new agent can safely make
  changes without reading prior conversation history.
- It references the WLED JSON API docs and this `plans/` folder.
- It does not contain secrets, environment-specific credentials, or private assumptions.

---

## 2. Add a cohesive `WLedDevice` snapshot API

Today callers can fetch each data source individually (`GetState`, `GetInformation`,
`GetEffects`, `GetPalettes`, `GetEffectMetadata`, etc.), but app code has to combine
those pieces itself.

Add a high-level read model:

```csharp
var device = await client.GetDevice();

Console.WriteLine(device.Name);
Console.WriteLine(device.ActiveSegments.Count);
Console.WriteLine(device.CurrentPresetId);

foreach (var segment in device.SelectedSegments)
{
    Console.WriteLine($"{segment.Id}: {segment.Effect.Name}");
}
```

Proposed shape:

- `WLedDevice`
  - `State`
  - `Information`
  - `Effects`
  - `Palettes`
  - optional `EffectMetadata`
  - derived helpers: `Name`, `Version`, `ActiveSegments`, `SelectedSegments`,
    `CurrentPresetId`, `CurrentPlaylistId`, `SupportsWhiteChannel`, etc.
- `WLedDeviceSegment`
  - segment id, name, selected/active flags, colors, options
  - current `EffectCatalogEntry`
  - current `PaletteCatalogEntry`

Client API:

```csharp
Task<WLedDevice> GetDevice(CancellationToken cancellationToken = default);
Task<WLedDevice> GetDevice(DeviceSnapshotOptions options, CancellationToken cancellationToken = default);
```

`DeviceSnapshotOptions` can control optional calls such as effect metadata if we want to
avoid extra network requests by default.

Definition of Done:

- One call gives consumers a coherent, easy-to-query snapshot.
- Derived properties are tested with realistic JSON.
- README shows `GetDevice()` as the recommended read path for app/UI code.

---

## 3. Add catalog-aware effect and palette APIs

The WLED docs note that effect arrays contain reserved entries such as `RSVD` and `-`.
Today callers receive raw `string[]` and need to remember ids manually.

Add typed catalog entries:

```csharp
var effect = device.Effects.FindByName("Rainbow");
await client.SetEffect(effect);

var palettes = device.Palettes.AvailableOnly();
await client.SetPalette(palettes.FindByName("Aurora"));
```

Proposed types:

- `EffectCatalog` / `PaletteCatalog`
- `EffectCatalogEntry` / `PaletteCatalogEntry`
  - `Id`
  - `Name`
  - `IsReserved`
  - possibly `IsUsable => !IsReserved`
- lookup helpers:
  - `FindById`
  - `FindByName`
  - `TryFindByName`
  - `AvailableOnly`

Client overloads:

```csharp
Task SetEffect(EffectCatalogEntry effect, int? segmentId = null, CancellationToken cancellationToken = default);
Task SetPalette(PaletteCatalogEntry palette, int? segmentId = null, CancellationToken cancellationToken = default);
```

Definition of Done:

- Reserved entries are filtered consistently.
- Name lookup is explicit about case-sensitivity (recommend ordinal ignore-case).
- Invalid lookups fail with clear exceptions, not silent defaults.
- Existing raw id APIs remain available for advanced callers.

---

## 4. Add selected-segment fluent updates

Plan 12 fixed no-id intent methods so they use WLED's selected-segment object form. The
fluent builder should expose that same concept directly.

Add:

```csharp
await client.UpdateState(update => update
    .SelectedSegments(segment => segment
        .Color(RgbColor.FromHex("FFAA00"))
        .Effect(Selector.Random)));
```

Semantics:

- `SelectedSegments(...)` serializes `seg` as an object (`"seg": { ... }`).
- `Segment(id, ...)` continues to serialize `seg` as an array with explicit ids.
- Mixing selected-segment object form and explicit segment array form in the same update
  should either be disallowed with a clear exception or require an explicit advanced API;
  do not silently pick one.

Definition of Done:

- Tests assert object form for `SelectedSegments(...)`.
- Tests assert explicit id array form for `Segment(id, ...)`.
- Tests cover mixed selected/id-targeted updates and verify clear behavior.
- README uses `SelectedSegments(...)` where it makes examples clearer.

---

## 5. Make effect metadata actionable

`/json/fxdata` is parsed, but consumers still need to understand how to map metadata to
controls. Use metadata to reduce guesswork in UI builders and state updates.

Possible APIs:

```csharp
var metadata = await client.GetEffectMetadata();
var rainbow = metadata.FindByName("Rainbow");

foreach (var control in rainbow.Controls)
{
    Console.WriteLine($"{control.Name}: {control.DefaultValue}");
}

await client.UpdateState(update => update
    .Segment(0, segment => segment
        .Effect(rainbow)
        .ApplyEffectDefaults(rainbow)));
```

Improvements:

- Add lookup helpers to effect metadata collections.
- Add stronger control models for sliders, colors, checkboxes, palettes, and options when
  the metadata describes them.
- Add `ApplyEffectDefaults(...)` to set speed/intensity/custom sliders/options from
  metadata defaults.
- Optionally expose validation helpers so app code can avoid showing unsupported controls.

Definition of Done:

- Metadata can be used without hand-parsing labels or raw arrays.
- Tests cover realistic metadata from WLED docs/fixtures.
- Invalid metadata fails explicitly where the parser cannot safely interpret it.

---

## 6. Add stronger id and range value types

The library already has strong command/value types, but public APIs still expose several
raw ids and ranges as `int`/`byte`.

Add focused value types where misuse is likely:

- `SegmentId`
- `EffectId`
- `PaletteId`
- `PresetId`
- `PlaylistId`
- `LedMapId`
- `SegmentBounds` / `MatrixBounds`

Guidelines:

- Do not introduce wrappers everywhere at once; start where ids cross API boundaries.
- Keep implicit conversions only where they do not hide validation or semantics.
- Prefer explicit factory methods when the value has a documented range or special
  meaning.

Definition of Done:

- High-level APIs can accept value types as well as existing simple values where helpful.
- Tests prove invalid ranges throw before serialization.
- Documentation shows the value types in new examples without making simple usage noisy.

---

## 7. Add a config update builder

Config support is now safer and partially typed, but callers still build nested DTOs
manually. Add a builder that encourages safe partial updates.

Example:

```csharp
await client.UpdateConfig(config => config
    .Identity(name: "Kitchen", mdnsName: "wled-kitchen")
    .Mqtt(enabled: true, broker: "mqtt.local")
    .BootDefaults(on: true, brightness: 128));
```

Proposed behavior:

- Builder only emits touched sections.
- Network-sensitive sections still require `AllowNetworkChanges`.
- Unknown/advanced config remains available through raw `DeviceConfig`.
- Builder methods validate known ranges and string requirements before sending.

Definition of Done:

- Partial config updates are easy without manually constructing DTO graphs.
- Tests assert untouched sections are omitted.
- Tests assert network-sensitive updates still require opt-in.
- README shows the builder as the recommended config update path.

---

## 8. Lower-priority follow-ups

Track these, but do not prioritize them unless a concrete user need appears:

- `ColorSlot.Fixed(Color)` / `ColorSlot.Random` once random color slot support is stable in
  WLED firmware.
- More ergonomic individual LED helpers, such as array/range/image/matrix helpers for
  larger updates.
- Flexible read-only `info.sensor` support.
- `/json/palx` custom palette support only if custom palette authoring becomes a real
  usability goal.

---

## Files

Likely files to touch:

- `AGENTS.md`
- `src/Kevsoft.WLED/IWLedClient.cs`
- `src/Kevsoft.WLED/WLedClient.cs`
- new device snapshot/catalog types under `src/Kevsoft.WLED/`
- `src/Kevsoft.WLED/Fluent/StateUpdate.cs`
- `src/Kevsoft.WLED/Fluent/SegmentUpdate.cs`
- `src/Kevsoft.WLED/EffectMetadata*.cs`
- `src/Kevsoft.WLED/Config/DeviceConfig.cs` and new config builder files
- `test/Kevsoft.WLED.Tests/`
- `README.md`
- `CHANGELOG.md`
- `samples/BasicConsole/Program.cs`

## Definition of Done

For this plan:

1. `AGENTS.md` exists and accurately captures project conventions, commands, and agent
   gotchas.
2. New ergonomic APIs are backed by tests on net8/net9/net10.
3. README and sample show the new happy paths rather than raw DTO construction.
4. CHANGELOG records user-facing changes and breaking changes.
5. The raw DTO layer remains available for advanced/escape-hatch scenarios, but the
   documented path uses builders, catalogs, snapshots, and value types.
