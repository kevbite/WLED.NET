namespace Kevsoft.WLED;

/// <summary>
/// A slider or checkbox control an effect uses, with its segment parameter key, label and value range.
/// </summary>
/// <param name="Key">The segment parameter key the control maps to (e.g. <c>sx</c>, <c>c3</c>, <c>o1</c>).</param>
/// <param name="Label">The display label, with effect-metadata defaults already applied.</param>
/// <param name="Minimum">The lowest valid value for the control.</param>
/// <param name="Maximum">The highest valid value for the control.</param>
public sealed record EffectControl(string Key, string Label, int Minimum, int Maximum);
