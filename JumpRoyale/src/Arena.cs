using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using TwitchChat;
using TwitchLib.PubSub.Events;

public partial class Arena : Node2D
{
    public const int WallHeightInTiles = 15;
    public const int ArenaHeightInTiles = 600;
    public const int TileSizeInPixels = 16;

    /// <summary>
    /// If the game timer reaches this value, players won't be able to revive anymore.
    /// </summary>
    public const int RevivePreventionCountdown = 30;

    private const string LobbyOverlayNodeName = "LobbyOverlay";
    private const string GameOverlayNodeName = "GameOverlay";
    private const string TimerOverlayNodeName = "TimerOverlay";
    private const string EndScreenOverlayNodeName = "EndScreenOverlay";
    private const string CameraNodeName = "Camera";
    private const string CanvasLayerNodeName = "CanvasLayer";
    private const string EndScreenOutput = "%EndScreenOutput";

    private readonly Dictionary<string, InstructionBot> _instructionBotsTemplates =
        new()
        {
            ["jumps_left"] = new(
                "jumps_left",
                "[color=cyan]j -45[/color] or [color=cyan]l 45[/color]",
                TileSizeInPixels * 105,
                45
            ),
            ["jumps_up"] = new("jumps_up", "[color=cyan]u[/color]", TileSizeInPixels * 60, 90),
            ["jumps_right"] = new(
                "jumps_right",
                "[color=cyan]j 30[/color] or [color=cyan]r 30[/color]",
                TileSizeInPixels * 20,
                120
            ),
        };

    private readonly Collection<Jumper> _spawnedInstructionBots = [];

    private TimerOverlay _timerOverlay = null!;
    private TileMap _lobbyTilemap = new();

    private ArenaBuilder _arenaBuilder = null!;

    private bool _hasGameStarted;
    private bool _hasGameEnded;

    private int _ceilingHeight;
    private int _choice = 1;
    private int _generatedMaxHeight;
    private int _heightInTiles;
    private int _widthInTiles;

    private long _timeSinceGameEnd;

    [Signal]
    public delegate void PlayerCountChangeEventHandler(int numPlayers);

    [Signal]
    public delegate void MaxHeightChangedEventHandler(string playerName, int height);

    [Signal]
    public delegate void CameraSpeedChangedEventHandler(int speed);

    public static int ViewportHeight { get; private set; }

    [Export]
    public PackedScene? JumperScene { get; private set; }

    [Export]
    public TileSet? TileSetToUse { get; private set; }

    public override void _Ready()
    {
        ViewportHeight = (int)GetViewportRect().Size.Y;
        PlayerStats.Instance.StatsFilePath = ProjectSettings.GlobalizePath(ResourcePathsConstants.PathToPlayerStats);
        PlayerStats.Instance.LoadPlayerData();

        _timerOverlay = GetTimerOverlay();
        _lobbyTilemap = new TileMap { Name = "TileMap", TileSet = TileSetToUse };
        _arenaBuilder = new(_lobbyTilemap);

        TwitchChatClient twitchChatClient = new();

        twitchChatClient.OnRedemptionEvent += OnRedemption;
        twitchChatClient.OnMessageEvent += OnMessage;
        GetLobbyOverlay().TimerDone += OnLobbyTimerDone;
        GetGameOverlay().TimerDone += OnGameTimerDone;

        SetBackground();
        GenerateLobby();
        SpawnInstructionBots();
    }

