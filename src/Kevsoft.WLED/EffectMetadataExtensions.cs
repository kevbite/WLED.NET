namespace Kevsoft.WLED;

/// <summary>
/// Lookup helpers over a collection of <see cref="EffectMetadata"/>, such as the result of
/// <see cref="IWLedClient.GetEffectMetadata"/>.
/// </summary>
public static class EffectMetadataExtensions
{
    /// <summary>Finds the metadata for the effect with the given id, or throws if there is none.</summary>
    public static EffectMetadata FindById(this IReadOnlyList<EffectMetadata> metadata, int effectId)
        => metadata.TryFindById(effectId, out var found)
            ? found
            : throw new KeyNotFoundException($"No effect metadata with id {effectId} exists.");

    /// <summary>Finds the metadata for the effect with the given id without throwing.</summary>
    public static bool TryFindById(this IReadOnlyList<EffectMetadata> metadata, int effectId, out EffectMetadata found)
    {
        if (metadata is null)
        {
            throw new ArgumentNullException(nameof(metadata));
        }

        foreach (var candidate in metadata)
        {
            if (candidate.EffectId == effectId)
            {
                found = candidate;
                return true;
            }
        }

        found = null!;
        return false;
    }

    /// <summary>
    /// Finds the single metadata entry with the given effect name (case-insensitive, ordinal). Throws
    /// if no entry, or more than one entry, matches.
    /// </summary>
    public static EffectMetadata FindByName(this IReadOnlyList<EffectMetadata> metadata, string name)
    {
        if (metadata is null)
        {
            throw new ArgumentNullException(nameof(metadata));
        }

        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        EffectMetadata? match = null;
        var count = 0;

        foreach (var candidate in metadata)
        {
            if (string.Equals(candidate.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                match = candidate;
                count++;
            }
        }

        return count switch
        {
            0 => throw new KeyNotFoundException($"No effect metadata named '{name}' exists."),
            1 => match!,
            _ => throw new InvalidOperationException($"More than one effect metadata named '{name}' exists."),
        };
    }

    /// <summary>
    /// Finds the single metadata entry with the given effect name (case-insensitive, ordinal) without
    /// throwing. Returns <c>false</c> if no entry, or more than one entry, matches.
    /// </summary>
    public static bool TryFindByName(this IReadOnlyList<EffectMetadata> metadata, string name, out EffectMetadata found)
    {
        found = null!;
        if (metadata is null || name is null)
        {
            return false;
        }

        var matched = false;

        foreach (var candidate in metadata)
        {
            if (string.Equals(candidate.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                if (matched)
                {
                    found = null!;
                    return false;
                }

                found = candidate;
                matched = true;
            }
        }

        return matched;
    }
}
