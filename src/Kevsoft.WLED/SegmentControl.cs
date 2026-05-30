namespace Kevsoft.WLED;

/// <summary>
/// A single segment control an effect exposes, used to query effect metadata.
/// </summary>
public enum SegmentControl
{
    /// <summary>Effect speed slider (<c>sx</c>).</summary>
    Speed,

    /// <summary>Effect intensity slider (<c>ix</c>).</summary>
    Intensity,

    /// <summary>Custom slider 1 (<c>c1</c>).</summary>
    Custom1,

    /// <summary>Custom slider 2 (<c>c2</c>).</summary>
    Custom2,

    /// <summary>Custom slider 3 (<c>c3</c>).</summary>
    Custom3,

    /// <summary>Option checkbox 1 (<c>o1</c>).</summary>
    Option1,

    /// <summary>Option checkbox 2 (<c>o2</c>).</summary>
    Option2,

    /// <summary>Option checkbox 3 (<c>o3</c>).</summary>
    Option3,

    /// <summary>Primary colour slot (<c>Fx</c>).</summary>
    Color1,

    /// <summary>Background colour slot (<c>Bg</c>).</summary>
    Color2,

    /// <summary>Custom colour slot (<c>Cs</c>).</summary>
    Color3,

    /// <summary>Palette selection.</summary>
    Palette,
}
