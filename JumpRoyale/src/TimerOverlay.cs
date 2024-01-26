using System.Threading.Tasks;
using Godot;

public partial class TimerOverlay : VFlowContainer
{
    private const string SpriteName = "Background";
    private const string TimerNodeName = SpriteName + "/Timer";

    private static readonly int GameLength = GameOverlay.GameLength;

    private int _timerSeconds = GameLength;

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
            return;
        }

        _ = StartTimer();
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
            sprite.Modulate = Colors.White;
            return;
        }

        sprite.Modulate = seconds % 2 == 0 ? Colors.White : Colors.Red;
    }
}
