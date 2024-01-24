using System;

namespace TwitchChat
{
    public class MessageEventArgs : EventArgs
    {
        public string Message { get; set; } = string.Empty;

        public string SenderName { get; set; } = string.Empty;

        public string SenderId { get; set; } = string.Empty;

        public string HexColor { get; set; } = string.Empty;

        public bool IsPrivileged { get; set; }
    }
}
