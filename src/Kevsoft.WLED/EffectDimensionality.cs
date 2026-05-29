namespace Kevsoft.WLED;

/// <summary>
/// The dimensionality an effect is designed for, derived from the effect metadata flags.
/// </summary>
public enum EffectDimensionality
{
    /// <summary>Optimised for 1D LED strips (flag <c>1</c>, and the default).</summary>
    OneDimensional,

    /// <summary>Requires a 2D matrix (flag <c>2</c>).</summary>
    TwoDimensional,

    /// <summary>Requires a 3D cube (flag <c>3</c>).</summary>
    ThreeDimensional,

    /// <summary>Works well on a single LED (flag <c>0</c>).</summary>
    SingleLed,
}
