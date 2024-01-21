using Godot;

/// <summary>
/// Exposes a <c>RandomNumberGenerator</c> instance.
/// </summary>
public static class Rng
{
    private static readonly RandomNumberGenerator _rng = new();

    /// <summary>
    /// Alias of <c>RandiRange</c> (inclusive int).
    /// </summary>
    public static int IntRange(int min, int max)
    {
        return _rng.RandiRange(min, max);
    }

    /// <summary>
    /// Returns a random Hex color, excluding Alpha component.
    /// </summary>
    public static string RandomHex()
    {
        return new Color(_rng.Randf(), _rng.Randf(), _rng.Randf(), 1).ToHtml(false);
    }
}
