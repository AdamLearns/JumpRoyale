using System;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.PubSub;

namespace TwitchChat;

public class BaseChatClient
{
    protected BaseChatClient()
    {
        TwitchClient = InitializeClient();

        ConnectToTwitch();
    }

    protected ChannelConfiguration Configuration { get; private set; } = new();

    protected TwitchPubSub TwitchPubSub { get; private set; } = new();

    protected TwitchClient TwitchClient { get; private set; }

    private TwitchClient InitializeClient()
    {
        (ConnectionCredentials credentials, ClientOptions options) = ConfigureClient();
        WebSocketClient customClient = new(options);
        TwitchClient client = new(customClient);

        client.Initialize(credentials, Configuration.ChannelName);

        return client;
    }

    private Tuple<ConnectionCredentials, ClientOptions> ConfigureClient()
    {
        ConnectionCredentials credentials = new(Configuration.ChannelName, Configuration.AccessToken);
        ClientOptions clientOptions =
            new()
            {
                MessagesAllowedInPeriod = TwitchConstants.MaximumMessages,
                ThrottlingPeriod = TimeSpan.FromSeconds(TwitchConstants.ThrottlingInSeconds),
            };

        return new(credentials, clientOptions);
    }

    private void ConnectToTwitch()
    {
        TwitchPubSub.Connect();
        TwitchClient.Connect();
    }
}
