using System.Text.RegularExpressions;

namespace Commands;

[TestFixture]
public class ChatCommandParserTests
{
    /// <summary>
    /// This test makes sure that whatever we put in the chat message, will become a potential command.
    /// </summary>
    [Test]
    public void CanExtractCommandName()
    {
        List<string> chatInput = ["j", "l", "u", "r", "l3", "l30 30", "r 30", "u 30 30", "rrrr7890"];

        foreach (string input in chatInput)
        {
            ChatCommandParser command = new(input);

            Assert.That(input, Does.StartWith(command.Name));
        }
    }

    /// <summary>
    /// This will most likely not happen on Twitch side, but in case the parser received a null or
    /// an empty message, it should give an empty string back.
    /// </summary>
    [Test]
    public void CanReturnNothingWhenEmpty()
    {
        List<string?> chatInput = [null, string.Empty];

        foreach (string? input in chatInput)
        {
            // We don't care that it's possibly null here
#pragma warning disable CS8604 // Possible null reference argument.
            ChatCommandParser command = new(input);
#pragma warning restore CS8604 // Possible null reference argument.

            Assert.That(command.Name, Is.EqualTo(string.Empty));
        }
    }

    /// <summary>
    /// Not all messages are "valid", game commands ideally should be [a-zA-Z0-9-], so
    /// as a fallback, there should be an empty string in return and sending just
    /// a number is still a valid message interpreted as a command.
    /// </summary>
    [Test]
    public void CanHandleGarbageMessage()
    {
        List<string> chatInput = [string.Empty, " ", ";", "!", "[]", "[#@}=]"];

        foreach (string input in chatInput)
        {
            ChatCommandParser command = new(input);

            Assert.That(command.Name, Is.EqualTo(string.Empty));
        }
    }

    /// <summary>
    /// This test ensures that both command formats work, where the initial space is omitted
    /// and the argument is extracted properly (spaced and non-spaced commands).
    /// </summary>
    [Test]
    public void CanExtractStringArguments()
    {
        List<string> commandInputs = ["l30 30", "l 30 30"];

        foreach (string input in commandInputs)
        {
            ChatCommandParser command = new(input);

            string?[] arguments = command.ArgumentsAsStrings();

            foreach (string? argument in arguments)
            {
                Assert.That(argument, Is.EqualTo("30"));
            }
        }
    }

    [Test]
    public void CanExtractNumericArguments()
    {
        List<string> commandInputs = ["l30 30", "l 30 30"];

        foreach (string input in commandInputs)
        {
            ChatCommandParser command = new(input);

            int?[] arguments = command.ArgumentsAsNumbers();

            foreach (int? argument in arguments)
            {
                Assert.That(argument, Is.EqualTo(30));
            }
        }
    }

    /// <summary>
    /// This test ensures that when we send just a matching command, we won't get any
    /// extra arguments on the arguments list. This is important, because we can
    /// later use null checks to default to something.
    /// </summary>
    [Test]
    public void CanPadStringArgumentsWithNulls()
    {
        ChatCommandParser command = new("l");

        string?[] arguments = command.ArgumentsAsStrings();

        Assert.Multiple(() =>
        {
            Assert.That(arguments[0], Is.Null);
            Assert.That(arguments[1], Is.Null);
        });
    }

    [Test]
    public void CanPadNumericArgumentsWithNulls()
    {
        ChatCommandParser command = new("l");

        int?[] arguments = command.ArgumentsAsNumbers();

        Assert.Multiple(() =>
        {
            Assert.That(arguments[0], Is.Null);
            Assert.That(arguments[1], Is.Null);
        });
    }

    /// <summary>
    /// There are commands, which take mixed type of input, apart from numbers or simple
    /// strings (just letters). For example, `glow` command can change the particle
    /// color and it takes Hex input from the user, which is a mixed type.
    /// </summary>
    [Test]
    public void CanExtractArgumentOfMixedType()
    {
        List<string> expectedOutputs = ["09B0D9", "BDFF00", "123456", "f02", "f02bc6"];

        foreach (string output in expectedOutputs)
        {
            // Just test for both non-space and space
            List<string> commandInputs = [$"glow {output}", $"glow{output}"];

            foreach (string input in commandInputs)
            {
                ChatCommandParser command = new(input);

                string?[] arguments = command.ArgumentsAsStrings();

                Assert.That(arguments[0], Is.EqualTo(output));
            }
        }
    }

    [Test]
    public void CanRejectInvalidGlowColors()
    {
        List<string> invalidColors = ["12342", "90 8da", "ffdd1", "v90909", "8888888", "l", "2"];

        foreach (string color in invalidColors)
        {
            List<string> commandInputs = [$"glow {color}", $"glow{color}"];

            foreach (string input in commandInputs)
            {
                ChatCommandParser command = new(input);

                string?[] arguments = command.ArgumentsAsStrings();

                Assert.That(arguments[0], Is.Null);
            }
        }
    }

    /// <summary>
    /// This test makes sure that when we try to execute a multi-color command and any of the arguments is invalid,
    /// it will be passed as null. The reason for this is to be able to set default colors in case the provided
    /// arguments are invalid.
    /// </summary>
    [Test]
    public void CanConvertInvalidColorToNull()
    {
        List<string> colorInputs = ["zxcv f0c", "f0c vcx"];

        foreach (string color in colorInputs)
        {
            List<string> commandInputs = [$"glow {color}", $"glow{color}"];

            foreach (string input in commandInputs)
            {
                ChatCommandParser command = new(input);

                string?[] arguments = command.ArgumentsAsStrings();

                Assert.That(arguments.ToList().Exists(argument => argument is null));
            }
        }
    }

    /// <summary>
    /// This test assumes all arguments are valid colors.
    /// </summary>
    [Test]
    public void CanExtractMultipleColors()
    {
        List<string> colorInputs = ["f0c f0c", "f0c173 434343", "919292 f0c", "f0c Ac0eAc"];

        foreach (string color in colorInputs)
        {
            List<string> commandInputs = [$"glow {color}", $"glow{color}"];

            foreach (string input in commandInputs)
            {
                ChatCommandParser command = new(input);

                string?[] arguments = command.ArgumentsAsStrings();

                Assert.That(arguments.ToList().TrueForAll(argument => argument is not null));
            }
        }
    }

    /// <summary>
    /// This test makes sure that when we try to execute commands like "glow" or any other command specified by in the
    /// CommandParser, we get a valid code name in return if there was a literal name provided by the user.
    /// </summary>
    [Test]
    public void CanDetectColorNames()
    {
        List<string> colorInputs = ["red red", "blue f0c", "f0c green"];

        foreach (string colors in colorInputs)
        {
            // Test for space and non-space arguments
            List<string> commandInputs = [$"glow {colors}", $"glow{colors}"];

            foreach (string input in commandInputs)
            {
                ChatCommandParser command = new(input);

                string?[] arguments = command.ArgumentsAsStrings();

                // If this evaluates to null, it means the command parser could not find the color name. The test color
                // inside the ColorProvider was either removed or renamed (or if the CommandParser was modified),
                // which caused the color to be returned as null, failing the name detection.
                Assert.That(arguments.ToList().TrueForAll(argument => argument is not null), $"Command: {input}");
            }
        }
    }
}
