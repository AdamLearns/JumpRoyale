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

    private const int GAME_LENGTH = 150;
    private int _timerSeconds = GAME_LENGTH;

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

        _timerSeconds--;
        GetNode<Label>(TimerNodeName).Text = $"Game ends in: {_timerSeconds}s";

        if (_timerSeconds % 30 == 0)
        {
            var arena = GetTree().Root.GetNode<Arena>("Arena");
            var camera = arena.GetNode<Camera2D>("Camera");
            var scaleFactor = _timerSeconds == GAME_LENGTH - 60 ? 3f : 2f;
            camera.PositionSmoothingSpeed *= scaleFactor;
            GetNode<Label>(CameraScrollSpeedNodeName).Text = $"Camera speed: {camera.PositionSmoothingSpeed} px/s";
        }

        if (_timerSeconds <= 0)
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
        GetNode<Label>(HeightOutputNodeName).Text =
            $"Leader: {playerName} at height={Formatter.FormatBigNumber(height)}";
    }
}
