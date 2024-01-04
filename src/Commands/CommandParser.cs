using System;
using System.Linq;

public abstract class CommandParser
{
    protected readonly string[] _arguments;

    private readonly string _name;

    public CommandParser(string chatMessage)
    {
        _arguments = chatMessage.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        _name = _arguments[0].ToLower();
    }

    public abstract bool IsValid();

    public string GetCommandName()
    {
        return _name;
    }
}
