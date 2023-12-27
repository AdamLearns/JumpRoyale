using Godot;
using System;
using System.Collections.Generic;
using TwitchChat;

public partial class Arena : Node2D
{

	[Export]
	public PackedScene JumperScene;

	private Dictionary<string, Jumper> jumpers = new Dictionary<string, Jumper>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TwitchChatClient twitchChatClient = new();
		twitchChatClient.OnMessage += OnMessage;
	}

	private void OnMessage(object sender, MessageEventArgs e)
	{
		if (e.Message.StartsWith("!reset"))
		{
			CallDeferred(nameof(ResetPlayer), e.SenderId);
		}
		if (e.Message.StartsWith("!join"))
		{
			CallDeferred(nameof(AddPlayer), e.SenderId, e.SenderName, "#ffffff");
		}
		if (e.Message.StartsWith("!jump"))
		{
			HandleJump(e);
		}
	}

	private void HandleJumpWithPowerAndAngle(string[] parts, MessageEventArgs e)
	{
		if (!int.TryParse(parts[1], out int angle) || !int.TryParse(parts[2], out int power))
		{
			return;
		}

		angle = Math.Clamp(angle, -90, 90);
		angle += 90;
		CallDeferred(nameof(JumpPlayer), e.SenderId, angle, power);
	}

	private void HandleJumpWithDirection(string[] parts, MessageEventArgs e)
	{
		string dir = parts[1];

		if (!int.TryParse(parts[2], out int angle) || !int.TryParse(parts[3], out int power))
		{
			return;
		}

		angle = Math.Clamp(angle, 0, 90);
		if (dir.ToLower().StartsWith("l"))
		{
			angle = 90 - angle;
		}
		else
		{
			angle += 90;
		}
		CallDeferred(nameof(JumpPlayer), e.SenderId, angle, power);
	}


	private void HandleJump(MessageEventArgs e)
	{
		string[] parts = e.Message.Split(" ");

		if (parts.Length == 3)
		{
			HandleJumpWithPowerAndAngle(parts, e);
		}
		else if (parts.Length == 4)
		{
			HandleJumpWithDirection(parts, e);
		}
	}

	private void JumpPlayer(string userId, int angle, int power)
	{
		if (!jumpers.ContainsKey(userId))
		{
			return;
		}

		Jumper jumper = jumpers[userId];
		jumper.Jump(angle, power);
	}

	private void AddPlayer(string userId, string userName, string hexColor)
	{
		if (jumpers.ContainsKey(userId))
		{
			return;
		}

		RandomNumberGenerator rng = new RandomNumberGenerator();
		Jumper jumper = JumperScene.Instantiate() as Jumper;
		jumper.Init(rng.RandiRange(0, 1000), userName);
		AddChild(jumper);

		jumpers.Add(userId, jumper);
	}

	private void ResetPlayer(string userId)
	{
		if (!jumpers.ContainsKey(userId))
		{
			return;
		}
		Jumper jumper = jumpers[userId];
		jumper.Reset();
	}
}
