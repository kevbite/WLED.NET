# Plan 0 — Update target frameworks (net8.0 / net9.0 / net10.0)

**Theme:** Foundation · **Do first:** this precedes Plan 1 and unblocks the modern-only
language/JSON features the rest of the roadmap leans on (source-generated converters,
trimming/AOT friendliness in Plan 11).

## Why

The repository is pinned to old frameworks/SDK throughout:

| Project / file | Current target | Notes |
|----------------|----------------|-------|
| `src/Kevsoft.WLED/Kevsoft.WLED.csproj` | `netstandard2.0` | the shipped package |
| `test/Kevsoft.WLED.Tests/Kevsoft.WLED.Tests.csproj` | `net6.0` | out of support |
| `samples/BasicConsole/BasicConsole.csproj` | `net6.0` | out of support |
| `Dockerfile` | `mcr.microsoft.com/dotnet/sdk:6.0` | CI builds/tests/packs here |
| `.github/workflows/continuous-integration-workflow.yml` | uses the Docker image | |

`net6.0` and `net7.0` are out of support; `net8.0` (LTS), `net9.0` (STS) and `net10.0`
(LTS) are the current/next supported set. We want the **library** to multi-target so
modern consumers get the best build while older consumers keep working.

## Goal

- **Library (`Kevsoft.WLED`)**: multi-target `netstandard2.0;net8.0;net9.0;net10.0`.
  - Keep `netstandard2.0` for maximum reach (this is the whole point of a wrapper lib).
  - Add `net8.0`/`net9.0`/`net10.0` so we can use `System.Text.Json` **source
    generation**, trimming/AOT annotations, and newer language/runtime APIs under
    `#if NET8_0_OR_GREATER` where beneficial.
- **Tests & samples**: target the modern TFMs (multi-target tests across
  `net8.0;net9.0;net10.0` so we exercise every runtime the library ships for).
- **SDK/CI**: build on an SDK that can compile `net10.0`.

## Changes

### 1. `Directory.Build.props`
Centralise the TFM lists so they're defined once:

```xml
<PropertyGroup>
  <LibraryTargetFrameworks>netstandard2.0;net8.0;net9.0;net10.0</LibraryTargetFrameworks>
  <TestTargetFrameworks>net8.0;net9.0;net10.0</TestTargetFrameworks>
</PropertyGroup>
```

(Existing `LangVersion=latest`, `Nullable`, `ImplicitUsings` already live here and are
fine.)

### 2. `src/Kevsoft.WLED/Kevsoft.WLED.csproj`
```xml
<TargetFrameworks>$(LibraryTargetFrameworks)</TargetFrameworks>
```
(note the **plural** `TargetFrameworks`). Guard any newer-only API usage with
`#if NET8_0_OR_GREATER`. The `System.Text.Json` / `System.Net.Http.Json` package
references stay for the `netstandard2.0` leg; on the `net8.0+` legs they are part of the
shared framework, so reference them `Condition="'$(TargetFramework)' == 'netstandard2.0'"`
to avoid downgrading the in-box version.

### 3. Test & sample csproj
- Tests: `<TargetFrameworks>$(TestTargetFrameworks)</TargetFrameworks>`.
- Sample: `<TargetFramework>net10.0</TargetFramework>` (a single modern TFM is fine for a
  sample).
- Bump stale test packages that won't restore on net10 (`Microsoft.NET.Test.Sdk` 16.5.0,
  `xunit` 2.4.0, `FluentAssertions` 5.10.3, `coverlet` 1.2.0 are all old). Pin to current
  versions as part of this plan so the matrix actually builds.

### 4. `Dockerfile`
- Bump every stage base from `dotnet/sdk:6.0` to `dotnet/sdk:10.0` (the SDK can target
  down-level TFMs, so one SDK builds all legs).
- The `dotnet build ./src/**/*.csproj` / `dotnet test` / `dotnet pack` steps are
  TFM-agnostic and need no change beyond the SDK bump.

### 5. CI workflow
- No structural change required (it delegates to Docker), but confirm the runner/image
  pulls `sdk:10.0`. Optionally add a non-Docker `actions/setup-dotnet` matrix job that
  installs the 8/9/10 SDKs and runs `dotnet test` directly, to get clearer per-TFM
  results than the single Docker entrypoint provides.

## Sequencing & risk

1. Bump SDK in `Dockerfile` + package versions first (so the existing `netstandard2.0`
   library still builds on the new SDK) → green build.
2. Add the three `net*` legs to the library → green build.
3. Multi-target tests → ensure the suite passes on `net8.0`, `net9.0`, `net10.0`.
4. Then proceed with Plan 1 onwards.

Low functional risk (no behaviour change), but watch for: package-version incompatibilities
on net10, `TreatWarningsAsErrors=true` surfacing new analyzer warnings on the modern legs,
and the conditional package references (don't ship a down-level STJ on `net8.0+`).

## Tests / verification

- `dotnet build` and `dotnet test` succeed for **every** TFM in the matrix.
- `dotnet pack` produces a package whose `lib/` contains
  `netstandard2.0`, `net8.0`, `net9.0`, `net10.0` folders.
- The existing test suite passes unchanged on all test TFMs (behaviour parity).
- `samples/BasicConsole` builds and runs on `net10.0`.

## Acceptance

The library multi-targets `netstandard2.0;net8.0;net9.0;net10.0`, tests run across
`net8.0/net9.0/net10.0`, the Docker/CI toolchain uses the .NET 10 SDK, all packages
restore, and the full suite is green — providing the modern foundation the rest of the
roadmap (esp. Plan 11's source-generated JSON) builds on.
