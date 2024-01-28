using System;
using System.Security.Cryptography;

public static class Rng
{
    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    /// <summary>
    /// Returns a random integer from specified range (right-inclusive).
    /// </summary>
    public static int IntRange(int min, int max)
    {
        uint range = (uint)(max - min + 1);
        uint number = GetNumber() % range;

        return (int)(number + min);
    }

    /// <summary>
    /// Returns a random float between 0 and 1.
    /// </summary>
    public static float RandomFloat()
    {
        return (float)GetNumber() / uint.MaxValue;
    }

    /// <summary>
    /// Returns a random Godot Color.
    /// </summary>
    public static string RandomHex()
    {
        return new Godot.Color(RandomFloat(), RandomFloat(), RandomFloat(), 1).ToHtml(false);
    }

    private static uint GetNumber()
    {
        byte[] bytes = new byte[4];

        // Fill the bytes with random sequence
        _rng.GetBytes(bytes);

        return BitConverter.ToUInt32(bytes, 0);
    }
}
