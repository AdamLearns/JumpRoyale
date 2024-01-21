using System.Text.RegularExpressions;

namespace Utils;

[TestFixture]
public class ColorProviderTests
{
    /// <summary>
    /// This test walks through the entire dictionary and makes sure that defined color codes are valid hex colors.
    /// </summary>
    [Test]
    public void ContainsValidColorCodes()
    {
        Regex pattern = new("^(?:[0-9a-fA-F]{3}){1,2}$");

        foreach (string colorName in ColorProvider.AvailableColorNames())
        {
            string color = ColorProvider.HexFromName(colorName);

            Assert.That(pattern.IsMatch(color), $"Invalid color code in the built-in dictionary: {color}");
        }
    }

    [Test]
    public void CanRetrieveColorByName()
    {
        string color = ColorProvider.HexFromName("red");

        Assert.That(color, Is.EqualTo("ff0000"));
    }

    [Test]
    public void CanReturnDefaultColor()
    {
        string color = ColorProvider.HexFromName("this color does not exist");

        Assert.That(color, Is.EqualTo("fff"));
    }

    /// <summary>
    /// This test makes sure that we can properly use the guarded color request, covering both cases where the expected
    /// value is not overwritten or omitted by a failed check (falling into the <c>true</c> block).
    /// </summary>
    [Test]
    public void TestsGuardedCall()
    {
        // If we try to get a color that exists, the if statement does not evaluate to true and the declared color
        // should return the actual color code and not null
        if (!ColorProvider.TryGetColor("red", out string? color))
        {
            color = "bad";
        }

        Assert.That(color, Is.EqualTo("ff0000"));

        // If we try to get a color that does not exist, the if statement will evaluate to true and override the
        // declaration, which assigns the "bad" value
        if (!ColorProvider.TryGetColor("rrrrrred", out string? newColor))
        {
            newColor = "bad";
        }

        Assert.That(newColor, Is.EqualTo("bad"));
    }
}
