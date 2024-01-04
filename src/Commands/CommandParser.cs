using System;

public abstract class BaseChatCommand
{
    protected readonly string[] arguments;

    public string Name { get; private set; }

    public BaseChatCommand(string chatMessage)
    {
        arguments = chatMessage.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        Name = arguments[0].ToLower();
    }

    public abstract bool IsValid();
}
