using System.Threading.Tasks;
using Godot;

public partial class GameOverlay : VFlowContainer
{
    public static readonly int GameLength = 150; // in seconds

    private const string NumPlayersNodeName = "NumPlayers";
    private const string HeightOutputNodeName = "HeightOutput";
    private const string CameraScrollSpeedNodeName = "CameraScrollSpeed";

    private int _timerSeconds = GameLength;

    private Arena _arena = null!;

    private Camera2D _camera = null!;

    private Label _cameraScrollSpeedLabel = null!;
    private Label _playerCountLabel = null!;
    private Label _heightOutputLabel = null!;

    [Signal]
    public delegate void TimerDoneEventHandler();

    public string CurrentCameraSpeed => $"Camera speed: {_camera.PositionSmoothingSpeed} px/s";

    public override void _Ready()
    {
        _arena = GetTree().Root.GetNode<Arena>("Arena");
        _camera = _arena.GetNode<Camera2D>("Camera");
        _cameraScrollSpeedLabel = GetNode<Label>(CameraScrollSpeedNodeName);
        _playerCountLabel = GetNode<Label>(NumPlayersNodeName);
        _heightOutputLabel = GetNode<Label>(HeightOutputNodeName);

        _arena.PlayerCountChange += OnPlayerCountChange;
        _arena.MaxHeightChanged += OnMaxHeightChange;
    }

    public void Init()
    {
        _ = StartTimer();
        _cameraScrollSpeedLabel.Text = CurrentCameraSpeed;
    }

    public async Task StartTimer()
    {
        await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

        _timerSeconds--;

        if (_timerSeconds % 30 == 0)
        {
            float scaleFactor = _timerSeconds == GameLength - 60 ? 3f : 2f;

            _camera.PositionSmoothingSpeed *= scaleFactor;
            _cameraScrollSpeedLabel.Text = CurrentCameraSpeed;
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
        _playerCountLabel.Text = $"Players: {numPlayers}";
    }

    public void OnMaxHeightChange(string playerName, int height)
    {
        _heightOutputLabel.Text = $"Leader: {playerName} at height: {Formatter.FormatBigNumber(height)}";
    }
}
