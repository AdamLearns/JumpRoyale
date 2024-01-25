using System;

namespace TwitchChat
{
    public class MissingTwitchAccessTokenException : Exception
    {
        public MissingTwitchAccessTokenException()
            : base(TwitchClientMessages.MissingAccessTokenError) { }

        public MissingTwitchAccessTokenException(string message)
            : base(message) { }

        public MissingTwitchAccessTokenException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
