using System.Threading.Tasks;
using Godot;

public partial class LobbyOverlay : FlowContainer
{
    private const string LobbyTimerNodeName = "LobbyTimer";
    private const string NumPlayersNodeName = "NumPlayers";

    private int _lobbyTimer = 40;

    [Signal]
    public delegate void TimerDoneEventHandler();

    public override void _Ready()
    {
        Arena arena = GetTree().Root.GetNode<Arena>("Arena");

        arena.PlayerCountChange += OnPlayerCountChange;

        _ = StartTimer();
    }

    private async Task StartTimer()
    {
        await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

        UpdateCountdown();

        if (_lobbyTimer <= 0)
        {
            EmitSignal(SignalName.TimerDone);
        }
        else
        {
            _ = StartTimer();
        }
    }

    private void OnPlayerCountChange(int numPlayers)
    {
        GetNode<Label>(NumPlayersNodeName).Text = $"Players: {numPlayers}";
    }

    private void UpdateCountdown()
    {
        _lobbyTimer--;

        GetNode<Label>(LobbyTimerNodeName).Text = $"Game starts in: {_lobbyTimer}s";
    }
}
