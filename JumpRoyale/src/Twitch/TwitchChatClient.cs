using System;
using Microsoft.Extensions.Configuration;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace TwitchChat
{
    public class TwitchChatClient
    {
        private readonly TwitchPubSub _tps;
        private readonly TwitchClient _client;

        private string _channelId = string.Empty;

        public TwitchChatClient()
        {
            _tps = CreateTwitchPubSub();
            _client = CreateTwitchClient();

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
                Console.WriteLine(TwitchConstants.OnPubSubConnected);

#pragma warning disable CS0618 // Type or member is obsolete
                tps.ListenToRewards("47098493"); // this is the ID of the "AdamLearnsLive" channel
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

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine(TwitchConstants.OnClientConnectedMessage);
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine(TwitchConstants.OnChannelJoinMessage);
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
            string accessToken = config["twitch_access_token"] ?? throw new Exception(TwitchConstants.MissingAccessTokenError);
            _channelId = config["twitch_channel_name"] ?? throw new Exception(TwitchConstants.MissingChannelNameError);
            ConnectionCredentials credentials = new(_channelId, accessToken);
            ClientOptions clientOptions =
                new()
                {
                    MessagesAllowedInPeriod = TwitchConstants.MaximumMessages,
                    ThrottlingPeriod = TimeSpan.FromSeconds(TwitchConstants.ThrottlingInSeconds),
                };

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
                    HexColor = colorHex,
                    IsPrivileged = isPrivileged,
                }
            );
        }
    }
}
