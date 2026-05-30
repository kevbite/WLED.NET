# Plan 14 — HttpClient connection lifetime & DNS staleness

**Theme:** Correctness · Reliability · Cross-cutting

Tracks [issue #8](https://github.com/kevbite/WLED.NET/issues/8).

## Why

A long-lived `HttpClient` over a default `HttpClientHandler` keeps its TCP connections
open indefinitely and therefore **never honours DNS changes** — a documented .NET pitfall
(see [HttpClient guidelines: DNS behavior](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines#dns-behavior)).

WLED devices are a realistic trigger for this:

- they are frequently addressed by mDNS/hostname or a DHCP-assigned IP that can change
  (device reboot, lease renewal, network change), and
- a typical consumer creates **one** `WLedClient("http://wled.local/")` for the lifetime of
  the app and reuses it.

The DI path (`services.AddWledClient(...)`) is already safe: it goes through
`IHttpClientFactory`, which rotates handlers on a default 2-minute lifetime. The problem is
isolated to the convenience constructors that own a self-created `HttpClient`:

- `WLedClient(string baseUri)` → `new HttpClientHandler()` (see `WLedClient.cs`).
- `WLedClient(HttpMessageHandler, string)` → caller-supplied handler (out of scope; the
  caller owns its lifetime).

## Goal

When the library creates the handler itself, give pooled connections a finite lifetime so
DNS is periodically refreshed, **without** regressing throughput or the netstandard2.0
target.

## Approach

In `WLedClient`, replace the default `new HttpClientHandler()` used by
`WLedClient(string baseUri)` with a `SocketsHttpHandler` configured with a bounded
`PooledConnectionLifetime`, on runtimes where `SocketsHttpHandler` exists.

`SocketsHttpHandler` is available on .NET Core 2.1+/.NET 5+ but **not** in the
netstandard2.0 reference assemblies, so this must be guarded by a target-framework
conditional rather than a runtime check:

```csharp
private static HttpMessageHandler CreateDefaultHandler()
{
#if NETSTANDARD2_0
    // .NET Framework / legacy: SocketsHttpHandler is unavailable. Fall back to the
    // platform handler; consumers on these runtimes should prefer IHttpClientFactory.
    return new HttpClientHandler();
#else
    return new SocketsHttpHandler
    {
        // Refresh pooled connections (and therefore DNS) periodically.
        PooledConnectionLifetime = TimeSpan.FromMinutes(2)
    };
#endif
}
```

Wire it into the existing constructor:

```csharp
public WLedClient(string baseUri) : this(CreateDefaultHandler(), baseUri)
{
}
```

Notes / decisions:

- **Two minutes** mirrors `IHttpClientFactory`'s default handler lifetime, keeping the two
  paths consistent. It is a reasonable default for LAN devices and can be revisited if a
  configurable overload is later requested.
- Keep the existing `Connection: keep-alive` default request header; `PooledConnectionLifetime`
  bounds connection reuse without disabling keep-alive.
- **Do not** change the `WLedClient(HttpMessageHandler, string)` or `WLedClient(HttpClient)`
  constructors — those hand ownership of the handler/client to the caller (including DI),
  and overriding their lifetime would be surprising.
- netstandard2.0 constraint: `SocketsHttpHandler` is not referenced under that TFM, so the
  `#if` keeps the netstandard2.0 build green (it currently builds clean and must stay so).

## Tests

- A test asserting `new WLedClient("http://wled.local/")` constructs successfully and can
  issue a request (the existing `MockHttpMessageHandler` path already covers request
  behaviour; this guards the default-handler wiring on net8/9/10).
- Because handler internals aren't observable through `HttpClient`, prefer a small,
  internal, testable seam if exact `PooledConnectionLifetime` verification is wanted:
  e.g. an `internal static HttpMessageHandler CreateDefaultHandler()` exposed to the test
  project via `InternalsVisibleTo`, asserting it returns a `SocketsHttpHandler` with the
  expected lifetime on modern TFMs. Keep this minimal and conditional-compiled.
- Confirm the full multi-target build (incl. netstandard2.0) and the existing 205 tests
  remain green.

## Definition of Done (per `plans/README.md`)

1. **README.md** — add a short note under connecting/DI guidance explaining that the
   convenience constructor now refreshes DNS via a bounded connection lifetime, and that
   `IHttpClientFactory`/DI remains the recommended approach for apps.
2. **samples/BasicConsole** — no behavioural change required; the existing
   `new WLedClient(...)` usage now benefits automatically. Update a comment if helpful.
3. **CHANGELOG.md** — record the fix under the unreleased section
   (e.g. *"The `WLedClient(string)` constructor now uses a `SocketsHttpHandler` with a
   bounded `PooledConnectionLifetime` so DNS changes are picked up (fixes #8)."*).

## Out of scope

- Making the connection lifetime configurable via a new constructor/options overload — can
  be a fast follow if requested.
- Any change to the DI package, which already delegates lifetime management to
  `IHttpClientFactory`.
