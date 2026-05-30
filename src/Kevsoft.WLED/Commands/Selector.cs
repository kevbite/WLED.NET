namespace Kevsoft.WLED;

/// <summary>
/// Selects an effect or palette by id, by relative movement (<c>"~"</c>/<c>"~-"</c>),
/// at random (<c>"r"</c>) or at random within a range (<c>"from~tor"</c>).
/// </summary>
[JsonConverter(typeof(SelectorJsonConverter))]
public readonly struct Selector : IEquatable<Selector>
{
    internal enum Kind : byte
    {
        Id,
        Next,
        Previous,
        Random,
        RandomInRange,
    }

    private Selector(Kind kind, int id)
        : this(kind, id, id)
    {
    }

    private Selector(Kind kind, int from, int to)
    {
        Type = kind;
        From = from;
        To = to;
    }

    internal Kind Type { get; }

    internal int From { get; }

    internal int To { get; }

    /// <summary>The selected id (only meaningful when this is an <see cref="Kind.Id"/> selector).</summary>
    internal int IdValue => From;

    /// <summary>Select a specific id.</summary>
    public static Selector Id(int id) => new(Kind.Id, id);

    /// <summary>Select the next entry.</summary>
    public static Selector Next => new(Kind.Next, 0);

    /// <summary>Select the previous entry.</summary>
    public static Selector Previous => new(Kind.Previous, 0);

    /// <summary>Select a random entry.</summary>
    public static Selector Random => new(Kind.Random, 0);

    /// <summary>Select a random entry between <paramref name="from"/> and <paramref name="to"/> (inclusive).</summary>
    public static Selector RandomInRange(int from, int to)
    {
        if (to < from)
        {
            throw new ArgumentException($"'to' ({to}) must be greater than or equal to 'from' ({from}).", nameof(to));
        }

        return new Selector(Kind.RandomInRange, from, to);
    }

    public static implicit operator Selector(int id) => Id(id);

    public bool Equals(Selector other) => Type == other.Type && From == other.From && To == other.To;

    public override bool Equals(object? obj) => obj is Selector other && Equals(other);

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

    public override string ToString() => Type switch
    {
        Kind.Id => From.ToString(System.Globalization.CultureInfo.InvariantCulture),
        Kind.Next => "~",
        Kind.Previous => "~-",
        Kind.Random => "r",
        Kind.RandomInRange => $"{From}~{To}r",
        _ => throw new InvalidOperationException(),
    };

    public static bool operator ==(Selector left, Selector right) => left.Equals(right);

    public static bool operator !=(Selector left, Selector right) => !left.Equals(right);
}
