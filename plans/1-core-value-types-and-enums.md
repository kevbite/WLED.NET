# Plan 1 — Core value types & enums

**Theme:** Foundation · **Unblocks:** every other plan

## Why

The current model leaks raw primitives that allow nonsensical values and force callers
to remember magic numbers:

- Colors are `int[][]` — nothing stops `[[999, -4]]` or a 7-element array.
- "Enums" are bare `byte`/`int`: `NightlightResponse.Mode` (0–3),
  `StateResponse.LiveDataOverride` (0–2), `LedsResponse.LightCapabilities` (bitfield).
- Preset / playlist ids are bare `int` with sentinel `-1` meaning "none".

Both reference libraries fix this with value types and enums
(`frenck/python-wled` `const.py`: `LightCapability`, `LiveDataOverride`,
`NightlightMode`, `SoundSimulationType`, `SyncGroup`; and a hex-parsing `Color`).

## What we build

### 1. Color value types

```csharp
public readonly record struct RgbColor(byte R, byte G, byte B)
{
    public static RgbColor FromHex(string hex);   // "FFAA00" / "#FFAA00"
    public string ToHex();
}

public readonly record struct RgbwColor(byte R, byte G, byte B, byte W);

/// Primary / secondary (background) / tertiary slots of a segment.
public sealed record SegmentColors(
    Color Primary,
    Color? Secondary = null,
    Color? Tertiary = null);
```

- A single `Color` abstraction must represent **either** RGB or RGBW (WLED accepts 3 or
  4 byte arrays, and hex strings). Model as a `readonly record struct Color` with a
  `bool HasWhite` discriminator, or two structs behind a small union. The constructor
  guarantees component validity (bytes), so an invalid color is unrepresentable.
- A `JsonConverter<SegmentColors>` reads the `col` array (handles 3-byte, 4-byte, and
  hex-string forms — see `frenck` `Color._deserialize`) and writes the canonical byte
  arrays. This is the only place that touches `int[][]`.

> Random colors (`"r"`) are a *command*, not a readable state, so they live in Plan 2
> (`SegmentUpdate`), not in the response `SegmentColors`.

### 2. Enums (replace bare numbers)

```csharp
public enum NightlightMode : byte { Instant = 0, Fade = 1, ColorFade = 2, Sunrise = 3 }

public enum LiveDataOverride : byte { Off = 0, UntilLiveEnds = 1, UntilReboot = 2 }

public enum SoundSimulation : byte { BeatSin = 0, WeWillRockYou = 1, Mode10_3 = 2, Mode14_3 = 3 }

public enum Expand1D : byte { Pixels = 0, Bar = 1, Arc = 2, Corner = 3 }

[Flags] public enum LightCapability : byte
{
    None = 0, Rgb = 1, WhiteChannel = 2, ColorTemperature = 4, ManualWhite = 8
}

[Flags] public enum SyncGroup : byte
{
    None = 0, Group1 = 1, Group2 = 2, Group3 = 4, Group4 = 8,
    Group5 = 16, Group6 = 32, Group7 = 64, Group8 = 128
}
```

Each enum gets a tiny `JsonConverter` (numeric on the wire, enum in C#). `[Flags]`
enums serialize as their integer bitfield.

### 3. Optional ids without sentinels

WLED uses `-1` for "no preset / no playlist". Expose these as `int?` on the response
(map `-1 → null` in a converter, as `frenck` does in `State.__post_deserialize__`) so
callers never compare against a magic sentinel.

## Files

- `src/Kevsoft.WLED/Color.cs`, `RgbColor.cs`, `RgbwColor.cs`, `SegmentColors.cs`
- `src/Kevsoft.WLED/Enums/*.cs`
- `src/Kevsoft.WLED/Json/*Converter.cs` (color + enums + nullable-id)

## Tests

- Hex round-trips: `"FFAA00"` ⇄ `(255,170,0)`; with/without `#`; lower/upper case.
- `col` deserialization for 3-byte, 4-byte, and hex-string arrays in one payload.
- Out-of-range hex / wrong-length arrays throw a clear `FormatException`/`JsonException`.
- Each enum maps to/from its documented integer; unknown values fail loudly.
- `[Flags]` round-trip for `LightCapability` value `7 → Rgb|WhiteChannel|ColorTemperature`.

## Acceptance

A color can only be constructed from valid components or valid hex; every documented
enumerated field is a real enum; "none" ids are `null`. No public API exposes `int[][]`
or a bare numeric "mode" any more (legacy members `[Obsolete]`, see Plan 11).
