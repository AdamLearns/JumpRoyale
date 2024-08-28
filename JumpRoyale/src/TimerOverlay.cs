using System.Threading.Tasks;
using Godot;

public partial class TimerOverlay : VFlowContainer
{
    private const string SpriteName = "Background";
    private const string TimerNodeName = SpriteName + "/Timer";

    /// <summary>
    /// Determines when the timer will start flashing red.
    /// </summary>
    private const int FinalCountdown = 15;

    private const float ColorAlpha = 0.75f;

    private Color _defaultColor = new(1, 1, 1, ColorAlpha);
    private Color _flashColor = new(1, 0, 0, ColorAlpha);

    private Label _timerLabel = null!;

    private Sprite2D _sprite = null!;

    [Signal]
    public delegate void TimerDoneEventHandler();

    public int TimerSeconds { get; private set; } = GameOverlay.GameLength;

    public override void _Ready()
    {
        _timerLabel = GetNode<Label>(TimerNodeName);
        _sprite = GetNode<Sprite2D>(SpriteName);

        _sprite.Modulate = _defaultColor;
    }

    public void Init()
    {
        _ = StartTimer();
    }

    public async Task StartTimer()
    {
        await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

        UpdateTimer();

        if (TimerSeconds <= 0)
        {
            EmitSignal(SignalName.TimerDone);

            return;
        }

        _ = StartTimer();
    }

    private void UpdateTimer()
    {
        TimerSeconds--;

        int seconds = TimerSeconds % 60;
        int minutes = TimerSeconds / 60;

        _timerLabel.Text = $"{minutes}:{seconds:00}";

        if (TimerSeconds > FinalCountdown)
        {
            _sprite.Modulate = _defaultColor;

            return;
        }

        _sprite.Modulate = seconds % 2 == 0 ? _defaultColor : _flashColor;
    }
}
