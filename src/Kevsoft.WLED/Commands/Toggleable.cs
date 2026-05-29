namespace Kevsoft.WLED;

/// <summary>
/// A boolean command that can also represent a "toggle" instruction, matching WLED's
/// acceptance of <c>true</c>, <c>false</c> or <c>"t"</c> for on/off style fields.
/// </summary>
[JsonConverter(typeof(ToggleableJsonConverter))]
public readonly struct Toggleable : IEquatable<Toggleable>
{
    private readonly byte _kind;

    private Toggleable(byte kind) => _kind = kind;

    /// <summary>Turn off.</summary>
    public static Toggleable Off => new(0);

    /// <summary>Turn on.</summary>
    public static Toggleable On => new(1);

    /// <summary>Toggle the current state.</summary>
    public static Toggleable Toggle => new(2);

    /// <summary><c>true</c> if this is the toggle instruction.</summary>
    public bool IsToggle => _kind == 2;

    /// <summary>The boolean value, or <c>null</c> when this is a toggle instruction.</summary>
    public bool? Value => _kind switch
    {
        0 => false,
        1 => true,
        _ => null,
    };

    public static implicit operator Toggleable(bool value) => value ? On : Off;

    public bool Equals(Toggleable other) => _kind == other._kind;

    public override bool Equals(object? obj) => obj is Toggleable other && Equals(other);

    public override int GetHashCode() => _kind;

    public override string ToString() => IsToggle ? "Toggle" : Value!.Value ? "On" : "Off";

    public static bool operator ==(Toggleable left, Toggleable right) => left.Equals(right);

    public static bool operator !=(Toggleable left, Toggleable right) => !left.Equals(right);
}
