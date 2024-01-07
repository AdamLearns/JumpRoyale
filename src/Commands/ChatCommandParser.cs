using System.Collections.Generic;
using System.Linq;

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
        return PadList(_arguments, null).ToArray();
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

        return PadList(arguments, null).ToArray();
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
    /// Pad the arguments list with a filler (preferably a nullable), to always have nullable arguments
    /// </summary>
    /// <param name="list"></param>
    /// <param name="padValue">"Filler" to add if the arguments count is below set Maximum</param>
    private static List<T> PadList<T>(List<T> list, T padValue)
    {
        if (list.Count < MaxArguments)
        {
            list.AddRange(Enumerable.Repeat(padValue, MaxArguments));
        }

        return list;
    }

    private static List<string> ParseChatMessage(string chatMessage)
    {
        // TODO: find a way to capture mixed values, like Hex (e.g. glow BDFF00)
        List<string> result = new();
        string tmpWord = "";
        bool wasDigit = false;
        bool wasLetter = false;

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
            /// Just a failsafe in case nothing was caught and to have reference for Name
            result.Add("");
        }

        return result;
    }
}
