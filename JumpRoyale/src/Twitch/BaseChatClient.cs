using System;
using Microsoft.Extensions.Configuration;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.PubSub;

namespace TwitchChat
{
    public class BaseChatClient
    {
        /// <summary>
        /// Describes <c>MessagesAllowedInPeriod</c> argument for ClientOptions.
        /// </summary>
        private const int _maximumMessages = 750;

        /// <summary>
        /// Describes <c>ThrottlingPeriod</c> TimeSpan in seconds for ClientOptions.
        /// </summary>
        private const int _throttlingInSeconds = 30;

        private readonly string _accessToken;

        protected BaseChatClient(string channelId)
        {
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<TwitchChatClient>().Build();

            _accessToken = config["twitch_access_token"] ?? throw new MissingTwitchAccessTokenException();
            ChannelName = config["twitch_channel_name"] ?? throw new MissingTwitchChannelNameException();

            ChannelId = channelId;
            TwitchPubSub = new();
            TwitchClient = CreateTwitchClient();

            ConnectToTwitch();
        }

        protected string ChannelId { get; private set; }

        protected string ChannelName { get; private set; }

        protected TwitchPubSub TwitchPubSub { get; private set; }

        protected TwitchClient TwitchClient { get; private set; }

        private TwitchClient CreateTwitchClient()
        {
            (ConnectionCredentials credentials, ClientOptions options) = ConfigureClient();
            WebSocketClient customClient = new(options);
            TwitchClient client = new(customClient);

            client.Initialize(credentials, ChannelName);

            return client;
        }

        private Tuple<ConnectionCredentials, ClientOptions> ConfigureClient()
        {
            ConnectionCredentials credentials = new(ChannelName, _accessToken);
            ClientOptions clientOptions =
                new()
                {
                    MessagesAllowedInPeriod = _maximumMessages,
                    ThrottlingPeriod = TimeSpan.FromSeconds(_throttlingInSeconds),
                };

            return new(credentials, clientOptions);
        }

        private void ConnectToTwitch()
        {
            TwitchPubSub.Connect();
            TwitchClient.Connect();
        }
    }
}
