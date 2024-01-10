using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

internal class ChatCommandParser
{
    private const int MaxArguments = 2;

    public string Name { get; private set; }

    private readonly List<string> _arguments = new();

    internal ChatCommandParser(string chatMessage)
    {
        _arguments = ParseChatMessage(chatMessage);

        Name = _arguments[0];

        // We don't need the name in the arguments anymore
        _arguments.RemoveAt(0);
    }

    public string[] ArgumentsAsStrings()
    {
        return PadListWithNullables(_arguments, null).ToArray();
    }

    /// <summary>
    /// Tries to parse detected arguments to numbers (or nulls) and returns them in a padded
    /// array. The padding values are nulls for easier defaulting
    /// </summary>
    public int?[] ArgumentsAsNumbers()
    {
        List<int?> arguments = (
            from argument in _arguments
            let parsedArgument = ParseToNumberOrNull(argument)
            select parsedArgument
        ).ToList();

        return PadListWithNullables(arguments, null).ToArray();
    }

    private static int? ParseToNumberOrNull(string test)
    {
        if (!int.TryParse(test, out int parsedNumber))
        {
            return null;
        }

        return parsedNumber;
    }

    /// <summary>
    /// Pad the arguments so the commands can imply their own default value in case a null was
    /// present at requested position
    /// </summary>
    /// <param name="list"></param>
    /// <param name="padValue">"Filler" to add if the arguments count is below set Maximum</param>
    private static List<T> PadListWithNullables<T>(List<T> list, T padValue)
    {
        if (list.Count < MaxArguments)
        {
            list.AddRange(Enumerable.Repeat(padValue, MaxArguments));
        }

        return list;
    }

    private static List<string> ParseChatMessage(string chatMessage)
    {
        List<string> result = new();

        // TODO: find a way to capture mixed values, like Hex (e.g. glow BDFF00)
        // Unfortunately, for now, since the only command that uses mixed values is glow, we have
        // to hardcode the glow command handler to retrieve Hex value, until coded properly :)
        // This could later be replaced to match other commands that use Hex arguments
        if (chatMessage.StartsWith("glow"))
        {
            return HandleHexArguments(chatMessage);
        }

        string tmpWord = "";
        bool wasDigit = false;
        bool wasLetter = false;

        // Commit from @smu4242 - this alternates between consecutive letters (as words)/digits
        void HandleNextLetter(char? letter, bool condition, bool ifWasDigit, bool ifWasLetter)
        {
            if (condition)
            {
                result.Add(tmpWord);

                tmpWord = "";
            }

            tmpWord += letter;
            wasDigit = ifWasDigit;
            wasLetter = ifWasLetter;
        }

        foreach (char c in chatMessage)
        {
            if (char.IsLetter(c))
            {
                HandleNextLetter(c, wasDigit, false, true);
            }
            else if (char.IsDigit(c) || c == '-')
            {
                HandleNextLetter(c, wasLetter, true, false);
            }
            else
            {
                HandleNextLetter(null, wasLetter || wasDigit, false, false);
            }
        }

        if (!string.IsNullOrEmpty(tmpWord))
        {
            result.Add(tmpWord);
        }

        if (!result.Any())
        {
            // Just a failsafe in case nothing was caught and to have reference for Name
            result.Add("");
        }

        return result;
    }

    private static List<string> HandleHexArguments(string chatMessage)
    {
        List<string> arguments = new() { "glow" };

        string[] split = chatMessage.Split(
            "glow",
            System.StringSplitOptions.TrimEntries | System.StringSplitOptions.RemoveEmptyEntries
        );

        // In case there was nothing, just return to be able to default to Twitch Color
        if (split.Length == 0)
        {
            return arguments;
        }

        // Match Both 3 and 6 length
        Regex pattern = new("^(?:[0-9a-fA-F]{3}){1,2}$");

        // Note: no Else for defaults, this defaults to Twitch color later anyway
        if (pattern.IsMatch(split[0]))
        {
            arguments.Add(split[0]);
        }

        return arguments;
    }
}
