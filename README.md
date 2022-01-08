# WLED.NET [![Continuous Integration Workflow](https://github.com/kevbite/WLED.NET/actions/workflows/continuous-integration-workflow.yml/badge.svg)](https://github.com/kevbite/WLED.NET/actions/workflows/continuous-integration-workflow.yml) [![install from nuget](http://img.shields.io/nuget/v/WLED.svg?style=flat-square)](https://www.nuget.org/packages/WLED) [![downloads](http://img.shields.io/nuget/dt/WLED.svg?style=flat-square)](https://www.nuget.org/packages/Kevsoft.WLED)

A .NET Wrapper around the [WLED](https://github.com/Aircoookie/WLED) [JSON API](https://github.com/Aircoookie/WLED/wiki/JSON-API).

## Getting Started

### Installing Package

**WLED.NET** can be installed directly via the package manager console by executing the following commandlet:

```powershell
Install-Package WLED
```

alternative you can use the dotnet CLI.

```bash
dotnet add package WLED
```

## Usage

### Getting data from the WLED device

```csharp
var client = new WLedClient("http://office-computer-wled/");

var data = await client.Get();
```

### Post data to the WLED device

Turn on the device on

```csharp
var client = new WLedClient("http://office-computer-wled/");
await client.Post(new StateRequest { On = true });
```
## Samples

The [samples](samples/) folder containers examples of how you could use the WLED.NET Library.

## Contributing

1. Issue
1. Fork
1. Hack!
1. Pull Request

