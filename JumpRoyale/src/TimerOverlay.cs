using System.Threading.Tasks;
using Godot;

public partial class TimerOverlay : VFlowContainer
{
    private const string SpriteName = "Sprite2D";
    private const string TimerNodeName = SpriteName + "/Timer";

    private static readonly int GameLength = GameOverlay.GameLength;

    private int _timerSeconds = GameLength;
    private Color _white = new(1, 1, 1, 1);
    private Color _red = new(1, 0, 0, 1);
    [Signal]
    public delegate void TimerDoneEventHandler();

    public void Init()
    {
        _ = StartTimer();
    }

    public async Task StartTimer()
    {
        await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

        UpdateTimer();

        if (_timerSeconds <= 0)
        {
            EmitSignal(SignalName.TimerDone);
        }
        else
        {
            _ = StartTimer();
        }
    }

    private void UpdateTimer()
    {
        _timerSeconds--;

        int seconds = _timerSeconds % 60;
        int minutes = _timerSeconds / 60;

        GetNode<Label>(TimerNodeName).Text = $"{minutes}:{seconds:00}";
        Sprite2D sprite = GetNode<Sprite2D>(SpriteName);
        if (_timerSeconds > 15)
        {
            sprite.Modulate = _white;
            return;
        }

        sprite.Modulate = seconds % 2 == 0 ? _white : _red;
    }
}
