using System;
using System.Linq;

public abstract class BaseChatCommand
{
    protected readonly string[] _arguments;

    public string Name { get; private set; }

    public BaseChatCommand(string chatMessage)
    {
        _arguments = chatMessage.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        Name = _arguments[0].ToLower();
    }

    public abstract bool IsValid();
}
