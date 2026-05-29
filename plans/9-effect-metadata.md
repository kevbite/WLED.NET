# Plan 9 — Effect metadata (`/json/fxdata`)

**Theme:** Feature · **Depends on:** Plans 1, 5

## Why

Since 0.14, `GET /json/fxdata` returns, per effect, a compact metadata string describing
which controls that effect actually uses (sliders, color slots, palette, flags, defaults).
This is what lets a UI hide irrelevant controls and what lets *us* validate that a caller
isn't setting, say, `c3` on an effect that ignores it. Today this is unexposed and the raw
string format is genuinely fiddly — perfect to parse once, correctly, behind a type.

## API surface (WLED)

`/json/fxdata` → array of strings aligned by index with the effects list. Format:

```
<parameters>;<colors>;<palette>;<flags>;<defaults>
```

Example (Aurora): `!,!;;!;1;sx=24,pal=50`

- **parameters:** up to 5 sliders (`sx,ix,c1,c2,c3`) + 3 checkboxes (`o1,o2,o3`),
  comma-separated labels; `!` = default label; empty = control hidden.
- **colors:** up to 3 slots (`Fx`,`Bg`,`Cs`); `!` = default label; empty = hidden.
- **palette:** `!` = palette selection enabled; empty = no palette.
- **flags:** single chars — `1`=1D, `2`=2D matrix, `3`=3D, `v`=volume-reactive,
  `f`=frequency-reactive, `0`=single-LED.
- **defaults:** `name=value` pairs, e.g. `sx=24,pal=50`.

Fallbacks for missing sections are documented (2 sliders; 3 colors; palette on; flag `1`).

## What we build — parse the format into a rich, queryable type

```csharp
public sealed record EffectMetadata(
    int EffectId,
    string Name,
    IReadOnlyList<EffectControl> Sliders,    // sx, ix, c1, c2, c3 (with label, range)
    IReadOnlyList<EffectControl> Options,    // o1, o2, o3 checkboxes
    IReadOnlyList<EffectColorSlot> Colors,   // Fx / Bg / Cs (visible ones)
    bool UsesPalette,
    EffectDimensionality Dimensionality,     // OneD / TwoD / ThreeD / SingleLed
    bool ReactsToVolume,
    bool ReactsToFrequency,
    IReadOnlyDictionary<string, int> Defaults);

[Flags] public enum EffectCapabilities { ... } // optional convenience overlay
```

`EffectControl` records its parameter key (`c3`), label, and documented value range
(so `c3` knows it's 0–31). The parser applies all documented fallbacks.

### Client methods

```csharp
Task<IReadOnlyList<EffectMetadata>> GetEffectMetadata(CancellationToken ct = default);
```

Pair it with the effect names (`GetEffects`) to populate `Name`, and **filter out
`RSVD`/`-` reserved effects** (docs recommend hiding them; `frenck` drops `RSVD`).

### The ergonomic payoff (optional, powerful)

Expose validation helpers that make misuse visible:

```csharp
metadata.Supports(SegmentControl.Custom3);   // false → setting c3 is a no-op
segmentUpdate.ValidateAgainst(metadata);     // warn/throw if setting hidden controls
```

This turns effect metadata from documentation into an *enforced contract*.

## Files

- `src/Kevsoft.WLED/{EffectMetadata,EffectControl,EffectColorSlot}.cs`
- `src/Kevsoft.WLED/EffectDimensionality.cs`
- `src/Kevsoft.WLED/Json/EffectMetadataParser.cs`
- `IWLedClient` + `WLedClient`: `GetEffectMetadata`

## Tests

- Parse documented examples: `!,!;;!;1;sx=24,pal=50` (Aurora), `2v` flags, empty
  sections, `,Saturation,,,,Invert`, `,,,,,Random colors`.
- All fallbacks (missing parameters → 2 sliders; missing colors → 3; missing palette →
  on; missing flags → 1D).
- Reserved effects filtered; indices stay aligned with the effects list.
- `Defaults` parsed to `{ sx:24, pal:50 }`.

## Acceptance

Effect metadata is available as a fully-parsed, queryable model (controls, ranges, flags,
defaults), reserved effects are filtered, and optional validation lets callers avoid
setting controls an effect ignores.
