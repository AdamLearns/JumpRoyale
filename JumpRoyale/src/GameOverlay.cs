using System.Threading.Tasks;
using Godot;

public partial class GameOverlay : VFlowContainer
{
    public static readonly int GameLength = 150;

    private const string NumPlayersNodeName = "NumPlayers";
    private const string HeightOutputNodeName = "HeightOutput";
    private const string CameraScrollSpeedNodeName = "CameraScrollSpeed";

    private int _timerSeconds = GameLength;

    [Signal]
    public delegate void TimerDoneEventHandler();

    public override void _Ready()
    {
        Arena arena = GetTree().Root.GetNode<Arena>("Arena");

        arena.PlayerCountChange += OnPlayerCountChange;
        arena.MaxHeightChanged += OnMaxHeightChange;
    }

    public void Init()
    {
        _ = StartTimer();

        Arena arena = GetTree().Root.GetNode<Arena>("Arena");
        Camera2D camera = arena.GetNode<Camera2D>("Camera");
        GetNode<Label>(CameraScrollSpeedNodeName).Text = $"Camera speed: {camera.PositionSmoothingSpeed} px/s";
    }

    public async Task StartTimer()
    {
        await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

        UpdateTimer();

        if (_timerSeconds % 30 == 0)
        {
            Arena arena = GetTree().Root.GetNode<Arena>("Arena");
            Camera2D camera = arena.GetNode<Camera2D>("Camera");
            float scaleFactor = _timerSeconds == GameLength - 60 ? 3f : 2f;

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

    private void UpdateTimer()
    {
        _timerSeconds--;
    }
}
