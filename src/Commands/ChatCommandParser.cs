using System.Collections.Generic;
using System.Linq;

internal class ChatCommandParser
{
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
        return _arguments.ToArray();
    }

    public int?[] ArgumentsAsNumbers()
    {
        List<int?> arguments = new();

        _arguments
            .ToList()
            .ForEach(argument =>
            {
                int? parsedArgument = ParseToNumberOrNull(argument);

                if (parsedArgument is null)
                {
                    return;
                }

                arguments.Add(parsedArgument);
            });

        return arguments.ToArray();
    }

    private static int? ParseToNumberOrNull(string test)
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
