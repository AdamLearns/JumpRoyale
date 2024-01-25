using System;

namespace TwitchChat
{
    public class MissingTwitchChannelNameException : Exception
    {
        public MissingTwitchChannelNameException()
            : base(TwitchClientMessages.MissingChannelNameError) { }

        public MissingTwitchChannelNameException(string message)
            : base(message) { }

        public MissingTwitchChannelNameException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
