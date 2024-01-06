using System.Collections.Generic;
using System.Linq;

internal class CommandParser
{
    public string Name { get; private set; }

    private readonly List<string> _arguments = new();

    internal CommandParser(string chatMessage)
    {
        _arguments = ParseChatMessage(chatMessage);

        Name = _arguments[0];

        // We don't need the name in the arguments anymore, that way we have the arguments
        // stored separately from the name
        _arguments.RemoveAt(0);
    }

    public string[] ArgumentsAsStrings()
    {
        return _arguments.ToArray();
    }

    public int?[] ArgumentsAsNumbers()
    {
        List<int?> arguments = new();

        _arguments
            .ToList()
            .ForEach(argument =>
            {
                int? parsedArgument = ParseToNumber(argument);

                if (parsedArgument is null)
                {
                    return;
                }

                arguments.Add(parsedArgument);
            });

        return arguments.ToArray();
    }

    /// <summary>
    /// Tries to convert the given string into a number
    /// </summary>
    /// <returns>`null` when the conversion failed, number on success</returns>
    private static int? ParseToNumber(string test)
    {
        if (!int.TryParse(test, out int parsedNumber))
        {
            return null;
        }

        return parsedNumber;
    }

    private static List<string> ParseChatMessage(string chatMessage)
    {
        List<string> result = new();
        string tmpWord = "";
        bool wasLetter = false;
        bool wasDigit = false;

        foreach (char c in chatMessage)
        {
            if (char.IsLetter(c))
            {
                if (wasDigit)
                {
                    result.Add(tmpWord);
                    tmpWord = "";
                }

                tmpWord += c;
                wasLetter = true;
                wasDigit = false;
            }
            else if (char.IsDigit(c) || c == '-')
            {
                if (wasLetter)
                {
                    result.Add(tmpWord);
                    tmpWord = "";
                }

                tmpWord += c;
                wasDigit = true;
                wasLetter = false;
            }
            else
            {
                if (wasLetter || wasDigit)
                {
                    result.Add(tmpWord);
                    tmpWord = "";
                }

                wasDigit = false;
                wasLetter = false;
            }
        }

        if (!string.IsNullOrEmpty(tmpWord))
        {
            result.Add(tmpWord);
        }

        return result;
    }
}
