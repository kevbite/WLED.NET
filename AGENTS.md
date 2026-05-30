# AGENTS.md — WLED.NET

Guidance for coding agents (and humans) working in this repository. Read this before
making changes; it captures the project's purpose, conventions, commands and gotchas so a
change can be made safely without prior conversation history.

## Project purpose

**WLED.NET** is a strongly typed, hard-to-misuse .NET client for the
[WLED JSON API](https://kno.wled.ge/interfaces/json-api/). It turns the device's loosely
typed JSON (`/json`, `/json/state`, `/json/info`, `/json/eff`, `/json/pal`, `/json/cfg`,
`/json/fxdata`, `/json/nodes`, `presets.json`, …) into expressive C# types.

## Core design principle

> Model things well so they're easy to use. Make it impossible to send the wrong
> information by how we model the library.

Concretely:

- **Types over primitives.** Prefer value types, enums and command types over raw
  `int`/`byte`/magic strings (`RgbColor`, `SegmentColors`, `Selector`, `ByteAdjust`,
  `Toggleable`, `LightCapability` `[Flags]`, the `*Id`/`*Bounds` value types, …).
- **Validation at construction.** Where WLED documents a range (e.g. `c3` is 0–31, `bri`
  is 0–255, ledmap is 0–9), the constructing type guards it so an out-of-range value
  throws *before* it hits the wire.
- **Read model ≠ write model.** Responses are immutable/total; requests/builders expose
  only what is settable.
- **Builders, not nullable bags.** High-level fluent builders (`StateUpdate`,
  `SegmentUpdate`, `PlaylistBuilder`, `ConfigUpdate`) sit on top of the raw DTOs.

## Repository conventions

- **Dual DTO layer.** Each JSON object is a pair: an immutable `XResponse` (all fields,
  non-null) and a sparse mutable `XRequest` (nullable fields with
  `[JsonIgnore(WhenWritingNull)]`), usually with a static `Request.From(Response)` factory
  and an implicit operator. The wire format never leaks into the public happy path.
- **`[JsonPropertyName]` always carries the raw WLED key**; the C# member uses a
  descriptive .NET name (e.g. `EffectId` → `"fx"`).
- **Every new endpoint** adds a method to `IWLedClient` + `WLedClient`, with GET/POST
  tests in `test/Kevsoft.WLED.Tests` (extend `JsonBuilder` and `MockHttpMessageHandler`).
- **Custom `JsonConverter`s** carry the "impossible to misuse" types across the wire and
  are unit-tested in both directions against realistic WLED payloads.
- **Unknown keys round-trip.** Config sections use `[JsonExtensionData]` (`Unknown`) so a
  read-modify-write cycle never drops firmware-specific fields.

## Target frameworks & netstandard2.0 constraints

- The library multi-targets `netstandard2.0;net8.0;net9.0;net10.0`
  (see `Directory.Build.props`). Tests run on `net8.0;net9.0;net10.0`.
- Because of `netstandard2.0`, **do not** use:
  - `Math.Clamp` — clamp manually.
  - `System.HashCode` — implement `GetHashCode()` with `unchecked` arithmetic.
  - newer async/Span API shapes that aren't available there.
- Prefer `readonly struct` + `IEquatable<T>` for small value types.

## Commands

```pwsh
dotnet build WLED.NET.sln -c Release
dotnet test  WLED.NET.sln -c Release
```

## Breaking-change stance

- No `[Obsolete]` compatibility shims unless explicitly requested.
- Record user-facing and breaking changes in `CHANGELOG.md`.
- Keep raw `XResponse`/`XRequest` DTOs available as escape hatches even when adding
  ergonomic builders/value types on top.

## Docs & sample expectations (acceptance criteria)

A feature isn't done when it compiles and tests pass. Also:

1. Update the root `README.md` feature matrix and add/refresh a short usage snippet.
2. Keep `samples/BasicConsole` exemplary — demonstrate the *ergonomic* path (builders,
   intent methods, catalogs, snapshots), not raw DTOs.
3. Update `CHANGELOG.md`.

## Current gotchas

- **`seg` object vs array form.** `"seg": { … }` targets the *selected* segments (no id);
  `"seg": [{ "id": N, … }]` targets explicit ids. WLED infers `id:0` for an array entry
  that omits its id, so the object form is the only way to address "the selected
  segments". Use `SegmentPayload.Selected`/`List`, `StateUpdate.SelectedSegments(...)` for
  the object form and `StateUpdate.Segment(id, ...)` for the array form. Mixing the two in
  one update throws.
- **`transition`/`tt` are `ushort`** (0–65535, one unit = 100ms).
- **Reserved effects/palettes.** Entries named `RSVD` or `-` are placeholders that fall
  back to Solid; filter them out of UI. `EffectCatalog`/`PaletteCatalog` expose
  `IsReserved`/`AvailableOnly()` for this.
- **Effect metadata** (`/json/fxdata`) describes which controls each effect uses and their
  defaults; `EffectMetadata` is aligned by id with the (reserved-filtered) effects list.
- **`GetDevice()`** is the recommended read path: it issues a single `GET /json` and
  exposes state, info and effect/palette catalogs as one coherent snapshot.

## Reference material

- WLED JSON API docs: <https://kno.wled.ge/interfaces/json-api/>
- This repo's `plans/` folder contains the staged roadmap (Plans 0–13) and the conventions
  every plan must uphold.
