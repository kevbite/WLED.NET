# Plan 11 — Client ergonomics & cross-cutting concerns

**Theme:** Quality · **Runs alongside:** all other plans

## Why

A complete field mapping isn't "easy to use" on its own. This plan covers the
cross-cutting work that makes the library pleasant, safe, and production-ready: high-level
intent methods, cancellation, DI, error handling, versioning, and migration.

## 1. High-level intent methods (the 80% use cases)

Most callers want one-liners, not state objects. Add convenience methods on `IWLedClient`
that compose the Plan 2 builders:

```csharp
Task TurnOn(CancellationToken ct = default);
Task TurnOff(CancellationToken ct = default);
Task Toggle(CancellationToken ct = default);
Task SetBrightness(byte brightness, CancellationToken ct = default);
Task SetColor(RgbColor color, int? segmentId = null, CancellationToken ct = default);
Task SetEffect(Selector effect, int? segmentId = null, CancellationToken ct = default);
Task SetPalette(Selector palette, int? segmentId = null, CancellationToken ct = default);
Task Reboot(CancellationToken ct = default);                 // {"rb":true}
```

The README's first example should become `await client.TurnOn();` /
`await client.SetColor(RgbColor.FromHex("FFAA00"));`.

## 2. `CancellationToken` on every async method

Every existing and new client method gains a trailing `CancellationToken ct = default`
and forwards it to `HttpClient`. (Source-compatible additive change.)

## 3. DI / `IHttpClientFactory` integration

```csharp
services.AddWledClient("http://wled-desk/");          // typed-client registration
// or
services.AddWledClient(o => o.BaseAddress = new(...)); // options-based
```

- Add a `Kevsoft.WLED.DependencyInjection` extension (or guard behind a target/feature)
  registering `IWLedClient` via `AddHttpClient`, so `HttpClient` lifetime/pooling is
  handled correctly instead of `new HttpClient(...)` per instance.
- Keep the existing constructors for non-DI use.

## 4. Error handling

Today every method calls `EnsureSuccessStatusCode()` (raw `HttpRequestException`) and
assumes non-null JSON. Improve to a typed exception hierarchy:

```csharp
public class WledException : Exception { }
public sealed class WledConnectionException : WledException { }   // transport/timeout
public sealed class WledResponseException : WledException         // non-2xx
{ public int StatusCode { get; } public string? Body { get; } }
public sealed class WledUnsupportedVersionException : WledException { }
```

- Wrap transport failures and non-2xx responses; include status + body snippet.
- Mirror `frenck`'s minimum-version guard (`MIN_REQUIRED_VERSION = 0.14.0`): optionally
  validate `info.ver` once and throw `WledUnsupportedVersionException` for too-old
  firmware, since many typed features assume ≥ 0.14.

## 5. Multi-targeting

Currently `netstandard2.0` with `System.Text.Json`. Add `net8.0` (and keep
`netstandard2.0`) so modern consumers get trimming/AOT-friendly **source-generated**
JSON contexts (`JsonSerializerContext`) for the new converters, and `netstandard2.0`
keeps broad reach.

## 6. Backwards-compatibility & migration

Several plans change public shapes (`int[][]` → `SegmentColors`, `byte` → enums,
`int` ids → `int?`). To avoid a hard break:

- Mark old members `[Obsolete("Use X")]` for one minor release, keeping them working via
  shims where feasible; remove in the next major.
- Document the migration in the README and a `CHANGELOG.md`.
- Follow SemVer: shape-changing removals → major bump.

## 7. Docs & samples (and the standing "keep them current" rule)

This plan **establishes** the documentation/sample assets; every other plan is then
responsible for keeping them up to date (see the *Definition of Done* in the
[plans README](README.md)).

- **Root `README.md`**:
  - Replace the manual-DTO examples with builder + intent-method examples
    (`await client.TurnOn();`, `await client.SetColor(RgbColor.FromHex("FFAA00"));`).
  - Add a **"Supported features" capability matrix** (feature → supported? → link to the
    relevant plan / sample) so consumers can see coverage at a glance. This matrix is
    updated by each feature plan as it lands.
  - Document supported WLED firmware versions and the target frameworks (Plan 0).
- **`CHANGELOG.md`**: create it (Keep a Changelog format) and require an entry per change.
- **Samples**: keep `samples/BasicConsole` as a clean, runnable showcase of the
  ergonomic API, and add focused samples as features land: color/effect, presets,
  playlists, individual LEDs, diagnostics (info/wifi/fs). Each sample must build and run
  on the current sample TFM (Plan 0). Wire the samples into CI build so they can't rot.
- Consider enabling XML doc output → a docs site (already `GenerateDocumentationFile`),
  and surfacing the README capability matrix there.

## Files

- `src/Kevsoft.WLED/WLedClient.cs`, `IWLedClient.cs` (intent methods, `ct`)
- `src/Kevsoft.WLED/Exceptions/*.cs`
- `src/Kevsoft.WLED.DependencyInjection/*` (or guarded folder)
- `Kevsoft.WLED.csproj` (multi-target, STJ source-gen), `README.md`, `CHANGELOG.md`

## Tests

- Intent methods emit the expected minimal JSON to the right route.
- `CancellationToken` cancels in-flight requests (cancelled token → `OperationCanceledException`).
- Error mapping: 404/500 → `WledResponseException` with status/body; transport →
  `WledConnectionException`.
- DI registration resolves a working `IWLedClient`.
- Obsolete shims still serialize identically to the new types.

## Acceptance

The common operations are one-liners with cancellation support; failures surface as typed
exceptions; the client integrates with DI/`IHttpClientFactory`; the package multi-targets;
and shape changes ship with obsolete shims + a documented migration path.
