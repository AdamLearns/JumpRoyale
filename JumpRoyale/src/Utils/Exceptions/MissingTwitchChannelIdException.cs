using System;

namespace TwitchChat;

public class MissingTwitchChannelIdException : Exception
{
    public MissingTwitchChannelIdException()
        : base(TwitchConstants.MissingChannelIdError) { }

    public MissingTwitchChannelIdException(string message)
        : base(message) { }

    public MissingTwitchChannelIdException(string message, Exception innerException)
        : base(message, innerException) { }
}
