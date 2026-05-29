# Plan 7 — Playlists API

**Theme:** Feature · **Depends on:** Plans 2, 6

## Why

Playlists (available since 0.11.0) cycle presets with per-step durations and transitions.
The wire format is awkward — three *parallel arrays* (`ps`, `dur`, `transition`) plus
`repeat`/`end` — which is exactly the kind of thing we should never make a caller
assemble by hand.

## API surface (WLED)

```jsonc
{ "playlist": {
    "ps": [26, 20, 18, 20],     // preset ids, in order
    "dur": [30, 20, 10, 50],    // tenths of a second (scalar broadcasts to all)
    "transition": 0,            // tenths of a second (scalar or array)
    "repeat": 10,               // 0 = forever
    "end": 21                   // preset to apply when finished
} }
```

Playlists are also stored in `presets.json` (a preset whose `playlist.ps` is non-empty).

## What we build — make the parallel arrays unrepresentable

Model a playlist as a list of **entries**, each a self-contained step. The library zips
them into/out of the parallel-array wire format (mirrors `frenck` `Playlist`/
`PlaylistEntry`).

```csharp
public sealed record PlaylistEntry(
    int PresetId,
    TimeSpan Duration,                       // serialized to tenths-of-second
    TimeSpan? Transition = null);

public sealed class PlaylistDefinition
{
    public IReadOnlyList<PlaylistEntry> Entries { get; init; } = [];
    public int Repeat { get; init; } = 0;    // 0 = indefinite
    public int? EndPresetId { get; init; }
    public bool Shuffle { get; init; }       // "r"
}

public sealed record Playlist(int Id, string Name, PlaylistDefinition Definition);
```

Because each `PlaylistEntry` couples its own preset/duration/transition, the
"arrays of different lengths" bug class is gone.

### Client methods

```csharp
Task<IReadOnlyDictionary<int, Playlist>> GetPlaylists(CancellationToken ct = default);
Task StartPlaylist(PlaylistDefinition playlist, CancellationToken ct = default);
Task SavePlaylist(int id, PlaylistDefinition playlist, SavePresetOptions? o = null, CancellationToken ct = default);
```

- `StartPlaylist` builds `{"playlist": {...}}` by **unzipping** entries into `ps`/`dur`/
  `transition` arrays. A `PlaylistRequest` DTO + converter owns this transformation.
- `SavePlaylist` combines the playlist with a `psave` (reuses Plan 6 plumbing).
- `GetPlaylists` reuses the `presets.json` parse from Plan 6, taking the playlist branch.

### Fluent helper (optional, nice)

```csharp
await client.StartPlaylist(p => p
    .Add(preset: 26, TimeSpan.FromSeconds(3))
    .Add(preset: 20, TimeSpan.FromSeconds(2), transition: TimeSpan.FromMilliseconds(700))
    .Repeat(10)
    .EndOn(21));
```

## Files

- `src/Kevsoft.WLED/{PlaylistEntry,PlaylistDefinition,Playlist}.cs`
- `src/Kevsoft.WLED/PlaylistRequest.cs` + `Json/PlaylistConverter.cs`
- Add `Playlist` to `StateRequest.cs` (the `playlist` key)
- `IWLedClient` + `WLedClient`: `GetPlaylists`, `StartPlaylist`, `SavePlaylist`

## Tests

- `PlaylistDefinition` with 4 entries → exact `ps`/`dur`/`transition` arrays (tenths).
- Scalar `dur`/`transition` from device → broadcast to all entries on read.
- Round-trip a real `presets.json` playlist entry into `Playlist` and back.
- `Repeat(0)` (indefinite) and `EndOn` behaviour.

## Acceptance

Playlists are created and read as ordered lists of typed entries; the library fully owns
the parallel-array ⇄ entries transformation, so callers cannot produce mismatched arrays.
