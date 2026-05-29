# Plan 8 — Per-segment individual LED control

**Theme:** Feature · **Depends on:** Plans 1, 2

## Why

The segment `i` property lets you address individual LEDs — great for clocks, meters,
notifications. It has three overlapping wire encodings packed into one heterogeneous JSON
array, which is hostile to hand-author. We hide all of that behind explicit, typed ops.

## API surface (WLED `seg.i`)

1. **Sequential from 0:** `{"i":["FF0000","00FF00","0000FF"]}` (or RGB arrays).
2. **Indexed:** `{"i":[0,"FF0000",2,"00FF00",4,"0000FF"]}` (index, color, ...).
3. **Ranges:** `{"i":[0,8,"FF0000",10,18,"0000FF"]}` (start, stop, color, ...).

Rules from the docs we must encode/enforce:
- Indices are **segment-relative**, not strip-global.
- Hex is preferred over arrays for large payloads (efficiency).
- Must send ≤ ~256 colors per request; **split** larger sets into sequential calls
  (never parallel — the device serializes poorly).
- Setting LEDs **freezes** the segment; brightness must be set *beforehand* (turning on
  and setting LEDs in the same request does **not** work).

## What we build — one typed builder, three intents

```csharp
public sealed class IndividualLedBuilder
{
    IndividualLedBuilder Set(params Color[] sequentialFromStart);     // form 1
    IndividualLedBuilder Set(int index, Color color);                // form 2
    IndividualLedBuilder SetRange(int start, int stopExclusive, Color color); // form 3
}
```

Client method:

```csharp
Task SetIndividualLeds(int segmentId, Action<IndividualLedBuilder> build,
                       CancellationToken ct = default);
```

### Safety / correctness baked in

- The builder accumulates ops, then the converter emits the **most compact** legal `i`
  array (prefers hex, collapses consecutive single sets into a range where shorter).
- **Auto-chunking:** if the resulting color count exceeds the safe limit (configurable,
  default 256), the client transparently issues **sequential** requests (awaiting each)
  using the `[startIndex, ...]` form — implementing the docs' guidance for the caller.
- A guard rejects negative indices and `stop <= start` at build time.
- Doc-comment + analyzer-style guidance: brightness/power-on must precede LED setting;
  optionally expose `SetIndividualLeds(..., ensureBrightness: byte?)` that pre-sends a
  brightness state in a prior request when requested.

### Reading

`i` is *not* part of the state response (write-only per docs), so there is no read model;
current colors come from `/json/live` (Plan 5) instead. Document this explicitly.

## Files

- `src/Kevsoft.WLED/IndividualLedBuilder.cs`
- `src/Kevsoft.WLED/Json/IndividualLedConverter.cs`
- `IWLedClient` + `WLedClient`: `SetIndividualLeds`

## Tests

- Sequential / indexed / range forms each emit the exact documented array shape.
- Hex chosen over RGB arrays for compactness; RGBW falls back to arrays.
- Auto-chunking: a 600-LED set produces 3 sequential requests with correct start offsets,
  issued in order (assert call sequence on `MockHttpMessageHandler`).
- Build-time guards: negative index, `stop <= start` throw.

## Acceptance

Individual-LED addressing is expressed through one builder with three clear intents; the
library owns encoding choice and the sequential-chunking rule, so a caller can light
thousands of LEDs correctly without knowing the wire format or the 256-color limit.
