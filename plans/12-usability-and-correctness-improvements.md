# Plan 12 — Usability & correctness improvements (post-review)

**Theme:** Correctness · Quality · **Source:** [`reviews/1-wled-json-api-review.md`](../reviews/1-wled-json-api-review.md)

## Why

Plans 0–11 brought the library to full, strongly-typed coverage of the WLED JSON API.
A subsequent review against the [JSON API docs](https://kno.wled.ge/interfaces/json-api/)
surfaced a handful of **correctness bugs** and **ergonomic gaps**. This plan turns those
findings into actionable work.

Per the project owner's steer, this plan **prioritises usability and correctness over
adding new endpoints**. New-endpoint findings (`/json/palx`, `info.sensor`) and
speculative future-WLED features (random color slots) are explicitly de-scoped here and
captured only as optional/back-log notes.

Because we are pre-1.0 on this API surface and have decided **not** to keep `[Obsolete]`
shims, shape-changing members are **removed outright** — consumers update on upgrade.
(This overrides the obsolete-shim guidance in Plan 11 / the plans README.)

---

## Priority 1 — Correctness bugs (must fix)

### 1.1 "Selected segments" intent methods currently target segment 0

**Finding:** review #1 (High). `WLedClient.SetColor` / `SetEffect` / `SetPalette`
document "selected segments when no id is given", but `SingleSegment(...)` always emits
`seg` as a one-element **array** (`WLedClient.cs:272-277`). Per the docs, an array entry
with no `id` infers `id:0`, so these one-liners actually only touch segment 0 — directly
contradicting their XML docs.

**Docs:** `seg` may be an **object** *or* an **array**. The selected-segment examples use
the object form, e.g. `{"seg":{"fx":"r"}}`. The array form infers `id` from array
position when `id` is omitted.

**Proposed change:**

- Model the request `seg` as a union that serializes as **either** a `SegmentRequest`
  object **or** a `SegmentRequest[]`. Introduce a small wrapper (e.g.
  `SegmentRequestPayload`) with a `JsonConverter` that writes:
  - the **object** form when targeting selected segments (no id), and
  - the **array** form when one or more explicit ids are given.
- No-id intent methods (`SetColor`/`SetEffect`/`SetPalette` with `segmentId == null`)
  emit the **object** form → `{"seg":{...}}`.
- Explicit `segmentId` calls continue to emit `{"seg":[{"id":N,...}]}`.
- Fix the XML docs so wording matches behaviour.

**Tests:** `IntentMethodTests.cs:47-85` currently asserts the buggy array/`id:0` form for
the no-id case — these expectations must be **updated** to the object form. Add explicit
`segmentId` tests that still assert the array+id form.

### 1.2 Transition values limited to `byte`, but the API allows `0..65535`

**Finding:** review #2 (High). `transition` and one-shot `tt` are documented as
`0..65535` (×100 ms ≈ up to ~109 min). Today:

- `StateResponse.Transition` is `byte` (`StateResponse.cs:20-21`)
- `StateRequest.Transition` / `TransientTransition` are `byte?` (`StateRequest.cs:16-26`)
- `StateUpdate.ToTransitionUnits` clamps to `byte.MaxValue` (`StateUpdate.cs:134-147`);
  the fluent comment claims a 25.5 s max (`StateUpdate.cs:35`).

The client therefore **cannot represent valid values > 255** and will mis-handle a
response whose transition exceeds 255 units.

**Proposed change:**

- Change `transition`/`tt` fields to `ushort` / `ushort?`.
- Update `StateUpdate.ToTransitionUnits` to clamp to `ushort.MaxValue` and fix the
  `TimeSpan` ↔ units helper/comment (max ≈ 6553.5 s, not 25.5 s).
- Mind the `netstandard2.0` target: no `Math.Clamp` — use a manual clamp helper.

**Tests:** add cases with a transition **> 255 units** (e.g. a `TimeSpan` of 60 s ⇒ 600
units) on both serialize and deserialize paths.

---

## Priority 2 — Ergonomic / forward-compat improvements (should fix)

### 2.1 Effect/palette `Selector` lacks ranged-random (`"5~10r"`)

**Finding:** review #3 (Medium). Docs show `{"seg":[{"id":2,"pal":"5~10r"}]}`.
`Selector` supports only id/next/previous/random (`Selector.cs:10-38`) and
`SelectorJsonConverter` rejects anything except `"~"`, `"~-"`, `"r"`
(`SelectorJsonConverter.cs:13-21`). `PresetSelector.RandomInRange` already does this for
presets (`PresetSelector.cs:40-45`) — mirror it.

**Proposed change:**

- Add `Selector.RandomInRange(int from, int to)` producing the token `"{from}~{to}r"`,
  with `from <= to` validation at construction.
- Extend `SelectorJsonConverter` to read/write that token (round-trip tested).

### 2.2 Richer typed config sections (incremental)

**Finding:** review #6 (Medium). `DeviceConfig` models only `id.name`; everything else is
`JsonExtensionData` (`DeviceConfig.cs:7-100`). Safe for round-tripping, but callers still
hand-build raw `JsonElement`s — not yet "impossible to send the wrong information".

**Proposed change (incremental, low-risk only):** model a few high-value, low-risk
sections as small value objects/enums while keeping `JsonExtensionData` as the escape
hatch. Candidate first slices (keep scope tight):

- identity (`id.*`: name, brightness factor),
- default-on/brightness defaults,
- sync/UDP enablement (already partly guarded),
- MQTT enablement.

This is explicitly **incremental** — do not attempt full `/json/cfg` modelling. Each
added section must round-trip losslessly with the extension data.

> If time-boxed, land 2.1 first; 2.2 can be split into its own follow-up plan/PR.

### 2.3 `ColorTemperature` Kelvin range / escape hatch

**Finding:** review #7 (Low). `ColorTemperature.Kelvin` allows only `1900..10091`
(`ColorTemperature.cs:11-38`), but the docs advise integrations expect a broader
forward-compatible Kelvin range (`1000..16000`, later `1000..20000`) and to preserve the
range received from WLED.

**Proposed change:** pick one:

- widen the validated constructor range to the docs' forward-compatible bounds, **or**
- add a clearly named escape hatch `ColorTemperature.KelvinUnchecked(int)` while keeping
  strict validation as the default.

Reading is already permissive (`FromWire` treats >255 as Kelvin); ensure a wide value
round-trips unchanged.

---

## De-scoped (noted, not done in this plan)

These are tracked for visibility but intentionally **out of scope** per the
"no new endpoints unless it really makes sense" steer:

- **`/json/palx` custom palettes** (review #4) — new endpoint; wire format needs
  confirming from firmware/companion libs. If not implemented, state it explicitly in the
  README feature matrix rather than implying full palette manipulation.
- **`info.sensor` draft Sensors API** (review #5) — read-only; nice-to-have. If trivial,
  could be added as a flexible `SensorResponse[]` with `JsonExtensionData`, but not
  required here.
- **Random color slots `"r"` in `col`** (review #8) — future/soon WLED behaviour; defer
  until stable in firmware, and prefer a separate `ColorSlot` union over weakening
  `Color`.

---

## Files

- `src/Kevsoft.WLED/WLedClient.cs`, `IWLedClient.cs` — seg union payload + intent-method
  object form (1.1)
- `src/Kevsoft.WLED/StateRequest.cs`, `StateResponse.cs`,
  `src/Kevsoft.WLED/Fluent/StateUpdate.cs` — `ushort` transition (1.2)
- `src/Kevsoft.WLED/Commands/Selector.cs`, `Commands/SelectorJsonConverter.cs` —
  ranged-random (2.1)
- `src/Kevsoft.WLED/Config/DeviceConfig.cs` (+ new small section types) — typed config
  (2.2)
- `src/Kevsoft.WLED/ColorTemperature.cs` — widened range / escape hatch (2.3)
- `test/Kevsoft.WLED.Tests/IntentMethodTests.cs` and related tests — updated expectations

## Tests

- No-id `SetColor`/`SetEffect`/`SetPalette` emit `{"seg":{...}}` (object); explicit
  `segmentId` emits `{"seg":[{"id":N,...}]}` (array). (1.1)
- Transition serialize/deserialize round-trips a value **> 255 units**; clamp at
  `ushort.MaxValue`. (1.2)
- `Selector.RandomInRange(5,10)` ⇄ `"5~10r"`; invalid range throws. (2.1)
- New config sections round-trip losslessly alongside `JsonExtensionData`. (2.2)
- A Kelvin value outside the old `1900..10091` band round-trips (via wide range or
  `KelvinUnchecked`). (2.3)

## Definition of Done

Per the [plans README](README.md) Definition of Done, **plus**:

1. **Root `README.md`** — fix any examples implying no-id intent methods target a single
   segment; ensure the feature matrix reflects transition range and ranged-random
   selectors; note `/json/palx` status honestly.
2. **`samples/BasicConsole`** — ensure the showcased one-liners reflect the corrected
   selected-segment behaviour.
3. **`CHANGELOG.md`** — record the breaking changes: `byte` → `ushort` transition, the
   `seg` object-vs-array behaviour change, any `Selector`/`ColorTemperature` API additions.
   **No `[Obsolete]` shims** — breaking members are removed outright (call this out in the
   changelog so consumers know to update).

## Acceptance

The selected-segment one-liners actually target selected segments; transitions above 255
units work end-to-end; ranged-random effect/palette selection is expressible; color
temperature is no longer stricter than WLED; and a first slice of config is ergonomically
typed — all with no `[Obsolete]` baggage and a documented, breaking-change CHANGELOG.
