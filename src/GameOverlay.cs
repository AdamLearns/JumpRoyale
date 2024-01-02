using System.Threading.Tasks;
using Godot;

public partial class GameOverlay : VFlowContainer
{

	private const string TimerNodeName = "Timer";
	private const string NumPlayersNodeName = "NumPlayers";
	private const string HeightOutputNodeName = "HeightOutput";
	private const string CameraScrollSpeedNodeName = "CameraScrollSpeed";

	[Signal]
	public delegate void TimerDoneEventHandler();

	private int timerSeconds = 150;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var arena = GetTree().Root.GetNode<Arena>("Arena");

		arena.PlayerCountChange += OnPlayerCountChange;
		arena.MaxHeightChanged += OnMaxHeightChange;
	}

	public void init()
	{
		_ = StartTimer();

		var arena = GetTree().Root.GetNode<Arena>("Arena");
		var camera = arena.GetNode<Camera2D>("Camera");
		GetNode<Label>(CameraScrollSpeedNodeName).Text = $"Camera speed: {camera.PositionSmoothingSpeed} px/s";
	}

	public async Task StartTimer()
	{
		await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

		timerSeconds--;
		GetNode<Label>(TimerNodeName).Text = $"Game ends in: {timerSeconds}s";

		if (timerSeconds % 30 == 0)
		{
			var arena = GetTree().Root.GetNode<Arena>("Arena");
			var camera = arena.GetNode<Camera2D>("Camera");
			camera.PositionSmoothingSpeed *= 2f;
			GetNode<Label>(CameraScrollSpeedNodeName).Text = $"Camera speed: {camera.PositionSmoothingSpeed} px/s";
		}

		if (timerSeconds <= 0)
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

	public void OnMaxHeightChange(string playerName, int height)
	{
		GetNode<Label>(HeightOutputNodeName).Text = $"Leader: {playerName} at height={Formatter.FormatBigNumber(height)}";
	}
}
