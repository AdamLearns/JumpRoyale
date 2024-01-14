using System;
using Microsoft.Extensions.Configuration;
using TwitchLib.Api.Core.HttpCallHandlers;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

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

    public class TwitchChatClient
    {
        /// <summary>
        /// Describes <c>MessagesAllowedInPeriod</c> argument for ClientOptions.
        /// </summary>
        private const int MaximumMessages = 750;

        /// <summary>
        /// Describes <c>ThrottlingPeriod</c> TimeSpan in seconds for ClientOptions.
        /// </summary>
        private const int Seconds = 30;

        private readonly TwitchPubSub _tps;
        private readonly TwitchClient _client;
#pragma warning disable // Reserved until Twitch API Requests are implemented
        private readonly TwitchHttpClient _httpClient;
#pragma warning restore

        private string _channelId = string.Empty;

        public TwitchChatClient()
        {
            _tps = CreateTwitchPubSub();
            _client = CreateTwitchClient();
            _httpClient = CreateHttpClient();

            ConnectToTwitch();
        }

        public event EventHandler<MessageEventArgs>? OnMessage;

        public event EventHandler<OnRewardRedeemedArgs>? OnRedemption;

        private void ConnectToTwitch()
        {
            _tps.Connect();
            _client.Connect();
        }

        private TwitchPubSub CreateTwitchPubSub()
        {
            TwitchPubSub tps = new();

            tps.OnPubSubServiceConnected += (object sender, EventArgs e) =>
            {
                Console.WriteLine("PubSub connected");
#pragma warning disable CS0618 // Type or member is obsolete
                tps.ListenToRewards("47098493");
#pragma warning restore CS0618 // Type or member is obsolete
                tps.SendTopics();
#pragma warning disable CS0618,CS8602// Type or member is obsolete
                tps.OnRewardRedeemed += (object sender, OnRewardRedeemedArgs e) =>
                {
                    Console.WriteLine($"Reward redeemed: {e}");
                    OnRedemption.Invoke(this, e);
                };
#pragma warning restore CS0618 // Type or member is obsolete
            };

            return tps;
        }

        private TwitchClient CreateTwitchClient()
        {
            (ConnectionCredentials credentials, ClientOptions options) = ConfigureClient();
            WebSocketClient customClient = new(options);
            TwitchClient client = new(customClient);

            client.Initialize(credentials, _channelId);

            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnConnected += Client_OnConnected;

            return client;
        }

        private TwitchHttpClient CreateHttpClient()
        {
            TwitchHttpClient httpClient = new();

            return httpClient;
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine("Successfully connected to Twitch");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Successfully connected to the channel");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            bool isPrivileged = e.ChatMessage.IsSubscriber || e.ChatMessage.IsModerator || e.ChatMessage.IsVip;

            HandleChatMessage(
                e.ChatMessage.UserId,
                e.ChatMessage.DisplayName,
                e.ChatMessage.Message,
                e.ChatMessage.ColorHex,
                isPrivileged
            );
        }

        private Tuple<ConnectionCredentials, ClientOptions> ConfigureClient()
        {
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<TwitchChatClient>().Build();
            string accessToken =
                config["twitch_access_token"]
                ?? throw new Exception(
                    "No access token found. Please run `dotnet user-secrets set twitch_access_token <your access token>`"
                );
            _channelId =
                config["twitch_channel_name"]
                ?? throw new Exception(
                    "Channel not found. Please run `dotnet user-secrets set twitch_channel_name <your twitch channel>`"
                );
            ConnectionCredentials credentials = new(_channelId, accessToken);
            ClientOptions clientOptions =
                new() { MessagesAllowedInPeriod = MaximumMessages, ThrottlingPeriod = TimeSpan.FromSeconds(Seconds), };

            return new(credentials, clientOptions);
        }

        private void HandleChatMessage(
            string senderId,
            string senderName,
            string message,
            string colorHex,
            bool isPrivileged
        )
        {
            OnMessage.Invoke(
                this,
                new MessageEventArgs
                {
                    Message = message,
                    SenderName = senderName,
                    SenderId = senderId,
                    HexColor = colorHex ?? "#ffffff",
                    IsPrivileged = isPrivileged,
                }
            );
        }
    }
}
