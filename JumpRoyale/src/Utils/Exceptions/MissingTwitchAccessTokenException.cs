using System;

namespace TwitchChat;

public class MissingTwitchAccessTokenException : Exception
{
    public MissingTwitchAccessTokenException()
        : base(TwitchConstants.MissingAccessTokenError) { }

    public MissingTwitchAccessTokenException(string message)
        : base(message) { }

    public MissingTwitchAccessTokenException(string message, Exception innerException)
        : base(message, innerException) { }
}