    public override void _PhysicsProcess(double delta)
    {
        ModifyPlayerScales();
        GenerateProceduralPlatforms();
        MoveCamera();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_accept") && Input.IsPhysicalKeyPressed(Key.Alt))
        {
            DisplayServer.WindowSetMode(
                DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen
                    ? DisplayServer.WindowMode.Maximized
                    : DisplayServer.WindowMode.Fullscreen
            );
        }

        if (Input.IsPhysicalKeyPressed(Key.W) && Input.IsPhysicalKeyPressed(Key.Ctrl))
        {
            OnGameTimerDone();
        }

        if (Input.IsPhysicalKeyPressed(Key.B))
        {
            for (int i = 0; i < 200; i++)
            {
                // Note: it does not matter that we instantiate this 200 times
                CommandHandler commandHandler =
                    new(string.Empty, i.ToString(), i.ToString(), "ffffff", false) { Arena = this };

                commandHandler.SpawnFakePlayers();
            }
        }

        // Make everyone jump, no exceptions
        if (Input.IsPhysicalKeyPressed(Key.J))
        {
            foreach (Jumper jumper in ActiveJumpers.Instance.AllJumpers())
            {
                jumper.RandomJump();
                jumper.FlashPlayerName();
            }
        }
    }

    /// <summary>
    /// Players are allowed to jump only if the game is still running or if 5
    /// seconds have passed since the game ended (that way players don't jump
    /// off the podiums due to a stream delay).
    /// </summary>
    public bool IsAllowedToJump()
    {
        return _timeSinceGameEnd <= 0 || (DateTime.Now.Ticks - _timeSinceGameEnd) / TimeSpan.TicksPerMillisecond > 5000;
    }

    private void SpawnInstructionBots()
    {
        Ensure.IsNotNull(JumperScene);
        Ensure.IsNotNull(TileSetToUse);

        // Position the instruction bots above the lobby
        int y = ((int)(GetViewportRect().Size.Y / TileSizeInPixels) - 1 - WallHeightInTiles - 5) * TileSizeInPixels;

        foreach (KeyValuePair<string, InstructionBot> bot in _instructionBotsTemplates)
        {
            // This is a modified CommandHandler logic portion for adding players, except we are not adding an actual
            // player, but a dummy jumper, which won't exist in the player data or active jumpers list
            Jumper jumper = (Jumper)JumperScene.Instantiate();

            // Create some fake data just to make Jumper methods work
            PlayerData playerData =
                new("ffffff", GD.RandRange(1, 18), "ffffff")
                {
                    Name = bot.Value.DisplayName,
                    UserId = bot.Value.UserId,
                };

            jumper.Init(bot.Value.XSpawnPosition, y, playerData);
            AddChild(jumper);
            _spawnedInstructionBots.Add(jumper);
        }

        _ = MakeBotsAutoJump(y);
    }

    /// <param name="y">Initial Y position, used to reset the bots on Y-axis.</param>
    private async Task MakeBotsAutoJump(int y)
    {
        await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

        // Abort this task if the game has already started, because the jumpers will be Freed at this point
        if (_hasGameStarted)
        {
            return;
        }

        foreach (Jumper bot in _spawnedInstructionBots)
        {
            bot.Jump(_instructionBotsTemplates[bot.PlayerData.UserId].JumpAngle, 100);
            bot.FlashPlayerName();
        }

        await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

        // The game may have started since we last checked
        if (_hasGameStarted)
        {
            return;
        }

        // Reset the bots to their original position
        foreach (Jumper bot in _spawnedInstructionBots)
        {
            bot.Position = new Vector2(_instructionBotsTemplates[bot.PlayerData.UserId].XSpawnPosition, y);
            bot.FlashPlayerName();
        }

        _ = MakeBotsAutoJump(y);
    }

    private void OnMessage(object sender, ChatMessageEventArgs e)
    {
        // This is kind of a workaround for now to have a top level Defer and be able to pass objects around inside,
        // which can't be converted to Godot's Variant. We still have to pull the required data separately, because `e`
        // is an object :(
        CallDeferred(nameof(HandleCommands), e.Message, e.SenderId, e.SenderName, e.HexColor, e.IsPrivileged);
    }

    private void HandleCommands(string message, string senderId, string senderName, string hexColor, bool isPrivileged)
    {
        CommandHandler commandHandler = new(message, senderId, senderName, hexColor, isPrivileged) { Arena = this };

        commandHandler.ProcessMessage();
    }

