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

### Quick commands

Common operations have first-class "intent" methods:

```csharp
await client.TurnOn();
await client.TurnOff();
await client.Toggle();

await client.SetBrightness(200);
await client.SetColor(RgbColor.FromHex("FFAA00"));
await client.SetEffect(9);    // by effect id
await client.SetPalette(11);  // by palette id

await client.Reboot();
```

### Reading data

```csharp
var root  = await client.Get();            // full /json document
var state = await client.GetState();       // /json/state
var info  = await client.GetInformation(); // /json/info

Console.WriteLine($"{info.Name} is running WLED {info.VersionName}.");
```

### Fluent state updates

Build a sparse update that only sends the fields you set:

```csharp
await client.UpdateState(update => update
    .TurnOn()
    .Brightness(128)
    .Segment(0, segment => segment
        .Effect(0)
        .Color(RgbColor.FromHex("0066FF"))
        .Speed(200)));
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
