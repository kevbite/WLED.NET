# Plan 15 — NuGet package metadata, README & Source Link

**Theme:** Packaging · Discoverability · Quality

Inspired by [a metadata-completeness issue on Mongo.SignalR.Backplane](https://github.com/gottscj/Mongo.SignalR.Backplane/issues/5):
the same class of gaps applies to the `WLED` and `WLED.DependencyInjection` packages.

## Why

A NuGet package is a product page. When key metadata is missing, consumers lose trust and
tooling loses links:

- no **Source repository** link back to GitHub,
- no **Project website** link,
- no **README** rendered on the package page (NuGet renders `PackageReadmeFile`),
- and — for a great debugging experience — **Source Link** lets consumers step into the
  library's source straight from their debugger.

The two packable projects (`src/Kevsoft.WLED`, `src/Kevsoft.WLED.DependencyInjection`)
already set `IncludeSymbols`/`SymbolPackageFormat=snupkg`, `PublishRepositoryUrl`,
`EmbedUntrackedSources` and `PackageLicenseExpression=MIT`, but are missing the repository
URL, project URL, README packaging and the Source Link package — so the symbols/source
experience is incomplete and the package pages have no GitHub/README links.

## Current state (per `.csproj`)

Present: `PackageId`, `Title`, `PackageTags`, `PackageIcon`, `PackageLicenseExpression`,
`Description`, `IsPackable`, `EmbedUntrackedSources`, `IncludeSymbols`,
`SymbolPackageFormat=snupkg`, `PublishRepositoryUrl`. `Authors`/`Copyright`/`Product` come
from `Directory.Build.props`.

Missing:

- ❌ `RepositoryUrl` / `RepositoryType` → no "Source repository" link.
- ❌ `PackageProjectUrl` → no "Project website" link.
- ❌ `PackageReadmeFile` + packed `README.md` → README not rendered on nuget.org.
- ❌ `Microsoft.SourceLink.GitHub` → Source Link not wired up, so the published `.snupkg`
  can't map back to GitHub source.

## Approach

Shared identity goes in `Directory.Build.props` (applies to every project; harmless on the
non-packable test/sample projects and keeps the two packages consistent):

```xml
<RepositoryUrl>https://github.com/kevbite/WLED.NET</RepositoryUrl>
<RepositoryType>git</RepositoryType>
<PackageProjectUrl>https://github.com/kevbite/WLED.NET</PackageProjectUrl>
```

Per packable project (`Kevsoft.WLED.csproj`, `Kevsoft.WLED.DependencyInjection.csproj`):

```xml
<PackageReadmeFile>README.md</PackageReadmeFile>
...
<ItemGroup>
  <None Include="../../README.md" Pack="true" Visible="false" PackagePath="" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="Microsoft.SourceLink.GitHub" Version="8.0.0" PrivateAssets="All" />
</ItemGroup>
```

Notes / decisions:

- The repo root `README.md` is reused for both packages. It's the project shop window and is
  already kept current as part of every plan's Definition of Done.
- `Microsoft.SourceLink.GitHub` is added per packable project (not globally) so the
  build-time package isn't pulled into the test/sample restores. `PrivateAssets="All"` keeps
  it out of the consumer's dependency graph.
- `8.0.0` is the current stable Source Link package and supports all four target frameworks
  (incl. `netstandard2.0`).
- `EmbedUntrackedSources` + `PublishRepositoryUrl` are already set, so once Source Link is
  referenced the deterministic source mapping completes with no further config.

## Tests

This is a packaging change with no runtime surface, so there are no new unit tests. Verify by
**packing** instead:

- `dotnet pack src/Kevsoft.WLED/Kevsoft.WLED.csproj -c Release` produces both a `.nupkg` and a
  `.snupkg`.
- Inspect the generated `.nuspec` inside the `.nupkg` and confirm `<repository …>`,
  `<projectUrl>`, `<readme>README.md</readme>` and a packed `README.md` are present.
- Full `dotnet build`/`dotnet test` (all four TFMs) remain green.

## Definition of Done (per `plans/README.md`)

1. **README.md** — no consumer-facing API change; the README itself now ships in the package.
   No snippet change required.
2. **samples/BasicConsole** — unaffected.
3. **CHANGELOG.md** — record the packaging improvements under the unreleased section.

## Out of scope

- Changing the package icon, license file form (`PackageLicenseExpression=MIT` stays), or
  versioning scheme.
- Publishing/CI changes — the existing pipeline already pushes `.nupkg`/`.snupkg`.
