# WLED.NET Roadmap — Full, Ergonomic JSON API Coverage

This folder contains a set of feature plans that evolve **WLED.NET** from a partial,
field-for-field wrapper into a *complete* and *hard-to-misuse* .NET client for the
[WLED JSON API](https://kno.wled.ge/interfaces/json-api/).

## Guiding principle: make wrong states unrepresentable

> "Model things well so they're easy to use. Not just a mapping of random fields —
> make it impossible to send the wrong information by how we model the library."

Every plan below is held to that bar. Concretely, that means:

- **Types over primitives.** No raw `int[][]` colors, no `byte` enums-by-convention,
  no magic strings. We introduce value types (`RgbColor`, `RgbwColor`, `SegmentColors`),
  enums (`NightlightMode`, `LiveDataOverride`, `LightCapability` `[Flags]`,
  `SyncGroup` `[Flags]`, `SoundSimulation`, `Expand1D`), and command types that can
  only hold valid values.
- **Command-value safety.** Fields that accept the WLED "toggle/increment/decrement/
  random" mini-language (`"t"`, `~`, `~-`, `~10`, `"r"`, `"1~6~"`) get dedicated types
  (e.g. `Toggleable`, `Adjust`, `EffectSelector`) so a caller *cannot* hand-write an
  invalid command string.
- **Builders, not nullable bags.** High-level fluent builders (`StateUpdate`,
  `SegmentUpdate`) replace manually newing up DTOs full of nullable properties.
  The raw `Request`/`Response` DTOs remain as the serialization layer underneath.
- **Validation at the boundary of construction.** Where the API has documented ranges
  (e.g. `c3` is 0–31, `bri` is 0–255, `cct` is 0–255 or 1900–10091), the constructing
  type guards the range so an out-of-range value throws *before* it ever hits the wire.
- **Read model ≠ write model.** Responses are immutable and total; requests/builders
  expose only what is actually settable.

## Where the library is today

Supported: `GET /json`, `/json/state`, `/json/info`, `/json/eff`, `/json/pal`;
`POST /json`, `/json/state`. The mapped objects cover only a subset of fields, expose
raw primitives (`int[][]` colors, `byte` "enums", `int` preset/playlist ids), and offer
no presets, playlists, per-LED control, effect metadata, discovery, or config support.

See each plan for the precise gap it closes.

## Reference implementations

These plans cross-reference two mature community libraries (linked by the WLED docs):

- **[paul-fornage/wled-json-api-library](https://github.com/paul-fornage/wled-json-api-library)**
  (Rust) — the most complete, documented JSON structure, including `/json/cfg`.
- **[frenck/python-wled](https://github.com/frenck/python-wled)** — excellent
  ergonomics: enums, a hex-parsing `Color` type, segments keyed by id, presets and
  playlists split apart, and a single `Device` aggregate.

## Plans (suggested execution order)

| # | Plan | Theme |
|---|------|-------|
| 0 | [Update target frameworks (net8/9/10)](0-update-target-frameworks.md) | Foundation |
| 1 | [Core value types & enums](1-core-value-types-and-enums.md) | Foundation |
| 2 | [Command-value model & fluent builders](2-command-values-and-builders.md) | Foundation |
| 3 | [Complete the State object](3-complete-state-object.md) | Data model |
| 4 | [Complete the Segment object](4-complete-segment-object.md) | Data model |
| 5 | [Complete Info & new read endpoints (si/net/nodes/live)](5-complete-info-and-read-endpoints.md) | Read APIs |
| 6 | [Presets API](6-presets-api.md) | Feature |
| 7 | [Playlists API](7-playlists-api.md) | Feature |
| 8 | [Per-segment individual LED control](8-individual-led-control.md) | Feature |
| 9 | [Effect metadata (`/json/fxdata`)](9-effect-metadata.md) | Feature |
| 10 | [Configuration API (`/json/cfg`)](10-config-api.md) | Feature |
| 11 | [Client ergonomics & cross-cutting concerns](11-client-ergonomics-and-cross-cutting.md) | Quality |
| 12 | [Usability & correctness improvements (post-review)](12-usability-and-correctness-improvements.md) | Correctness |

Plan 0 modernises the toolchain (multi-targeting `netstandard2.0;net8.0;net9.0;net10.0`)
and should land first. Plans 1–2 are the ergonomic foundation and unblock everything else. Plans 3–10 add
discrete capabilities. Plan 11 (high-level helpers, `CancellationToken`, DI, errors,
multi-targeting, versioning) runs alongside all of them.

## Shared conventions for every plan

- Keep the existing dual-DTO serialization layer: immutable `XResponse` (total,
  non-null) + mutable `XRequest` (nullable, `[JsonIgnore(WhenWritingNull)]`) with
  `Request.From(Response)` + implicit operator. Builders and value types sit *on top*
  of this layer; the wire format never leaks into the public happy path.
- `[JsonPropertyName]` always carries the raw WLED key; the C# member uses the
  descriptive .NET name.
- Every new endpoint adds a method to `IWLedClient` + `WLedClient`, with GET/POST tests
  in `Kevsoft.WLED.Tests` (extend `JsonBuilder`, `MockHttpMessageHandler`).
- Custom `JsonConverter`s carry the "impossible to misuse" types across the wire; they
  are unit-tested in both directions against real WLED sample payloads.
- Backwards compatibility: where a public member changes shape (e.g. `int[][]` →
  `SegmentColors`), provide an `[Obsolete]` shim for one minor version (see Plan 11).

## Definition of Done — applies to **every** plan

A feature isn't finished when the code compiles and tests pass. Each plan must also:

1. **Update the root [`README.md`](../README.md)** — keep a "Supported features" /
   capability matrix current, and add or refresh a short usage snippet for the new
   capability. The README is the project's shop window; a feature that isn't documented
   there is effectively invisible to consumers.
2. **Keep [`samples/BasicConsole`](../samples/BasicConsole) exemplary** — extend the
   sample (or add a focused sample) so the new capability has a copy-pasteable, runnable
   example that demonstrates the *ergonomic* path (builders / intent methods), not the
   raw DTOs. The sample should always build and run against the latest API.
3. **Update `CHANGELOG.md`** (introduced in Plan 11) with the user-facing change.

Treat items 1 and 2 as acceptance criteria for the plan — reviewers should reject a
change that adds a feature without updating the README and the sample.
