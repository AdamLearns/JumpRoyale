using System.Collections.Immutable;

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
        foreach (string colorName in ColorProvider.AvailableColors.Keys)
        {
            string color = ColorProvider.HexFromName(colorName);

            Assert.That(
                RegexPatterns.HexColor.IsMatch(color),
                $"Invalid color code in the built-in dictionary: {color}"
            );
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

    [Test]
    public void CanExecuteCustomActionIfColorNotExists()
    {
        string expected = "ABCDEF";
        string? test = null;
        string? color = ColorProvider.HexFromName("asdf", true);

        // Just assume there is a method somewhere in the application that does something. This could be anything,
        // e.g. overriding class members, populating lists, anything
        void ShouldOverrideExpected()
        {
            test = expected;
        }

        // We don't care about the color here, we only care about the action
        if (color is null)
        {
            ShouldOverrideExpected();
        }

        Assert.That(test, Is.EqualTo(expected));
    }

    /// <summary>
    /// This test makes sure that we will only add colors to a new dictionary, keeping the original untouched. Exposed
    /// generic collections should not be modified externally.
    /// </summary>
    [Test]
    public void CantModifyBuiltinColors()
    {
        ImmutableDictionary<string, string> copy = ColorProvider.AvailableColors.Add("something", "new");

        Assert.Multiple(() =>
        {
            Assert.That(ColorProvider.AvailableColors, Does.Not.ContainKey("something"));
            Assert.That(copy, Contains.Key("something"));
        });
    }
}
