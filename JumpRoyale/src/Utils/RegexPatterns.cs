using System.Text.RegularExpressions;

/// <summary>
/// Exposes a predefined set of Regex patterns. Ideally, all patterns should be generated and exposed from here.
/// <para>
/// Please refer to: <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-source-generators">regex source generators</a>.
/// </para>
/// </summary>
public static partial class RegexPatterns
{
    /// <summary>
    /// Gets the Hex color matching pattern (3 and 6 characters long). This pattern is used to perform an exact match
    /// on the entire string, e.g. "FFFF" or "A0A0A0A" will not match.
    /// </summary>
    public static Regex HexColor
    {
        get { return HexColorPattern(); }
    }

    [GeneratedRegex("^(?:[0-9a-fA-F]{3}){1,2}$")]
    private static partial Regex HexColorPattern();
}
