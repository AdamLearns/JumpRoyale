using Godot;
using TwitchChat;

public partial class DeleteMe : Sprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TwitchChatClient twitchChatClient = new();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = new Vector2(Position.X + 1, Position.Y);
	}
}
