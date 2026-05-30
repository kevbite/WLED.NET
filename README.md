# WLED.NET [![Continuous Integration Workflow](https://github.com/kevbite/WLED.NET/actions/workflows/continuous-integration-workflow.yml/badge.svg)](https://github.com/kevbite/WLED.NET/actions/workflows/continuous-integration-workflow.yml) [![install from nuget](http://img.shields.io/nuget/v/WLED.svg?style=flat-square)](https://www.nuget.org/packages/WLED) [![downloads](http://img.shields.io/nuget/dt/WLED.svg?style=flat-square)](https://www.nuget.org/packages/Kevsoft.WLED)

A .NET wrapper around the [WLED](https://github.com/Aircoookie/WLED) [JSON API](https://kno.wled.ge/interfaces/json-api/).

WLED.NET aims to be **hard to misuse**: state is modelled with strong types and fluent
builders so that, wherever possible, an invalid request simply won't compile.

## Supported frameworks

The library multi-targets `net8.0`, `net9.0`, `net10.0` and `netstandard2.0`.

## Getting Started

### Installing Package

**WLED.NET** can be installed via the dotnet CLI:

```bash
dotnet add package WLED
```

For dependency-injection / `IHttpClientFactory` integration, also install:

```bash
dotnet add package WLED.DependencyInjection
```

## Usage

### Connecting

```csharp
var client = new WLedClient("http://office-computer-wled/");
```

Or register it with dependency injection so the underlying `HttpClient` is pooled correctly:

```csharp
services.AddWledClient("http://office-computer-wled/");
// or
services.AddWledClient(client => client.BaseAddress = new Uri("http://office-computer-wled/"));
```

The `WLedClient(string)` constructor owns its `HttpClient` and uses a `SocketsHttpHandler`
with a bounded `PooledConnectionLifetime`, so a long-lived client still picks up DNS changes
(e.g. a WLED device that gets a new IP). For applications, registering via
`IHttpClientFactory`/DI as above remains the recommended approach.

### Quick commands

Common operations have first-class "intent" methods:

```csharp
await client.TurnOn();
await client.TurnOff();
await client.Toggle();

await client.SetBrightness(200);
await client.SetColor(RgbColor.FromHex("FFAA00"));   // selected segments
await client.SetColor(RgbColor.FromHex("FFAA00"), segmentId: 1);
await client.SetEffect(9);                            // by effect id
await client.SetPalette(11);                          // by palette id
await client.SetEffect(Selector.Random);             // random effect
await client.SetPalette(Selector.RandomInRange(5, 10)); // random palette in a range

await client.Reboot();
```

With no `segmentId`, `SetColor`/`SetEffect`/`SetPalette` target the currently *selected*
segments (the WLED `"seg":{…}` object form); pass a `segmentId` to target one segment.

### Reading data

```csharp
var root  = await client.Get();            // full /json document
var state = await client.GetState();       // /json/state
var info  = await client.GetInformation(); // /json/info

Console.WriteLine($"{info.Name} is running WLED {info.VersionName}.");
```

### Device snapshot

`GetDevice()` reads `/json` once and returns a queryable `WLedDevice` read model that
resolves each segment's effect and palette against the device catalogs:

```csharp
var device = await client.GetDevice();

Console.WriteLine($"{device.Name} — {(device.IsOn ? "on" : "off")} @ {device.Brightness}");

foreach (var segment in device.SelectedSegments)
{
    Console.WriteLine($"Segment {segment.Id}: {segment.Effect.Name} / {segment.Palette.Name}");
}

// Opt in to effect metadata (an extra GET /json/fxdata) when you need it:
var detailed = await client.GetDevice(new DeviceSnapshotOptions { IncludeEffectMetadata = true });
```

### Effect & palette catalogs

Look effects and palettes up by id or name, and apply them type-safely:

```csharp
var effects = await client.GetEffectCatalog();

var rainbow = effects.FindByName("Rainbow");   // throws if missing/ambiguous
await client.SetEffect(rainbow);

foreach (var entry in effects.AvailableOnly)    // skips reserved RSVD/"-" slots
{
    Console.WriteLine($"{entry.Id}: {entry.Name}");
}
```

### Fluent state updates

Build a sparse update that only sends the fields you set:

```csharp
await client.UpdateState(update => update
    .TurnOn()
    .Brightness(128)
    .Transition(TimeSpan.FromSeconds(2))   // crossfade, up to ~109 minutes
    .Segment(0, segment => segment
        .Effect(0)
        .Color(RgbColor.FromHex("0066FF"))
        .Speed(200)));
```

Target the currently *selected* segments (the WLED `"seg":{…}` object form) with
`SelectedSegments(...)` instead of an explicit id:

```csharp
await client.UpdateState(update => update
    .SelectedSegments(segment => segment
        .Effect(9)
        .Palette(11)));
```

### Strong ids & ranges

Range-checked value types catch invalid ids before a request is sent, and flow into the
existing `int`-based APIs:

```csharp
var preset = PresetId.From(5);                       // throws unless 1–250
var ledmap = LedMapId.From(3);                       // throws unless 0–9

await client.UpdateState(update => update
    .LoadLedMap(ledmap)
    .SelectedSegments(segment => segment
        .Range(SegmentBounds.From(0, 30))));
```

### Individual LED control

```csharp
await client.SetIndividualLeds(segmentId: 0, leds => leds
    .Set(0, RgbColor.FromHex("FF0000"))
    .SetRange(1, 10, RgbColor.FromHex("00FF00")));
```

Large updates are transparently and safely split into multiple sequential requests.

### Presets & playlists

```csharp
var presets = await client.GetPresets();
await client.ApplyPreset(1);
await client.SavePreset(5, new SavePresetOptions { Name = "Movie night" });

await client.StartPlaylist(playlist => playlist
    .Add(1, TimeSpan.FromSeconds(10))
    .Add(2, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1))
    .Repeat(3));
```

### Device configuration

Read configuration and apply safe partial writes with a fluent builder — only the
sections and fields you touch are sent:

```csharp
var config = await client.GetConfig();

await client.UpdateConfig(cfg => cfg
    .Identity(name: "Kitchen", mdnsName: "wled-kitchen")
    .Mqtt(enabled: true, broker: "mqtt.local", port: 1883)
    .BootDefaults(on: true, brightness: 128, presetId: PresetId.From(5)));
```

Network and access-point changes can disconnect the device, so they require explicit
opt-in via `UpdateConfigOptions.AllowNetworkChanges`.

### Error handling

All calls throw a typed exception hierarchy:

```csharp
try
{
    await client.TurnOn();
}
catch (WledConnectionException) { /* device unreachable / timed out */ }
catch (WledResponseException ex) { /* non-2xx: ex.StatusCode, ex.Body */ }
```

Every method also accepts an optional `CancellationToken`.

## Supported features

| Area | Endpoint(s) | Supported |
| --- | --- | --- |
| Full state/info/effects/palettes | `GET /json` | ✅ |
| Device snapshot read model (`GetDevice`) | `GET /json` (+ `fxdata` opt-in) | ✅ |
| Effect & palette catalogs (lookup by id/name) | `GET /json/eff`, `GET /json/pal` | ✅ |
| Live state + info | `GET /json/si` | ✅ |
| State | `GET`/`POST /json/state` | ✅ |
| Device information | `GET /json/info` | ✅ |
| Effects & palettes lists | `GET /json/eff`, `GET /json/pal` | ✅ |
| Nearby Wi-Fi networks | `GET /json/net` | ✅ |
| Live LED stream | `GET /json/live` | ✅ |
| Brightness / on-off / toggle | `POST /json/state` | ✅ |
| Per-segment control (effect, palette, colours, options, 2D, grouping…) | `POST /json/state` | ✅ |
| Individual LED control (with auto-chunking) | `POST /json/state` | ✅ |
| Presets (read / apply / save / delete) | `presets.json`, `POST /json/state` | ✅ |
| Playlists (read / start / save) | `presets.json`, `POST /json/state` | ✅ |
| Effect metadata | `GET /json/fxdata` | ✅ |
| Node discovery | `GET /json/nodes` | ✅ |
| Device configuration (read / safe partial write) | `GET`/`POST /json/cfg` | ✅ |
| Strong id/range value types (`PresetId`, `LedMapId`, `SegmentBounds`…) | — | ✅ |
| Typed exceptions & cancellation | — | ✅ |
| DI / `IHttpClientFactory` integration | — | ✅ |

## Samples

The [samples](samples/) folder contains examples of how you could use the WLED.NET library.

## Changelog

See [CHANGELOG.md](CHANGELOG.md).

## Contributing

1. Issue
1. Fork
1. Hack!
1. Pull Request
