namespace Kevsoft.WLED;

/// <summary>
/// A colour slot an effect uses, with its slot key and display label.
/// </summary>
/// <param name="Key">The colour slot key (<c>Fx</c>, <c>Bg</c> or <c>Cs</c>).</param>
/// <param name="Label">The display label, with effect-metadata defaults already applied.</param>
public sealed record EffectColorSlot(string Key, string Label);