    private void SetBackground()
    {
        Sprite2D backgroundNode = GetNode<Sprite2D>("Background");
        string[] backgrounds = ["Bricks", "Tiles", "TilesAlt", "BrickWallAlt", "BrickWall", "BricksAlt"];
        string background = backgrounds[Rng.IntRange(0, backgrounds.Length - 1)];

        backgroundNode.Texture = ResourceLoader.Load<Texture2D>($"res://assets/sprites/backgrounds/{background}.png");
    }

    private PanelContainer GetEndScreenOverlay()
    {
        return GetNode<CanvasLayer>(CanvasLayerNodeName).GetNode<PanelContainer>(EndScreenOverlayNodeName);
    }

    private GameOverlay GetGameOverlay()
    {
        return GetNode<CanvasLayer>(CanvasLayerNodeName).GetNode<GameOverlay>(GameOverlayNodeName);
    }

    private TimerOverlay GetTimerOverlay()
    {
        return GetNode<CanvasLayer>(CanvasLayerNodeName).GetNode<TimerOverlay>(TimerOverlayNodeName);
    }

    private LobbyOverlay GetLobbyOverlay()
    {
        return GetNode<CanvasLayer>(CanvasLayerNodeName).GetNode<LobbyOverlay>(LobbyOverlayNodeName);
    }

    private void GenerateLobby()
    {
        Ensure.IsNotNull(TileSetToUse);

        Rect2 viewport = GetViewportRect();

        _heightInTiles = (int)(viewport.Size.Y / TileSetToUse.TileSize.Y);
        _widthInTiles = (int)(viewport.Size.X / TileSetToUse.TileSize.X);

        int floorY = _heightInTiles - 3;

        // Draw grass along the bottom to form the floor
        _lobbyTilemap.SetCell(0, new Vector2I(0, floorY), 0, new Vector2I(6, 0));
        _lobbyTilemap.SetCell(0, new Vector2I(0, floorY + 1), 0, new Vector2I(6, 1));
        _lobbyTilemap.SetCell(0, new Vector2I(0, floorY + 2), 0, new Vector2I(6, 2));

        _lobbyTilemap.SetCell(0, new Vector2I(_widthInTiles - 1, floorY), 0, new Vector2I(8, 0));
        _lobbyTilemap.SetCell(0, new Vector2I(_widthInTiles - 1, floorY + 1), 0, new Vector2I(8, 1));
        _lobbyTilemap.SetCell(0, new Vector2I(_widthInTiles - 1, floorY + 2), 0, new Vector2I(8, 2));

        for (int x = 1; x < _widthInTiles - 1; x++)
        {
            _lobbyTilemap.SetCell(0, new Vector2I(x, floorY), 0, new Vector2I(7, 0));
            _lobbyTilemap.SetCell(0, new Vector2I(x, floorY + 1), 0, new Vector2I(7, 1));
            _lobbyTilemap.SetCell(0, new Vector2I(x, floorY + 2), 0, new Vector2I(7, 2));
        }

        int wallStartY = floorY - 1;

        _ceilingHeight = wallStartY - WallHeightInTiles - 1;

        // Draw the vertical walls
        for (int y = wallStartY; y >= _ceilingHeight; y--)
        {
            _arenaBuilder.DrawPoint(0, y);
            _arenaBuilder.DrawPoint(_widthInTiles - 1, y);
        }

        for (int y = wallStartY; y >= _ceilingHeight - 200; y--)
        {
            _arenaBuilder.DrawPoint(0, y);
            _arenaBuilder.DrawPoint(_widthInTiles - 1, y);
        }

        // Draw the ceiling
        for (int x = 1; x < _widthInTiles - 1; x++)
        {
            _arenaBuilder.DrawPoint(x, _ceilingHeight);
        }

        // Generate some lobby platforms
        int platformStartY = wallStartY - 2;
        int platformEndY = _ceilingHeight + 4;

        for (int y = platformStartY; y >= platformEndY; y--)
        {
            int width = Rng.IntRange(3, 15);
            int startX = Rng.IntRange(2, _widthInTiles - width - 3);

            _arenaBuilder.DrawPlatform(startX, y, width);
        }

        for (int y = _ceilingHeight - 1; y >= _ceilingHeight - ArenaHeightInTiles; y--)
        {
            float difficultyFactor = GetDifficultyFactor(y);

            // Rarely, make a solid block to add some variety
            int r = Rng.IntRange(0, 100);

            if (r < 6 + difficultyFactor * 40)
            {
                int blockWidth = 2 + (int)(difficultyFactor * 24);
                int blockX = Rng.IntRange(2, _widthInTiles - 1 - blockWidth);

                DrawRectangleOfTiles(blockX, y + 1, blockWidth, blockWidth);
            }
        }

        _generatedMaxHeight = _ceilingHeight;

        AddChild(_lobbyTilemap);
    }

