using System;
using Godot;

/// <summary>
/// Exposes a <c>Random</c> instance.
/// </summary>
public static class Rng
{
    private static readonly Random _rng = new();

    /// <summary>
    /// Alias of <c>RandiRange</c> (inclusive int).
    /// </summary>
    public static int IntRange(int min, int max)
    {
        return _rng.Next(min, max);
    }

    /// <summary>
    /// Returns a random Hex color, excluding Alpha component.
    /// </summary>
    public static string RandomHex()
    {
        return new Color(_rng.NextSingle(), _rng.NextSingle(), _rng.NextSingle(), 1).ToHtml(false);
    }
}
