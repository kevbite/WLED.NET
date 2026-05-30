namespace Kevsoft.WLED;

/// <summary>
/// Fluent builder for addressing individual LEDs within a segment.
/// </summary>
/// <remarks>
/// Three intents are supported, mirroring the WLED <c>i</c> property:
/// <list type="bullet">
/// <item><see cref="Set(Color[])"/> — colours applied sequentially from the start of the segment.</item>
/// <item><see cref="Set(int, Color)"/> — a single LED at a segment-relative index.</item>
/// <item><see cref="SetRange(int, int, Color)"/> — a contiguous run of LEDs.</item>
/// </list>
/// Indices are segment-relative. The builder owns the wire encoding (preferring compact hex over byte arrays)
/// and, via the client, the sequential chunking required for large sets.
/// </remarks>
public sealed class IndividualLedBuilder
{
    private readonly List<IIndividualLedOp> _ops = new();

    /// <summary>Set LEDs sequentially starting at the first LED of the segment.</summary>
    public IndividualLedBuilder Set(params Color[] sequentialFromStart)
    {
        if (sequentialFromStart is null)
        {
            throw new ArgumentNullException(nameof(sequentialFromStart));
        }

        if (sequentialFromStart.Length > 0)
        {
            _ops.Add(new SequentialOp(sequentialFromStart));
        }

        return this;
    }

    /// <summary>Set a single LED at the given segment-relative <paramref name="index"/>.</summary>
    public IndividualLedBuilder Set(int index, Color color)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, "LED index must not be negative.");
        }

        _ops.Add(new IndexedOp(index, color));
        return this;
    }

    /// <summary>Set a contiguous range of LEDs from <paramref name="start"/> (inclusive) to <paramref name="stopExclusive"/> (exclusive).</summary>
    public IndividualLedBuilder SetRange(int start, int stopExclusive, Color color)
    {
        if (start < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(start), start, "Range start must not be negative.");
        }

        if (stopExclusive <= start)
        {
            throw new ArgumentOutOfRangeException(nameof(stopExclusive), stopExclusive, "Range stop must be greater than start.");
        }

        _ops.Add(new RangeOp(start, stopExclusive, color));
        return this;
    }

    /// <summary>
    /// Encode the accumulated operations into one or more requests, each containing at most
    /// <paramref name="maxColorsPerRequest"/> colours.
    /// </summary>
    internal IReadOnlyList<IndividualLedData> Build(int maxColorsPerRequest)
    {
        if (maxColorsPerRequest < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxColorsPerRequest), maxColorsPerRequest, "At least one colour per request is required.");
        }

        var requests = new List<IndividualLedData>();
        var current = new List<IndividualLedToken>();
        var colorCount = 0;

        void Flush()
        {
            if (current.Count > 0)
            {
                requests.Add(new IndividualLedData(current.ToArray()));
                current = new List<IndividualLedToken>();
                colorCount = 0;
            }
        }

        foreach (var op in _ops)
        {
            switch (op)
            {
                case SequentialOp sequential:
                    var pos = 0;
                    while (pos < sequential.Colors.Count)
                    {
                        if (colorCount == maxColorsPerRequest)
                        {
                            Flush();
                        }

                        var take = Math.Min(maxColorsPerRequest - colorCount, sequential.Colors.Count - pos);

                        // A piece needs an explicit start index unless it begins at LED 0 of an empty request.
                        if (pos != 0 || current.Count > 0)
                        {
                            current.Add(IndividualLedToken.FromIndex(pos));
                        }

                        for (var i = 0; i < take; i++)
                        {
                            current.Add(IndividualLedToken.FromColor(sequential.Colors[pos + i]));
                        }

                        colorCount += take;
                        pos += take;
                    }

                    break;

                case IndexedOp indexed:
                    if (colorCount + 1 > maxColorsPerRequest)
                    {
                        Flush();
                    }

                    current.Add(IndividualLedToken.FromIndex(indexed.Index));
                    current.Add(IndividualLedToken.FromColor(indexed.Color));
                    colorCount++;
                    break;

                case RangeOp range:
                    if (colorCount + 1 > maxColorsPerRequest)
                    {
                        Flush();
                    }

                    current.Add(IndividualLedToken.FromIndex(range.Start));
                    current.Add(IndividualLedToken.FromIndex(range.StopExclusive));
                    current.Add(IndividualLedToken.FromColor(range.Color));
                    colorCount++;
                    break;
            }
        }

        Flush();

        return requests;
    }

    private interface IIndividualLedOp
    {
    }

    private sealed class SequentialOp : IIndividualLedOp
    {
        public SequentialOp(IReadOnlyList<Color> colors) => Colors = colors;

        public IReadOnlyList<Color> Colors { get; }
    }

    private sealed class IndexedOp : IIndividualLedOp
    {
        public IndexedOp(int index, Color color)
        {
            Index = index;
            Color = color;
        }

        public int Index { get; }

        public Color Color { get; }
    }

    private sealed class RangeOp : IIndividualLedOp
    {
        public RangeOp(int start, int stopExclusive, Color color)
        {
            Start = start;
            StopExclusive = stopExclusive;
            Color = color;
        }

        public int Start { get; }

        public int StopExclusive { get; }

        public Color Color { get; }
    }
}
