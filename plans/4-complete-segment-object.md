# Plan 4 — Complete the Segment object

**Theme:** Data model · **Depends on:** Plans 1, 2

## Why

Segments are where most of WLED's expressive power lives, and the current
`SegmentResponse`/`SegmentRequest` map only ~18 of ~30 documented keys, with colors as
raw `int[][]`. This plan completes the segment surface with the typed foundations.

## Current vs. API

Mapped today: `id`, `start`, `stop`, `len`, `grp`, `spc`, `of`, `col`, `fx`, `sx`, `ix`,
`pal`, `sel`, `rev`, `frz`, `on`, `bri`, `mi`.

Missing (see [JSON API → Segment object](https://kno.wled.ge/interfaces/json-api/)):

| Key | Meaning | Modeled as |
|-----|---------|-----------|
| `n` | segment name | `string? Name` |
| `startY`,`stopY` | 2D matrix rows | `int? StartY/StopY` (Plan: 2D group) |
| `rY` | flip 2D vertically | `bool ReverseY` |
| `mY` | mirror 2D vertically | `bool MirrorY` |
| `tp` | transpose (swap X/Y) | `bool Transpose` |
| `cct` | color temperature (0–255 *or* 1900–10091 K) | `ColorTemperature` value type |
| `c1`,`c2` | custom sliders (0–255) | `byte CustomSlider1/2` |
| `c3` | custom slider (0–31) | `CustomSlider3` (0–31 guard) |
| `o1`,`o2`,`o3` | effect options | `bool Option1/2/3` |
| `m12` | Expand 1D FX | `Expand1D` enum (Plan 1) |
| `si` | sound simulation type | `SoundSimulation` enum (Plan 1) |
| `set` | group/set id (0–3) | `SegmentSet` (0–3 guard) |
| `fxdef` | load effect defaults | `SegmentUpdate.LoadEffectDefaults()` (write-only) |
| `rpt` | repeat segment to fill strip | `SegmentUpdate.RepeatToFill()` (write-only) |
| `cln` | clone source segment | `int? Clones` (read) |
| `i` | individual LED control | Plan 8 |
| `col` | colors | `SegmentColors` (Plan 1) — replaces `int[][]` |
| `fx`,`pal` | selection w/ `~`/`r` | `Selector` (Plan 2) on write |
| `sx`,`ix`,`bri` | adjustable | `ByteAdjust` (Plan 2) on write |
| `on`,`frz` | toggleable | `Toggleable` (Plan 2) on write |

### CCT value type (impossible to misuse)

`cct` is genuinely dual-range. Model it so the caller states *intent*:

```csharp
public readonly struct ColorTemperature
{
    public static ColorTemperature Relative(byte value);        // 0-255
    public static ColorTemperature Kelvin(int kelvin);          // 1900-10091, guarded
    public bool IsKelvin { get; }
}
```

The converter writes the relative byte or the Kelvin int per the docs, and reads back
into whichever range WLED reported (per the docs' forward-compat guidance).

## What we build

- Add the missing read fields to `SegmentResponse` (typed).
- Replace `int[][] Colors` with `SegmentColors Colors` (+ `[Obsolete]` shim, Plan 11).
- Add a **2D matrix sub-group** comment/region so matrix-only fields (`startY`,`stopY`,
  `rY`,`mY`,`tp`) are clearly grouped and documented as no-ops on 1D strips.
- Extend `SegmentUpdate` (Plan 2) with all writable fields, using `Toggleable`,
  `ByteAdjust`, `Selector`, `ColorTemperature`, and the guarded slider types.

## Files

- Edit `SegmentResponse.cs`, `SegmentRequest.cs`
- `src/Kevsoft.WLED/ColorTemperature.cs`, `CustomSlider3.cs`, `SegmentSet.cs`
- Extend `Fluent/SegmentUpdate.cs`

## Tests

- Full real segment payload round-trips (including a 2D segment with `startY/stopY`).
- `c3 > 31`, `set > 3`, Kelvin out of `1900–10091` all throw at construction.
- `cct` round-trips in both relative and Kelvin form.
- `col` via `SegmentColors` (RGB, RGBW, hex) round-trips; random color is a write command.
- Extend `JsonBuilder` segment emission with the new keys.

## Acceptance

Every documented segment key is represented with a correct, range-safe type; colors flow
through `SegmentColors`; 2D fields are clearly delineated; writable fields use the Plan 2
command types so relative/toggle/random work without magic strings.