    /// <summary>
    /// Returns a difficulty factor that goes from 0 to 1 linearly as Y decreases.
    /// </summary>
    /// <param name="y">A Y coordinate in tiles.</param>
    private float GetDifficultyFactor(int y)
    {
        return (float)Math.Min(0, y) / -ArenaHeightInTiles;
    }

    private void GenerateProceduralPlatforms()
    {
        Ensure.IsNotNull(TileSetToUse);

        Camera2D camera = GetNode<Camera2D>(CameraNodeName);
        Rect2 viewport = GetViewportRect();

        // NOTE(Hop): GetScreenCenterPosition was the only way to get accurate viewport position
        //            without ignoring Position Smoothing
        float cameraPos = camera.GetScreenCenterPosition().Y - (viewport.Size.Y / 2);
        int cameraPosInTiles = (int)(cameraPos / TileSetToUse.TileSize.Y);

        if (cameraPosInTiles >= _generatedMaxHeight)
        {
            return;
        }

        // Add platforms higher up;
        for (int y = _generatedMaxHeight - 1; y >= cameraPosInTiles; y--)
        {
            float difficultyFactor = GetDifficultyFactor(y);
            int r = Rng.IntRange(0, 100);

            if (r > (70 - difficultyFactor * 60))
            {
                continue;
            }

            int width = Rng.IntRange(3, 15 - (int)Math.Round(6 * difficultyFactor));
            int startX = Rng.IntRange(2, _widthInTiles - width - 3);

            _arenaBuilder.DrawPlatform(startX, y, width);
        }

        _generatedMaxHeight = cameraPosInTiles;
    }

    private void OnGameTimerDone()
    {
        GetGameOverlay().Visible = false;
        _timerOverlay.Visible = false;
        _hasGameEnded = true;
        _timeSinceGameEnd = DateTime.Now.Ticks;

        string[] winners = ActiveJumpers.Instance.ComputeStats();

        PlayerStats.Instance.SaveAllPlayers();
        ShowEndScreen(winners);
        GenerateEndArena(winners);
    }

