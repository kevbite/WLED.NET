namespace Kevsoft.WLED;

/// <summary>
/// A single, write-only batch of individual-LED assignments destined for a segment's <c>i</c> property.
/// </summary>
/// <remarks>
/// The WLED <c>i</c> array packs three different addressing forms (sequential, indexed and range) into one
/// heterogeneous JSON array. This type stores the already-encoded tokens for a single request so they can be
/// serialised verbatim; build instances through <see cref="IndividualLedBuilder"/> rather than by hand.
/// </remarks>
[JsonConverter(typeof(IndividualLedDataJsonConverter))]
public sealed class IndividualLedData
{
    internal IndividualLedData(IReadOnlyList<IndividualLedToken> tokens)
    {
        Tokens = tokens;
    }

    internal IReadOnlyList<IndividualLedToken> Tokens { get; }
}

/// <summary>A single token within an <see cref="IndividualLedData"/> array: either a positional index or a colour.</summary>
internal readonly struct IndividualLedToken
{
    private IndividualLedToken(int? index, Color? color)
    {
        Index = index;
        Color = color;
    }

    public int? Index { get; }

    public Color? Color { get; }

    public bool IsIndex => Index.HasValue;

    public static IndividualLedToken FromIndex(int index) => new(index, null);

    public static IndividualLedToken FromColor(Color color) => new(null, color);
}
