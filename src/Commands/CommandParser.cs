using System;
using System.Linq;

public abstract class CommandParser
{
    protected readonly string[] _arguments;

    private readonly string _name;

    public CommandParser(string chatMessage)
    {
        _arguments = chatMessage.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        /// TODO: test if it's actually possible to receive an empty message from Twitch, because
        /// previously this had a check for <1 Arguments Length, but Twitch blocks empty
        /// message requests, although there could be extensions preventing that

        _name = _arguments[0].ToLower();

        // Check the last part. If it's not a number, then just reject it entirely.
        // This way, people can get around the duplicate-message problem on Twitch.
        if (!int.TryParse(_arguments[^1], out _))
        {
            _arguments = _arguments.Take(_arguments.Length - 1).ToArray();
        }
    }

    public abstract bool IsValid();

    public string GetCommandName()
    {
        return _name;
    }
}
