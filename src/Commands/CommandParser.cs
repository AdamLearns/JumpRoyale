using System;

/// <summary>
/// Base class for chat commands that provide command name and argument storage. Commands
/// have to parse the arguments in their own classes due to different argument formats
/// they can operate on: strings, special string formats, number parse, no args
/// </summary>
public abstract class BaseChatCommand
{
    protected readonly string[] arguments;

    /// Command name extracted from the detected chat message. This is always the first part of the
    /// chat message, so this can either be an exact command name match or an alias, depending
    /// how strict the message event handler is when parsing the detected chat message
    public string Name { get; private set; }

    public BaseChatCommand(string chatMessage)
    {
        arguments = chatMessage.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        Name = arguments[0].ToLower();
    }

    /// <summary>
    /// Command validation implemented by derived classes. Depending on the command implementation,
    /// it's either an allowed command alias, argument presence or formatting
    /// </summary>
    public abstract bool IsValid();
}
