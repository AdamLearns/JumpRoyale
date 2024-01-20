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
}
