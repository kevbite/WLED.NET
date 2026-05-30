using System.Collections;

namespace Kevsoft.WLED;

/// <summary>
/// A single entry in an effect or palette catalog: its id and display name.
/// </summary>
/// <param name="Id">The zero-based id, matching the index in the WLED effects/palettes list.</param>
/// <param name="Name">The display name.</param>
public abstract record CatalogEntry(int Id, string Name)
{
    /// <summary>
    /// <c>true</c> if this entry is a reserved placeholder (named <c>RSVD</c> or <c>-</c>). WLED keeps
    /// these so ids stay stable across builds; selecting one falls back to the Solid effect.
    /// </summary>
    public bool IsReserved => Name == "RSVD" || Name == "-";

    /// <summary><c>true</c> if this entry is selectable (i.e. not a reserved placeholder).</summary>
    public bool IsUsable => !IsReserved;

    public override string ToString() => $"{Id}: {Name}";
}

/// <summary>A single effect in the <see cref="EffectCatalog"/>.</summary>
public sealed record EffectCatalogEntry(int Id, string Name) : CatalogEntry(Id, Name);

/// <summary>A single palette in the <see cref="PaletteCatalog"/>.</summary>
public sealed record PaletteCatalogEntry(int Id, string Name) : CatalogEntry(Id, Name);

/// <summary>
/// A read-only, id-aligned catalog of effects or palettes that supports lookup by id or name and
/// filtering out reserved placeholder entries.
/// </summary>
/// <typeparam name="TEntry">The concrete entry type.</typeparam>
public abstract class Catalog<TEntry> : IReadOnlyList<TEntry>
    where TEntry : CatalogEntry
{
    private readonly IReadOnlyList<TEntry> _entries;

    private protected Catalog(IReadOnlyList<TEntry> entries)
        => _entries = entries ?? throw new ArgumentNullException(nameof(entries));

    /// <summary>The entry at the given list position (which equals its id).</summary>
    public TEntry this[int index] => _entries[index];

    /// <summary>The total number of entries, including reserved placeholders.</summary>
    public int Count => _entries.Count;

    /// <summary>Returns only the selectable entries, excluding reserved placeholders.</summary>
    public IEnumerable<TEntry> AvailableOnly()
    {
        foreach (var entry in _entries)
        {
            if (entry.IsUsable)
            {
                yield return entry;
            }
        }
    }

    /// <summary>Finds the entry with the given id, or throws if there is none.</summary>
    public TEntry FindById(int id)
        => TryFindById(id, out var entry)
            ? entry
            : throw new KeyNotFoundException($"No entry with id {id} exists in the catalog.");

    /// <summary>Finds the entry with the given id without throwing.</summary>
    public bool TryFindById(int id, out TEntry entry)
    {
        foreach (var candidate in _entries)
        {
            if (candidate.Id == id)
            {
                entry = candidate;
                return true;
            }
        }

        entry = null!;
        return false;
    }

    /// <summary>
    /// Finds the single entry with the given name (case-insensitive, ordinal). Throws if no entry
    /// or more than one entry matches.
    /// </summary>
    public TEntry FindByName(string name)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        TEntry? match = null;
        var count = 0;

        foreach (var candidate in _entries)
        {
            if (string.Equals(candidate.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                match = candidate;
                count++;
            }
        }

        return count switch
        {
            0 => throw new KeyNotFoundException($"No entry named '{name}' exists in the catalog."),
            1 => match!,
            _ => throw new InvalidOperationException($"More than one entry named '{name}' exists in the catalog."),
        };
    }

    /// <summary>
    /// Finds the single entry with the given name (case-insensitive, ordinal) without throwing.
    /// Returns <c>false</c> if no entry, or more than one entry, matches.
    /// </summary>
    public bool TryFindByName(string name, out TEntry entry)
    {
        entry = null!;
        if (name is null)
        {
            return false;
        }

        var found = false;

        foreach (var candidate in _entries)
        {
            if (string.Equals(candidate.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                if (found)
                {
                    entry = null!;
                    return false;
                }

                entry = candidate;
                found = true;
            }
        }

        return found;
    }

    public IEnumerator<TEntry> GetEnumerator() => _entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// The device's effects, by id and name. Reserved placeholders (<c>RSVD</c>/<c>-</c>) are included so
/// ids stay aligned; use <see cref="Catalog{TEntry}.AvailableOnly"/> to skip them.
/// </summary>
public sealed class EffectCatalog : Catalog<EffectCatalogEntry>
{
    private EffectCatalog(IReadOnlyList<EffectCatalogEntry> entries) : base(entries)
    {
    }

    /// <summary>Builds an effect catalog from the raw <c>/json/eff</c> names.</summary>
    public static EffectCatalog FromNames(IReadOnlyList<string> names)
    {
        if (names is null)
        {
            throw new ArgumentNullException(nameof(names));
        }

        var entries = new EffectCatalogEntry[names.Count];
        for (var id = 0; id < names.Count; id++)
        {
            entries[id] = new EffectCatalogEntry(id, names[id]);
        }

        return new EffectCatalog(entries);
    }
}

/// <summary>
/// The device's palettes, by id and name. Reserved placeholders (<c>RSVD</c>/<c>-</c>) are included so
/// ids stay aligned; use <see cref="Catalog{TEntry}.AvailableOnly"/> to skip them.
/// </summary>
public sealed class PaletteCatalog : Catalog<PaletteCatalogEntry>
{
    private PaletteCatalog(IReadOnlyList<PaletteCatalogEntry> entries) : base(entries)
    {
    }

    /// <summary>Builds a palette catalog from the raw <c>/json/pal</c> names.</summary>
    public static PaletteCatalog FromNames(IReadOnlyList<string> names)
    {
        if (names is null)
        {
            throw new ArgumentNullException(nameof(names));
        }

        var entries = new PaletteCatalogEntry[names.Count];
        for (var id = 0; id < names.Count; id++)
        {
            entries[id] = new PaletteCatalogEntry(id, names[id]);
        }

        return new PaletteCatalog(entries);
    }
}
