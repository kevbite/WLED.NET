namespace Kevsoft.WLED;

/// <summary>
/// Parsed metadata for a single effect, describing the controls it actually uses.
/// </summary>
/// <param name="EffectId">The effect's id, matching its index in the effects list.</param>
/// <param name="Name">The effect's display name.</param>
/// <param name="Sliders">The slider controls the effect uses (a subset of <c>sx</c>, <c>ix</c>, <c>c1</c>, <c>c2</c>, <c>c3</c>).</param>
/// <param name="Options">The checkbox controls the effect uses (a subset of <c>o1</c>, <c>o2</c>, <c>o3</c>).</param>
/// <param name="Colors">The colour slots the effect uses (a subset of <c>Fx</c>, <c>Bg</c>, <c>Cs</c>).</param>
/// <param name="UsesPalette"><c>true</c> if palette selection is enabled for the effect.</param>
/// <param name="Dimensionality">The dimensionality the effect is designed for.</param>
/// <param name="ReactsToVolume"><c>true</c> if the effect reacts to audio amplitude/volume.</param>
/// <param name="ReactsToFrequency"><c>true</c> if the effect reacts to the audio frequency distribution.</param>
/// <param name="Defaults">Recommended default values for segment parameters, keyed by parameter name.</param>
public sealed record EffectMetadata(
    int EffectId,
    string Name,
    IReadOnlyList<EffectControl> Sliders,
    IReadOnlyList<EffectControl> Options,
    IReadOnlyList<EffectColorSlot> Colors,
    bool UsesPalette,
    EffectDimensionality Dimensionality,
    bool ReactsToVolume,
    bool ReactsToFrequency,
    IReadOnlyDictionary<string, int> Defaults)
{
    /// <summary><c>true</c> if the effect is audio reactive (to either volume or frequency).</summary>
    public bool IsAudioReactive => ReactsToVolume || ReactsToFrequency;

    /// <summary>
    /// Returns <c>true</c> if the effect uses the given control. Setting a control the effect
    /// does not use has no visible result.
    /// </summary>
    public bool Supports(SegmentControl control) => control switch
    {
        SegmentControl.Speed => HasSlider("sx"),
        SegmentControl.Intensity => HasSlider("ix"),
        SegmentControl.Custom1 => HasSlider("c1"),
        SegmentControl.Custom2 => HasSlider("c2"),
        SegmentControl.Custom3 => HasSlider("c3"),
        SegmentControl.Option1 => HasOption("o1"),
        SegmentControl.Option2 => HasOption("o2"),
        SegmentControl.Option3 => HasOption("o3"),
        SegmentControl.Color1 => HasColor("Fx"),
        SegmentControl.Color2 => HasColor("Bg"),
        SegmentControl.Color3 => HasColor("Cs"),
        SegmentControl.Palette => UsesPalette,
        _ => false
    };

    private bool HasSlider(string key)
    {
        foreach (var slider in Sliders)
        {
            if (slider.Key == key)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasOption(string key)
    {
        foreach (var option in Options)
        {
            if (option.Key == key)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasColor(string key)
    {
        foreach (var color in Colors)
        {
            if (color.Key == key)
            {
                return true;
            }
        }

        return false;
    }
}
