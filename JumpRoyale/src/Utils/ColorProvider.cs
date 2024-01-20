using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

public static class ColorProvider
{
    private static readonly Dictionary<string, string> _colors = [];

    static ColorProvider()
    {
        _colors.Add("red", "ff0000");
        _colors.Add("orange", "ffa500");
        _colors.Add("yellow", "ffff00");
        _colors.Add("green", "008000");
        _colors.Add("cyan", "00ffff");
        _colors.Add("blue", "0000ff");
        _colors.Add("magenta", "ff00ff");
        _colors.Add("purple", "800080");
        _colors.Add("white", "ffffff");
        _colors.Add("black", "000000");
        _colors.Add("grey", "808080");
        _colors.Add("gray", "808080");
        _colors.Add("silver", "c0c0c0");
        _colors.Add("pink", "ffc0cb");
        _colors.Add("maroon", "800000");
        _colors.Add("brown", "a52a2a");
        _colors.Add("beige", "f5f5dc");
        _colors.Add("tan", "d4b48c");
        _colors.Add("lime", "00ff00");
        _colors.Add("olive", "808000");
        _colors.Add("turquoise", "40e0d0");
        _colors.Add("teal", "008080");
        _colors.Add("indigo", "4b0082");
        _colors.Add("violet", "ee82ee");
    }

    /// <summary>
    /// Returns a collection of all available names in the built-in dictionary.
    /// </summary>
    public static Collection<string> AvailableColorNames()
    {
        return [.. _colors.Keys];
    }

    /// <summary>
    /// Returns a hex color associated with the provided name in the built-in collection, if exists. Use this method
    /// if you don't care about overriding the default value if the specified color does not exist.
    /// </summary>
    /// <remarks>
    /// This method is guaranteed to return a color code even if it doesn't exist in the collection. For a condition
    /// guarded method use <c>TryGetColor</c>.
    /// </remarks>
    public static string HexFromName(string colorName)
    {
        if (!_colors.TryGetValue(colorName, out string? hexColor))
        {
            hexColor ??= "fff";
        }

        return hexColor;
    }

    /// <summary>
    /// Gets the color code associated with the specified color name. This helps with assigning a new value if the
    /// specified color does not exist or to skip the default assignment.
    /// </summary>
    /// <param name="colorName">Color name to check on the dictionary.</param>
    /// <param name="color">Nullable reference to eventual color.</param>
    /// <returns>
    /// True if the colors dictionary contains the requested specified color.
    /// </returns>
    public static bool TryGetColor(string colorName, [MaybeNullWhen(false)] out string color)
    {
        bool condition = _colors.ContainsKey(colorName);

        color = condition ? HexFromName(colorName) : null;

        return condition;
    }
}
