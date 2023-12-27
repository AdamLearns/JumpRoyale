using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Microsoft.Extensions.Configuration;

namespace TwitchChat
{
  class TwitchChatClient
  {
    readonly TwitchClient client;

    public TwitchChatClient()
    {
      var config = new ConfigurationBuilder().AddUserSecrets<TwitchChatClient>().Build();
      string accessToken = config["twitch_access_token"] ?? throw new Exception("No access token found. Please run `dotnet user-secrets set twitch_access_token <your access token>`");
      ConnectionCredentials credentials = new ConnectionCredentials("WhyOhWhyOhWhyOh", accessToken);
      var clientOptions = new ClientOptions
      {
        MessagesAllowedInPeriod = 750,
        ThrottlingPeriod = TimeSpan.FromSeconds(30)
      };
      WebSocketClient customClient = new WebSocketClient(clientOptions);
      client = new TwitchClient(customClient);
      client.Initialize(credentials, "AdamLearnsLive");

      client.OnJoinedChannel += Client_OnJoinedChannel;
      client.OnMessageReceived += Client_OnMessageReceived;
      client.OnWhisperReceived += Client_OnWhisperReceived;
      client.OnConnected += Client_OnConnected;

      client.Connect();
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
      HandleChatMessage(e.ChatMessage.UserId, e.ChatMessage.Username, e.ChatMessage.Message);
    }

    private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
    {
      HandleChatMessage(e.WhisperMessage.UserId, e.WhisperMessage.Username, e.WhisperMessage.Message);
    }

    private void HandleChatMessage(string senderId, string senderName, string message)
    {
      Console.WriteLine($"Got message from {senderName}: {message}");
    }
  }
}
