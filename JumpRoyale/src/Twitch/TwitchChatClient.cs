using System;
using TwitchLib.Client.Events;
using TwitchLib.PubSub.Events;

namespace TwitchChat;

public class TwitchChatClient : BaseChatClient
{
    public TwitchChatClient()
    {
        SubscribeToEvents();
    }

    public event EventHandler<ChatMessageEventArgs>? OnMessageEvent;

    public event EventHandler<OnRewardRedeemedArgs>? OnRedemptionEvent;

    private void SubscribeToEvents()
    {
        TwitchPubSub.OnPubSubServiceConnected += OnPubSubServiceConnected;
#pragma warning disable CS0618 // Type or member is obsolete
        TwitchPubSub.OnRewardRedeemed += OnRewardRedeemed;
#pragma warning restore CS0618 // Type or member is obsolete

        TwitchClient.OnJoinedChannel += OnJoinedChannel;
        TwitchClient.OnMessageReceived += OnMessageReceived;
        TwitchClient.OnConnected += OnConnected;
    }

    private void OnRewardRedeemed(object sender, OnRewardRedeemedArgs e)
    {
        Console.WriteLine(TwitchConstants.OnRewardRedeemMessage);
        Console.WriteLine(e);

        OnRedemptionEvent?.Invoke(this, e);
    }

    private void OnConnected(object sender, OnConnectedArgs e)
    {
        Console.WriteLine(TwitchConstants.OnClientConnectedMessage);
    }

    private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        Console.WriteLine(TwitchConstants.OnChannelJoinMessage);
    }

    private void OnPubSubServiceConnected(object sender, EventArgs e)
    {
        Console.WriteLine(TwitchConstants.OnPubSubConnected);

#pragma warning disable CS0618 // Type or member is obsolete
        TwitchPubSub.ListenToRewards(Configuration.ChannelId);
#pragma warning restore CS0618 // Type or member is obsolete

        TwitchPubSub.SendTopics();
    }

    private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        bool isPrivileged =
            e.ChatMessage.IsSubscriber
            || e.ChatMessage.IsModerator
            || e.ChatMessage.IsVip
            || e.ChatMessage.IsBroadcaster;

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
