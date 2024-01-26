using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class ChatCommandParser
{
    /// <summary>
    /// Maximum amount of arguments returned by the parser. Increase, if necessary.
    /// </summary>
    private const int MaxArguments = 2;

    private readonly List<string?> _arguments;

    public ChatCommandParser(string chatMessage)
    {
        chatMessage ??= string.Empty;

        _arguments = ParseChatMessage(chatMessage);

        if (_arguments.Count > 0)
        {
            Name = _arguments[0] ?? string.Empty;

            // We don't need the name in the arguments anymore
            _arguments.RemoveAt(0);
        }
    }

    public string Name { get; private set; } = string.Empty;

    public string?[] ArgumentsAsStrings()
    {
        return [.. PadList(_arguments, null)];
    }

    /// <summary>
    /// Tries to parse detected arguments to numbers (or nulls) and returns them in a padded
    /// array. The padding values are nulls for easier defaulting.
    /// </summary>
    public int?[] ArgumentsAsNumbers()
    {
        List<int?> arguments = (
            from argument in _arguments
            let parsedArgument = ParseToNumberOrNull(argument)
            select parsedArgument
        ).ToList();

        return [.. PadList(arguments, null)];
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
    /// present at requested position.
    /// </summary>
    /// <param name="list">List to pad.</param>
    /// <param name="padValue">"Filler" to add if the arguments count is below set Maximum.</param>
    private static List<T> PadList<T>(List<T> list, T padValue)
    {
        if (list.Count < MaxArguments)
        {
            list.AddRange(Enumerable.Repeat(padValue, MaxArguments));
        }

        return list;
    }

    private static List<string?> ParseChatMessage(string chatMessage)
    {
        List<string?> result = [];

        // Just hardcode the list of commands that could be using hex colors as arguments until someone finds a
        // smarter way of doing this :)
        string forcedMatch = chatMessage switch
        {
            string when chatMessage.StartsWith("glow") => "glow",
            _ => string.Empty
        };

        if (!string.IsNullOrEmpty(forcedMatch))
        {
            return HandleHexArguments(chatMessage, forcedMatch);
        }

        string tmpWord = string.Empty;
        bool wasDigit = false;
        bool wasLetter = false;

        // Commit from @smu4242 - this alternates between consecutive letters (as words)/digits
        void HandleNextLetter(char? letter, bool condition, bool ifWasDigit, bool ifWasLetter)
        {
            if (condition)
            {
                result.Add(tmpWord);

                tmpWord = string.Empty;
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

        if (result.Count == 0)
        {
            // Just a failsafe in case nothing was caught and to have reference for Name
            result.Add(string.Empty);
        }

        return result;
    }

    private static List<string?> HandleHexArguments(string chatMessage, string splitBy)
    {
        // Force the supposed command name to be on the arguments list so the constructor can extract it
        List<string?> arguments = [splitBy];

        string[] split = chatMessage.Split(
            splitBy,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );

        // In case there was nothing, just return to be able to default to Twitch Color
        if (split.Length == 0)
        {
            return arguments;
        }

        // If there is a split result, it can be a potential hex argument, but just in case catch more than just the
        // first argument. This will be useful if multiple hex arguments are required for some commands, e.g.
        // simple gradients or multi-colored clothes
        string[] parameters = split[0].Split(" ");

        // Match Both 3 and 6 length
        Regex pattern = new("^(?:[0-9a-fA-F]{3}){1,2}$");

        for (int i = 0; i < Math.Min(parameters.Length, MaxArguments); i++)
        {
            string input = parameters[i];

            // To match the hex color, inputs have to literally be 3 or 6 characters and a valid hex, so sending
            // something like "ffff" is not valid. Input has to be specific
            arguments.Add(pattern.IsMatch(input) ? input : null);
        }

        return arguments;
    }
}
