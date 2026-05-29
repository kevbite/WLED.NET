namespace Kevsoft.WLED;

/// <summary>
/// Selects a preset by id, by cycling through a range (<c>"from~to~"</c>) or by choosing a
/// random preset within a range (<c>"from~tor"</c>).
/// </summary>
[JsonConverter(typeof(PresetSelectorJsonConverter))]
public readonly struct PresetSelector : IEquatable<PresetSelector>
{
    internal enum Kind : byte
    {
        Id,
        Cycle,
        RandomInRange,
    }

    private PresetSelector(Kind kind, int from, int to)
    {
        Type = kind;
        From = from;
        To = to;
    }

    internal Kind Type { get; }

    internal int From { get; }

    internal int To { get; }

    /// <summary>Select a specific preset id.</summary>
    public static PresetSelector Id(int id) => new(Kind.Id, id, id);

    /// <summary>Cycle through presets from <paramref name="from"/> to <paramref name="to"/>.</summary>
    public static PresetSelector Cycle(int from, int to)
    {
        EnsureRange(from, to);
        return new(Kind.Cycle, from, to);
    }

    /// <summary>Select a random preset between <paramref name="from"/> and <paramref name="to"/>.</summary>
    public static PresetSelector RandomInRange(int from, int to)
    {
        EnsureRange(from, to);
        return new(Kind.RandomInRange, from, to);
    }

    public static implicit operator PresetSelector(int id) => Id(id);

    internal string ToToken() => Type switch
    {
        Kind.Id => From.ToString(System.Globalization.CultureInfo.InvariantCulture),
        Kind.Cycle => $"{From}~{To}~",
        Kind.RandomInRange => $"{From}~{To}r",
        _ => throw new InvalidOperationException(),
    };

    private static void EnsureRange(int from, int to)
    {
        if (to < from)
        {
            throw new ArgumentException($"'to' ({to}) must be greater than or equal to 'from' ({from}).", nameof(to));
        }
    }

    public bool Equals(PresetSelector other) => Type == other.Type && From == other.From && To == other.To;

    public override bool Equals(object? obj) => obj is PresetSelector other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = (int)Type;
            hash = (hash * 397) ^ From;
            hash = (hash * 397) ^ To;
            return hash;
        }
    }

    public override string ToString() => ToToken();

    public static bool operator ==(PresetSelector left, PresetSelector right) => left.Equals(right);

    public static bool operator !=(PresetSelector left, PresetSelector right) => !left.Equals(right);
}
