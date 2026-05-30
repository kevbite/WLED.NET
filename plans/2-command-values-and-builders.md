# Plan 2 — Command-value model & fluent builders

**Theme:** Foundation · **Depends on:** Plan 1 · **Unblocks:** Plans 3, 4, 6, 7, 8

## Why

The single biggest "easy to misuse" risk in the JSON API is its **command mini-language**.
Many writable fields accept more than a plain value:

| Field(s) | Accepts | Example |
|----------|---------|---------|
| `on`, segment `on`, `frz` | `true` / `false` / `"t"` (toggle) | `{"on":"t"}` |
| `bri`, `sx`, `ix` | absolute, `~`/`~-` (inc/dec), `~10`/`~-10`, `w~40` (wrap) | `{"bri":"~40"}` |
| `fx`, `pal` | id, `~`/`~-`, `"r"` (random) | `{"seg":{"fx":"r"}}` |
| `ps` | id, `"1~6~"` (cycle), `"4~10r"` (random in range) | `{"ps":"1~6~"}` |
| `col` slot | RGB(W), hex, `"r"` (random) | `["r",[0,0,0],"r"]` |

If we expose these as `string`, callers will hand-build invalid commands. If we expose
them only as numbers, we lose toggling/relative/random entirely. The fix is a small set
of closed command types that can *only* be created through valid factory methods.

## What we build

### 1. Command value types (closed, factory-only)

```csharp
// Boolean that may also toggle.
public readonly struct Toggleable
{
    public static Toggleable On { get; }
    public static Toggleable Off { get; }
    public static Toggleable Toggle { get; }
    public static implicit operator Toggleable(bool value);
}

// 0-255 value that can be set, nudged, or wrapped.
public readonly struct ByteAdjust
{
    public static ByteAdjust Set(byte value);
    public static ByteAdjust Increment(byte by = 1);
    public static ByteAdjust Decrement(byte by = 1);
    public static ByteAdjust IncrementWrap(byte by);   // "w~40"
    public static implicit operator ByteAdjust(byte value);
}

// Effect / palette selection.
public readonly struct Selector
{
    public static Selector Id(int id);
    public static Selector Next { get; }      // "~"
    public static Selector Previous { get; }  // "~-"
    public static Selector Random { get; }    // "r"
    public static implicit operator Selector(int id);
}

// Preset selection incl. cycle / random-in-range.
public readonly struct PresetSelector
{
    public static PresetSelector Id(int id);
    public static PresetSelector Cycle(int from, int to);          // "1~6~"
    public static PresetSelector RandomInRange(int from, int to);  // "4~10r"
}
```

Each type has a `JsonConverter` that emits the correct primitive or string. Because the
only way to create one is a validated factory/implicit conversion, **an invalid command
string is unrepresentable**.

### 2. Fluent builders (the primary public write surface)

Instead of newing up `StateRequest { ... }` bags of nullables, callers use builders that
read like intent and compile down to the existing `StateRequest`/`SegmentRequest` DTOs:

```csharp
await client.UpdateState(s => s
    .TurnOn()                       // or .Toggle()
    .Brightness(ByteAdjust.IncrementWrap(40))
    .Transition(TimeSpan.FromMilliseconds(700))
    .Segment(0, seg => seg
        .Color(RgbColor.FromHex("FFAA00"))
        .Effect(Selector.Random)
        .Speed(200)));
```

- `StateUpdate` / `SegmentUpdate` expose only settable concepts, each strongly typed.
- `Transition` accepts a `TimeSpan` and converts to WLED's 100 ms units (and supports
  `tt` — "this call only" — via `.TransitionOnce(...)`).
- The builder produces a `StateRequest`; `client.UpdateState(Action<StateUpdate>)` is a
  thin wrapper over the existing `Post(StateRequest)`.

### 3. Keep DTOs, hide them

The nullable `StateRequest`/`SegmentRequest` remain (serialization layer + power users),
but the documented, discoverable path is the builder.

## Files

- `src/Kevsoft.WLED/Commands/{Toggleable,ByteAdjust,Selector,PresetSelector}.cs`
- `src/Kevsoft.WLED/Json/*CommandConverter.cs`
- `src/Kevsoft.WLED/Fluent/{StateUpdate,SegmentUpdate}.cs`
- `IWLedClient.UpdateState(Action<StateUpdate>)` + `WLedClient` impl

## Tests

- Each command type serializes to the exact documented token (`"t"`, `"~"`, `"~-"`,
  `"~10"`, `"w~40"`, `"r"`, `"1~6~"`, `"4~10r"`) and absolute values stay numeric.
- Builder composition emits minimal JSON (only touched fields present) — reuse the
  existing `PostEmpty*`/`PostFull*` assertion style in `WLedClientPostTests`.
- `TimeSpan` → 100 ms unit conversion (rounding, clamping to `0–65535`).

## Acceptance

Toggling, relative adjustments, random and preset-cycle commands are all expressible
**only** through valid types, and the fluent builder is the natural way to write state.
