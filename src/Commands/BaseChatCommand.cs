using System;
using System.Collections.Generic;

/// <summary>
/// Base class for chat commands that provide command name and argument storage. Commands
/// have to parse the arguments in their own classes due to different argument formats
/// they can operate on: strings, special string formats, number parsing, no args
/// </summary>
public abstract class BaseChatCommand
{
    protected readonly string[] arguments;

    /// <summary>
    /// Command name extracted from the detected chat message. This is always the first part of the
    /// chat message, so this can either be an exact command name match or an alias, depending on
    /// how strict the message event handler is when parsing the chat message
    /// </summary>
    public string Name { get; private set; }

    public BaseChatCommand(string chatMessage)
    {
        arguments = splitArguments(chatMessage);

        Name = arguments[0].ToLower();
    }

    // split arguments, e.g. make "r-12" into ["r", "-12"]
    public static string[] splitArguments(string chatMessage)
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
        return result.ToArray();
    }

    /// <summary>
    /// Command validation implemented by derived classes. Depending on the
    /// command implementation, it could be checking for an allowed command
    /// alias, the presence of particular arguments, or formatting
    /// </summary>
    public abstract bool IsValid();
}
