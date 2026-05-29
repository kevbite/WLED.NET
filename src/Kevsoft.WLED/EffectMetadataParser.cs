namespace Kevsoft.WLED;

internal static class EffectMetadataParser
{
    private static readonly (string Key, string DefaultLabel, int Min, int Max)[] SliderSlots =
    {
        ("sx", "Effect speed", 0, 255),
        ("ix", "Effect intensity", 0, 255),
        ("c1", "Custom 1", 0, 255),
        ("c2", "Custom 2", 0, 255),
        ("c3", "Custom 3", 0, 31),
    };

    private static readonly (string Key, string DefaultLabel)[] OptionSlots =
    {
        ("o1", "Option 1"),
        ("o2", "Option 2"),
        ("o3", "Option 3"),
    };

    private static readonly (string Key, string DefaultLabel)[] ColorSlots =
    {
        ("Fx", "Fx"),
        ("Bg", "Bg"),
        ("Cs", "Cs"),
    };

    public static IReadOnlyList<EffectMetadata> Parse(string[] fxdata, string[] effectNames)
    {
        var result = new List<EffectMetadata>(fxdata.Length);

        for (var id = 0; id < fxdata.Length; id++)
        {
            var name = id < effectNames.Length ? effectNames[id] : string.Empty;

            if (IsReserved(name))
            {
                continue;
            }

            result.Add(ParseSingle(id, name, fxdata[id]));
        }

        return result;
    }

    private static bool IsReserved(string name)
        => name == "RSVD" || name == "-";

    private static EffectMetadata ParseSingle(int id, string name, string data)
    {
        var sections = data.Split(';');

        var (sliders, options) = ParseParameters(SectionOrNull(sections, 0));
        var colors = ParseColors(SectionOrNull(sections, 1));
        var usesPalette = ParsePalette(SectionOrNull(sections, 2));
        var (dimensionality, volume, frequency) = ParseFlags(SectionOrNull(sections, 3));
        var defaults = ParseDefaults(SectionOrNull(sections, 4));

        return new EffectMetadata(id, name, sliders, options, colors, usesPalette, dimensionality, volume, frequency, defaults);
    }

    private static string? SectionOrNull(string[] sections, int index)
        => index < sections.Length ? sections[index] : null;

    private static (IReadOnlyList<EffectControl> Sliders, IReadOnlyList<EffectControl> Options) ParseParameters(string? section)
    {
        // A missing section falls back to two sliders; a present-but-empty section means no controls.
        if (section is null)
        {
            return (new[]
            {
                new EffectControl(SliderSlots[0].Key, SliderSlots[0].DefaultLabel, SliderSlots[0].Min, SliderSlots[0].Max),
                new EffectControl(SliderSlots[1].Key, SliderSlots[1].DefaultLabel, SliderSlots[1].Min, SliderSlots[1].Max),
            }, Array.Empty<EffectControl>());
        }

        var sliders = new List<EffectControl>();
        var options = new List<EffectControl>();

        if (section.Length == 0)
        {
            return (sliders, options);
        }

        var tokens = section.Split(',');

        for (var i = 0; i < tokens.Length; i++)
        {
            var label = tokens[i];
            if (label.Length == 0)
            {
                continue;
            }

            if (i < SliderSlots.Length)
            {
                var slot = SliderSlots[i];
                sliders.Add(new EffectControl(slot.Key, Label(label, slot.DefaultLabel), slot.Min, slot.Max));
            }
            else if (i - SliderSlots.Length < OptionSlots.Length)
            {
                var slot = OptionSlots[i - SliderSlots.Length];
                options.Add(new EffectControl(slot.Key, Label(label, slot.DefaultLabel), 0, 1));
            }
        }

        return (sliders, options);
    }

    private static IReadOnlyList<EffectColorSlot> ParseColors(string? section)
    {
        if (section is null)
        {
            return new[]
            {
                new EffectColorSlot(ColorSlots[0].Key, ColorSlots[0].DefaultLabel),
                new EffectColorSlot(ColorSlots[1].Key, ColorSlots[1].DefaultLabel),
                new EffectColorSlot(ColorSlots[2].Key, ColorSlots[2].DefaultLabel),
            };
        }

        var colors = new List<EffectColorSlot>();

        if (section.Length == 0)
        {
            return colors;
        }

        var tokens = section.Split(',');

        for (var i = 0; i < tokens.Length && i < ColorSlots.Length; i++)
        {
            var label = tokens[i];
            if (label.Length == 0)
            {
                continue;
            }

            colors.Add(new EffectColorSlot(ColorSlots[i].Key, Label(label, ColorSlots[i].DefaultLabel)));
        }

        return colors;
    }

    private static bool ParsePalette(string? section)
    {
        // Fallback (missing) is enabled; present-but-empty means no palette.
        if (section is null)
        {
            return true;
        }

        return section.Length > 0;
    }

    private static (EffectDimensionality Dimensionality, bool Volume, bool Frequency) ParseFlags(string? section)
    {
        if (string.IsNullOrEmpty(section))
        {
            return (EffectDimensionality.OneDimensional, false, false);
        }

        var volume = false;
        var frequency = false;
        bool single = false, oneD = false, twoD = false, threeD = false;

        foreach (var flag in section!)
        {
            switch (flag)
            {
                case '0': single = true; break;
                case '1': oneD = true; break;
                case '2': twoD = true; break;
                case '3': threeD = true; break;
                case 'v': volume = true; break;
                case 'f': frequency = true; break;
            }
        }

        var dimensionality = single ? EffectDimensionality.SingleLed
            : oneD ? EffectDimensionality.OneDimensional
            : twoD ? EffectDimensionality.TwoDimensional
            : threeD ? EffectDimensionality.ThreeDimensional
            : EffectDimensionality.OneDimensional;

        return (dimensionality, volume, frequency);
    }

    private static IReadOnlyDictionary<string, int> ParseDefaults(string? section)
    {
        var defaults = new Dictionary<string, int>();

        if (string.IsNullOrEmpty(section))
        {
            return defaults;
        }

        foreach (var pair in section!.Split(','))
        {
            var separator = pair.IndexOf('=');
            if (separator <= 0)
            {
                continue;
            }

            var key = pair.Substring(0, separator);
            var value = pair.Substring(separator + 1);

            if (int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
            {
                defaults[key] = parsed;
            }
        }

        return defaults;
    }

    private static string Label(string label, string defaultLabel)
        => label == "!" ? defaultLabel : label;
}
