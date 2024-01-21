using System.Collections.Immutable;

public static class ColorProvider
{
    static ColorProvider()
    {
        AvailableColors = ImmutableDictionary<string, string>
            .Empty.Add("red", "ff0000")
            .Add("orange", "ffa500")
            .Add("yellow", "ffff00")
            .Add("green", "008000")
            .Add("cyan", "00ffff")
            .Add("blue", "0000ff")
            .Add("magenta", "ff00ff")
            .Add("purple", "800080")
            .Add("white", "ffffff")
            .Add("black", "000000")
            .Add("grey", "808080")
            .Add("gray", "808080")
            .Add("silver", "c0c0c0")
            .Add("pink", "ffc0cb")
            .Add("maroon", "800000")
            .Add("brown", "a52a2a")
            .Add("beige", "f5f5dc")
            .Add("tan", "d4b48c")
            .Add("lime", "00ff00")
            .Add("olive", "808000")
            .Add("turquoise", "40e0d0")
            .Add("teal", "008080")
            .Add("indigo", "4b0082")
            .Add("violet", "ee82ee");
    }

    public static ImmutableDictionary<string, string> AvailableColors { get; private set; }

    /// <summary>
    /// Returns a hex color associated with the provided name in the built-in collection, if exists. Use this method
    /// if you don't care about overriding the default value if the specified color does not exist.
    /// </summary>
    /// <remarks>
    /// This method is guaranteed to return a color code even if it doesn't exist in the collection to omit
    /// unnecessary null checks every time. For a nullable return type, use the boolean overload.
    /// </remarks>
    /// <param name="colorName">Color name for dictionary lookup.</param>
    public static string HexFromName(string colorName)
    {
        if (!AvailableColors.TryGetValue(colorName, out string? hexColor))
        {
            hexColor ??= "fff";
        }

        return hexColor;
    }

    /// <summary>
    /// Returns a hex color associated with the provided name in the built-in collection, will return default value
    /// if the specified color does not exist or <c>null</c> if <c>nullIfNotExists</c> was set to <c>true</c>.
    /// </summary>
    /// <remarks>
    /// This is a helper overload that allows performing a custom action if the color does not exist in the
    /// collection, e.g. "if no color was returned, call some arbitrary method".
    /// </remarks>
    /// <param name="colorName">Color name for dictionary lookup.</param>
    /// <param name="nullIfNotExists">If true, the default return value is returned as <c>null</c>.</param>
    public static string? HexFromName(string colorName, bool nullIfNotExists)
    {
        if (!AvailableColors.TryGetValue(colorName, out string? hexColor))
        {
            hexColor ??= nullIfNotExists ? null : "fff";
        }

        return hexColor;
    }
}
