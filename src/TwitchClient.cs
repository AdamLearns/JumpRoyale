using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Microsoft.Extensions.Configuration;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace TwitchChat
{

  public class MessageEventArgs : EventArgs
  {
	public string Message { get; set; }
	public string SenderName { get; set; }
	public string SenderId { get; set; }

	public string HexColor { get; set; }
	public bool IsPrivileged { get; set; }
  }

  class TwitchChatClient
  {
	readonly TwitchClient client;

	public event EventHandler<MessageEventArgs> OnMessage;
	public event EventHandler<OnRewardRedeemedArgs> OnRedemption;

	private TwitchPubSub tps;

	public TwitchChatClient()
	{

	  tps = new TwitchPubSub();
	  tps.OnPubSubServiceConnected += (object sender, EventArgs e) =>
	  {
		Console.WriteLine("PubSub connected");
		tps.ListenToRewards("47098493");
		tps.SendTopics();
		tps.OnRewardRedeemed += (object sender, OnRewardRedeemedArgs e) =>
		{
		  Console.WriteLine($"Reward redeemed: {e}");
		  OnRedemption.Invoke(this, e);
		};
	  };
	  tps.Connect();

	  var config = new ConfigurationBuilder().AddUserSecrets<TwitchChatClient>().Build();
	  string accessToken = config["twitch_access_token"] ?? throw new Exception("No access token found. Please run `dotnet user-secrets set twitch_access_token <your access token>`");
	  string channel = config["twitch_channel_name"] ?? throw new Exception("Channel not found. Please run `dotnet user-secrets set twitch_channel_name <your twitch channel>`");
	  ConnectionCredentials credentials = new ConnectionCredentials(channel, accessToken);
	  var clientOptions = new ClientOptions
	  {
		MessagesAllowedInPeriod = 750,
		ThrottlingPeriod = TimeSpan.FromSeconds(30)
	  };
	  WebSocketClient customClient = new WebSocketClient(clientOptions);
	  client = new TwitchClient(customClient);
	  client.Initialize(credentials, channel);

	  client.OnJoinedChannel += Client_OnJoinedChannel;
	  client.OnMessageReceived += Client_OnMessageReceived;
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
	  var isPrivileged = e.ChatMessage.IsSubscriber || e.ChatMessage.IsModerator || e.ChatMessage.IsVip;
	  HandleChatMessage(e.ChatMessage.UserId, e.ChatMessage.DisplayName, e.ChatMessage.Message, e.ChatMessage.ColorHex, isPrivileged);
	}

	private void HandleChatMessage(string senderId, string senderName, string message, string colorHex, bool isPrivileged)
	{
	  OnMessage.Invoke(this, new MessageEventArgs
	  {
		Message = message,
		SenderName = senderName,
		SenderId = senderId,
		HexColor = colorHex ?? "#ffffff",
		IsPrivileged = isPrivileged
	  });
	}
  }
}
