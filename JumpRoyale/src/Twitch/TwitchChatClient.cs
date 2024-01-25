using System;
using TwitchLib.Client.Events;
using TwitchLib.PubSub.Events;

namespace TwitchChat
{
    public class TwitchChatClient : BaseChatClient
    {
        public TwitchChatClient(string channelId)
            : base(channelId)
        {
            SubscribeToEvents();
        }

        public event EventHandler<ChatMessageEventArgs>? OnMessageEvent;

        public event EventHandler<OnChannelPointsRewardRedeemedArgs>? OnRedemptionEvent;

        private void SubscribeToEvents()
        {
            TwitchPubSub.OnPubSubServiceConnected += OnPubSubServiceConnected;
            TwitchPubSub.OnChannelPointsRewardRedeemed += OnChannelPointsRewardRedeemed;

            TwitchClient.OnJoinedChannel += OnJoinedChannel;
            TwitchClient.OnMessageReceived += OnMessageReceived;
            TwitchClient.OnConnected += OnConnected;
        }

        private void OnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e)
        {
            Console.WriteLine(TwitchClientMessages.OnRewardRedeemMessage);
            Console.WriteLine(e.RewardRedeemed.Redemption.Id);

            OnRedemptionEvent?.Invoke(this, e);
        }

        private void OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine(TwitchClientMessages.OnClientConnectedMessage);
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine(TwitchClientMessages.OnChannelJoinMessage);
        }

        private void OnPubSubServiceConnected(object sender, EventArgs e)
        {
            Console.WriteLine(TwitchClientMessages.OnPubSubConnected);

            TwitchPubSub.ListenToChannelPoints(ChannelId); // this is the ID of the "AdamLearnsLive" channel
            TwitchPubSub.SendTopics();
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
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

        private void HandleChatMessage(
            string senderId,
            string senderName,
            string message,
            string colorHex,
            bool isPrivileged
        )
        {
            OnMessageEvent?.Invoke(
                this,
                new ChatMessageEventArgs
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
