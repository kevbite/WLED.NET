namespace Kevsoft.WLED;

/// <summary>
/// A 0–255 value that can be set absolutely, nudged up/down, or incremented with wrap,
/// matching WLED's acceptance of a number, <c>"~"</c>/<c>"~-"</c>, <c>"~N"</c>/<c>"~-N"</c>
/// and <c>"wN"</c> tokens for brightness-style fields.
/// </summary>
[JsonConverter(typeof(ByteAdjustJsonConverter))]
public readonly struct ByteAdjust : IEquatable<ByteAdjust>
{
    internal enum Operation : byte
    {
        Set,
        Increment,
        Decrement,
        IncrementWrap,
    }

    private ByteAdjust(Operation operation, byte value)
    {
        Op = operation;
        Amount = value;
    }

    internal Operation Op { get; }

    internal byte Amount { get; }

    /// <summary>Set the value absolutely.</summary>
    public static ByteAdjust Set(byte value) => new(Operation.Set, value);

    /// <summary>Increase the value by <paramref name="by"/> (default 1).</summary>
    public static ByteAdjust Increment(byte by = 1) => new(Operation.Increment, by);

    /// <summary>Decrease the value by <paramref name="by"/> (default 1).</summary>
    public static ByteAdjust Decrement(byte by = 1) => new(Operation.Decrement, by);

    /// <summary>Increase the value by <paramref name="by"/>, wrapping around at the limit.</summary>
    public static ByteAdjust IncrementWrap(byte by) => new(Operation.IncrementWrap, by);

    public static implicit operator ByteAdjust(byte value) => Set(value);

    internal string ToToken() => Op switch
    {
        Operation.Set => Amount.ToString(System.Globalization.CultureInfo.InvariantCulture),
        Operation.Increment => Amount == 1 ? "~" : $"~{Amount}",
        Operation.Decrement => Amount == 1 ? "~-" : $"~-{Amount}",
        Operation.IncrementWrap => $"w~{Amount}",
        _ => throw new InvalidOperationException(),
    };

    public bool Equals(ByteAdjust other) => Op == other.Op && Amount == other.Amount;

    public override bool Equals(object? obj) => obj is ByteAdjust other && Equals(other);

    public override int GetHashCode() => ((int)Op * 397) ^ Amount;

    public override string ToString() => ToToken();

    public static bool operator ==(ByteAdjust left, ByteAdjust right) => left.Equals(right);

    public static bool operator !=(ByteAdjust left, ByteAdjust right) => !left.Equals(right);
}