    private void GenerateEndArena(string[] winners)
    {
        Ensure.IsNotNull(TileSetToUse);

        // Draw border around the whole arena and clear any platforms
        for (int x = 0; x < _widthInTiles; x++)
        {
            for (int y = _heightInTiles; y > -_heightInTiles; y--)
            {
                if (x == 0 || x == _widthInTiles - 1 || y == _heightInTiles - 1)
                {
                    _arenaBuilder.DrawPoint(x, y);
                }
                else
                {
                    _arenaBuilder.RemovePoint(x, y);
                }
            }
        }

        int numPodiums = 3;
        int podiumWidth = 6;
        int podiumHeight = 6; // has to be divisible by numPodiums
        int podiumHeightDifference = podiumHeight / numPodiums;
        int podiumX = _widthInTiles / 2;
        int podiumY = 13 + podiumHeight;

        List<Tuple<int, int>> platformCoords = [];

        // Add some platforms so that there are "fun" jumps to make
        int startY = podiumY + podiumHeight + 10;
        for (int y = startY; y < _heightInTiles; y += 6)
        {
            for (int x = Rng.IntRange(3, 15); x < _widthInTiles - 5; x += Rng.IntRange(8, 15))
            {
                _arenaBuilder.DrawPlatform(x, y, GD.RandRange(0, 2));
                platformCoords.Add(new Tuple<int, int>(x, y));
            }
        }

        // Put all players back in the arena
        foreach (Jumper jumper in ActiveJumpers.Instance.AllJumpers())
        {
            int platformNumber = Rng.IntRange(0, platformCoords.Count - 1);
            (int platformX, int platformY) = platformCoords[platformNumber];

            jumper.Position = new Vector2(
                (platformX + 0.5f) * TileSetToUse.TileSize.X,
                platformY * TileSetToUse.TileSize.Y - 5
            );
            jumper.Scale = new Vector2(1, 1);
            jumper.Velocity = new Vector2(0, 0);
        }

        // Reset the camera position
        Camera2D camera = GetNode<Camera2D>(CameraNodeName);

        camera.PositionSmoothingEnabled = false;
        camera.Position = new Vector2(0, 0);

        // Draw podiums
        DrawRectangleOfTiles(podiumX, podiumY, podiumWidth, podiumHeight);
        DrawRectangleOfTiles(podiumX - podiumWidth, podiumY, podiumWidth, podiumHeight - podiumHeightDifference);
        DrawRectangleOfTiles(podiumX + podiumWidth, podiumY, podiumWidth, podiumHeight - podiumHeightDifference * 2);

        // Place winners on podiums
        for (int i = 0; i < winners.Length; i++)
        {
            string userId = winners[i];
            Jumper jumper = ActiveJumpers.Instance.GetById(userId);
            int tileX = podiumX + (podiumWidth / 2);

            // This warning is only disabled due to a bug about false positives: https://github.com/SonarSource/sonar-dotnet/issues/8028
#pragma warning disable S2583 // Conditionally executed code should be reachable
            if (i == 1)
            {
                tileX -= podiumWidth;
            }
            else if (i == 2)
            {
                tileX += podiumWidth;
            }
#pragma warning restore S2583 // Conditionally executed code should be reachable

            // Make the winners much bigger
            int scale = Math.Max(2, 4 - i);

            jumper.Position = new Vector2(tileX * TileSetToUse.TileSize.X, 50);
            jumper.Scale = new Vector2(scale, scale);

            jumper.SetCrazyParticles();
        }
    }

    private void DrawRectangleOfTiles(int leftX, int bottomY, int width, int height)
    {
        for (int x = leftX; x < leftX + width; x++)
        {
            for (int y = bottomY; y > bottomY - height; y--)
            {
                _arenaBuilder.DrawPoint(x, y);
            }
        }
    }

    private void ShowEndScreen(string[] winners)
    {
        GetEndScreenOverlay().Visible = true;

        PanelContainer endScreen = GetEndScreenOverlay();
        StringBuilder text = new("Winners:");

        text.AppendLine();

        for (int i = 0; i < winners.Length; i++)
        {
            string userId = winners[i];
            PlayerData playerData = PlayerStats.Instance.GetPlayerById(userId);
            Jumper jumper = ActiveJumpers.Instance.GetById(userId);
            int height = ActiveJumpers.Instance.HeightToPosition(jumper.Position.Y);
            string totalHeight = Formatter.FormatBigNumber(playerData.TotalHeightAchieved);

            text.Append($"\t{i + 1}: {playerData.Name}. Height reached: {height}. ");
            text.Append($"Games played: {playerData.NumPlays}. ");
            text.Append(
                $"Wins: {playerData.Num1stPlaceWins}/{playerData.Num2ndPlaceWins}/{playerData.Num3rdPlaceWins}. "
            );
            text.Append($"Lifetime height: {totalHeight}");
            text.AppendLine();
        }

        text.AppendLine();
        text.Append($"Number of players this game: {ActiveJumpers.Instance.Count}");
        text.AppendLine().AppendLine();
        text.Append("YOU CAN NOW JUMP FREELY (until Adam gets back)!");

        endScreen.GetNode<Label>(EndScreenOutput).Text = text.ToString();
    }

