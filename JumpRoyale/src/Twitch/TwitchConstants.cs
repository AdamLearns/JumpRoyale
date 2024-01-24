namespace TwitchChat
{
    public static class TwitchConstants
    {
        /// <summary>
        /// Describes <c>MessagesAllowedInPeriod</c> argument for ClientOptions.
        /// </summary>
        public const int MaximumMessages = 750;

        /// <summary>
        /// Describes <c>ThrottlingPeriod</c> TimeSpan in seconds for ClientOptions.
        /// </summary>
        public const int ThrottlingInSeconds = 30;

        public const string OnPubSubConnected = "Successfully connected to PubSub";
        public const string OnClientConnectedMessage = "Successfully connected to Twitch";
        public const string OnChannelJoinMessage = "Successfully connected to the channel";
        public const string MissingAccessTokenError = "No access token found. Please run `dotnet user-secrets set twitch_access_token <your access token>`";
        public const string MissingChannelNameError = "Channel not found. Please run `dotnet user-secrets set twitch_channel_name <your twitch channel>`";
    }
}
