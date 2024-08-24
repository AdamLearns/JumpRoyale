using System.Threading.Tasks;
using Godot;

public partial class TimerOverlay : VFlowContainer
{
    private const string SpriteName = "Background";
    private const string TimerNodeName = SpriteName + "/Timer";

    [Signal]
    public delegate void TimerDoneEventHandler();

    public int TimerSeconds { get; private set; } = GameOverlay.GameLength;

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

        GetNode<Label>(TimerNodeName).Text = $"{minutes}:{seconds:00}";
        Sprite2D sprite = GetNode<Sprite2D>(SpriteName);
        if (TimerSeconds > 15)
        {
            sprite.Modulate = Colors.White;
            return;
        }

        sprite.Modulate = seconds % 2 == 0 ? Colors.White : Colors.Red;
    }
}