    private void OnLobbyTimerDone()
    {
        _hasGameStarted = true;

        foreach (Jumper bot in _spawnedInstructionBots)
        {
            bot.QueueFree();
        }

        for (int x = 1; x < _widthInTiles - 1; x++)
        {
            _arenaBuilder.RemovePoint(x, _ceilingHeight);
        }

        GetLobbyOverlay().Visible = false;

        GameOverlay gameOverlay = GetGameOverlay();
        gameOverlay.Visible = true;
        gameOverlay.Init();
        _timerOverlay.Init();
    }

    private void OnRedemption(object sender, OnRewardRedeemedArgs e)
    {
        if (!e.RewardId.Equals(Guid.Parse("f04bb300-d135-4670-a7ba-1d6761590042")))
        {
            return;
        }

        GD.Print($"{e.DisplayName} is redeeming a revive!");

        CallDeferred(nameof(RedeemRevive), e.DisplayName);
    }

    private void RedeemRevive(string displayName)
    {
        if (_timerOverlay.TimerSeconds <= RevivePreventionCountdown)
        {
            return;
        }

        foreach (Jumper jumper in ActiveJumpers.Instance.AllJumpers())
        {
            if (!jumper.PlayerData.Name.Equals(displayName, StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }

            ReadOnlyCollection<Tuple<string, int>> playersByHeight = ActiveJumpers.Instance.SortJumpersByHeight();

            if (playersByHeight.Count <= 2)
            {
                break;
            }

            // Take user IDs of the top three players
            List<string> topPlayers = playersByHeight.Take(3).Select(p => p.Item1).ToList();

            // If the reviving player is already in the top 3, don't revive them
            if (topPlayers.Contains(jumper.PlayerData.UserId))
            {
                break;
            }

            string thirdHighestPlayerId = topPlayers[2];
            Jumper thirdHighestJumper = ActiveJumpers.Instance.GetById(thirdHighestPlayerId);

            GD.Print("Reviving " + displayName);
            GD.Print("Snapping to " + thirdHighestJumper.PlayerData.Name);

            jumper.Position = thirdHighestJumper.Position;
            jumper.Velocity = thirdHighestJumper.Velocity;

            jumper.FlashPlayerName();

            break;
        }
    }

    private void ModifyPlayerScales()
    {
        if (_hasGameEnded)
        {
            return;
        }

        foreach (Jumper jumper in ActiveJumpers.Instance.AllJumpers())
        {
            int height = ActiveJumpers.Instance.HeightToPosition(jumper.Position.Y);

            if (height > 0)
            {
                float scale = height / 5000f + 1;

                jumper.Scale = new Vector2(scale, scale);
            }
        }
    }

    private void MoveCamera()
    {
        Ensure.IsNotNull(TileSetToUse);

        if (ActiveJumpers.Instance.Count == 0 || _hasGameEnded)
        {
            return;
        }

        Jumper jumper = ActiveJumpers.Instance.GetHighestJumper();
        int lowestYValue = (int)jumper.Position.Y;
        int maxHeight = ActiveJumpers.Instance.HeightToPosition(lowestYValue);

        // Make sure the camera doesn't go higher than 0
        int tileHeight = TileSetToUse.TileSize.Y;

        lowestYValue = Math.Min(lowestYValue - tileHeight * 16, 0);

        Camera2D camera = GetNode<Camera2D>(CameraNodeName);

        camera.Position = new Vector2(0, lowestYValue);

        EmitSignal(SignalName.MaxHeightChanged, jumper.Name, maxHeight);
    }
}
