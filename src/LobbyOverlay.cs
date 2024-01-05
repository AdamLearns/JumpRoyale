using System.Threading.Tasks;
using Godot;

public partial class LobbyOverlay : FlowContainer
{
	private const string LobbyTimerNodeName = "LobbyTimer";
	private const string NumPlayersNodeName = "NumPlayers";

	[Signal]
	public delegate void TimerDoneEventHandler();

	private int lobbyTimer = 40;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var arena = GetTree().Root.GetNode<Arena>("Arena");

		arena.PlayerCountChange += OnPlayerCountChange;

		_ = StartTimer();
	}

	public async Task StartTimer()
	{
		await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

		lobbyTimer--;
		GetNode<Label>(LobbyTimerNodeName).Text = $"Game starts in: {lobbyTimer}s";

		if (lobbyTimer <= 0)
		{
			EmitSignal(SignalName.TimerDone);
		}
		else
		{
			_ = StartTimer();
		}
	}

	public void OnPlayerCountChange(int numPlayers)
	{
		GetNode<Label>(NumPlayersNodeName).Text = $"Players: {numPlayers}";
	}
}
