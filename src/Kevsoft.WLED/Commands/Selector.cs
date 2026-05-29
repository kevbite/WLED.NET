namespace Kevsoft.WLED;

/// <summary>
/// Selects an effect or palette by id, by relative movement (<c>"~"</c>/<c>"~-"</c>) or
/// at random (<c>"r"</c>).
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
    }

    private Selector(Kind kind, int id)
    {
        Type = kind;
        IdValue = id;
    }

    internal Kind Type { get; }

    internal int IdValue { get; }

    /// <summary>Select a specific id.</summary>
    public static Selector Id(int id) => new(Kind.Id, id);

    /// <summary>Select the next entry.</summary>
    public static Selector Next => new(Kind.Next, 0);

    /// <summary>Select the previous entry.</summary>
    public static Selector Previous => new(Kind.Previous, 0);

    /// <summary>Select a random entry.</summary>
    public static Selector Random => new(Kind.Random, 0);

    public static implicit operator Selector(int id) => Id(id);

    public bool Equals(Selector other) => Type == other.Type && IdValue == other.IdValue;

    public override bool Equals(object? obj) => obj is Selector other && Equals(other);

    public override int GetHashCode() => ((int)Type * 397) ^ IdValue;

    public override string ToString() => Type switch
    {
        Kind.Id => IdValue.ToString(System.Globalization.CultureInfo.InvariantCulture),
        Kind.Next => "~",
        Kind.Previous => "~-",
        Kind.Random => "r",
        _ => throw new InvalidOperationException(),
    };

    public static bool operator ==(Selector left, Selector right) => left.Equals(right);

    public static bool operator !=(Selector left, Selector right) => !left.Equals(right);
}
