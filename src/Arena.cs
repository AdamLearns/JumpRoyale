using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using TwitchChat;
using TwitchLib.PubSub.Events;

public partial class Arena : Node2D
{
    [Export]
    public PackedScene JumperScene;

    [Export]
    public TileSet TileSetToUse;

    [Signal]
    public delegate void PlayerCountChangeEventHandler(int numPlayers);

    [Signal]
    public delegate void MaxHeightChangedEventHandler(string playerName, int height);

    [Signal]
    public delegate void CameraSpeedChangedEventHandler(int speed);

    private int _choice = 1;

    private bool _hasGameEnded = false;
    private long _timeSinceGameEnd = 0;

    private const string LobbyOverlayNodeName = "LobbyOverlay";
    private const string GameOverlayNodeName = "GameOverlay";
    private const string EndScreenOverlayNodeName = "EndScreenOverlay";
    private const string CameraNodeName = "Camera";
    private const string CanvasLayerNodeName = "CanvasLayer";
    private const string SaveLocation = "res://save_data/players.json";

    private const int WallHeight = 15; // in tiles
    private int _widthInTiles;
    private int _heightInTiles;
    private int _ceilingHeight;
    private TileMap _lobbyTilemap;

    private readonly Dictionary<string, Jumper> _jumpers = new Dictionary<string, Jumper>();

    private AllPlayerData _allPlayerData = new AllPlayerData();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TwitchChatClient twitchChatClient = new();
        twitchChatClient.OnRedemption += OnRedemption;
        twitchChatClient.OnMessage += OnMessage;

        GetLobbyOverlay().TimerDone += OnLobbyTimerDone;
        GetGameOverlay().TimerDone += OnGameTimerDone;

        SetBackground();

        GenerateWorld();

        LoadPlayerData();
    }

    private void SetBackground()
    {
        var background = GetNode<Sprite2D>("Background");
        var colors = new string[] { "Blue", "Brown", "Gray", "Green", "Pink", "Purple", "Yellow" };
        var color = colors[new RandomNumberGenerator().RandiRange(0, colors.Length - 1)];
        background.Texture = ResourceLoader.Load<Texture2D>($"res://assets/sprites/backgrounds/{color}.png");
    }

    private FlowContainer GetEndScreenOverlay()
    {
        return GetNode<CanvasLayer>(CanvasLayerNodeName).GetNode<FlowContainer>(EndScreenOverlayNodeName);
    }

    private GameOverlay GetGameOverlay()
    {
        return GetNode<CanvasLayer>(CanvasLayerNodeName).GetNode<GameOverlay>(GameOverlayNodeName);
    }

    private LobbyOverlay GetLobbyOverlay()
    {
        return GetNode<CanvasLayer>(CanvasLayerNodeName).GetNode<LobbyOverlay>(LobbyOverlayNodeName);
    }

    private void LoadPlayerData()
    {
        var filesystemLocation = ProjectSettings.GlobalizePath(SaveLocation);
        if (!File.Exists(filesystemLocation))
        {
            return;
        }

        var jsonString = File.ReadAllText(filesystemLocation);
        _allPlayerData = JsonSerializer.Deserialize<AllPlayerData>(jsonString);
    }

    private void SaveAllPlayers()
    {
        var filesystemLocation = ProjectSettings.GlobalizePath(SaveLocation);
        string jsonString = JsonSerializer.Serialize<AllPlayerData>(_allPlayerData);
        File.WriteAllText(filesystemLocation, jsonString);
    }

    private void GenerateWorld()
    {
        _lobbyTilemap = new TileMap { Name = "TileMap", TileSet = TileSetToUse };

        var viewport = GetViewportRect();
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
        _ceilingHeight = wallStartY - WallHeight - 1;

        // Draw the vertical walls
        for (int y = wallStartY; y >= _ceilingHeight; y--)
        {
            _lobbyTilemap.SetCell(0, new Vector2I(0, y), 0, new Vector2I(12, 1));
            _lobbyTilemap.SetCell(0, new Vector2I(_widthInTiles - 1, y), 0, new Vector2I(12, 1));
        }
        for (int y = wallStartY; y >= _ceilingHeight - 200; y--)
        {
            _lobbyTilemap.SetCell(0, new Vector2I(0, y), 0, new Vector2I(12, 1));
            _lobbyTilemap.SetCell(0, new Vector2I(_widthInTiles - 1, y), 0, new Vector2I(12, 1));
        }

        // Draw the ceiling
        for (int x = 1; x < _widthInTiles - 1; x++)
        {
            _lobbyTilemap.SetCell(0, new Vector2I(x, _ceilingHeight), 0, new Vector2I(12, 1));
        }

        // Add some platforms
        //
        // The easiest way to do this is to generate one platform per row of the
        // lobby area, then generate a platform of random width in that row, that
        // way there are no collisions.
        int platformStartY = wallStartY - 2;
        int platformEndY = _ceilingHeight + 4;
        for (int y = platformStartY; y >= platformEndY; y--)
        {
            RandomNumberGenerator rng = new RandomNumberGenerator();
            int width = rng.RandiRange(3, 15);
            int startX = rng.RandiRange(2, _widthInTiles - width - 2);
            AddPlatform(startX, y, width);
        }

        // Add platforms higher up
        int lowestY = -600;
        for (int y = _ceilingHeight - 2; y > lowestY; y--)
        {
            // This goes from 0 to 1 linearly as Y decreases
            float difficultyFactor = (float)Math.Min(0, y) / lowestY;

            // Rarely, make a solid block to add some variety
            RandomNumberGenerator rng = new RandomNumberGenerator();
            int r = rng.RandiRange(0, 100);
            if (r < 6 + difficultyFactor * 40)
            {
                int blockWidth = 2 + (int)(difficultyFactor * 24);
                int blockX = rng.RandiRange(2, _widthInTiles - 1 - blockWidth);
                DrawRectangleOfTiles(blockX, y + 1, blockWidth, blockWidth, new Vector2I(12, 1));
            }

            r = rng.RandiRange(0, 100);
            if (r > (70 - difficultyFactor * 60))
            {
                continue;
            }
            int width = rng.RandiRange(3, 15 - (int)Math.Round(6 * difficultyFactor));
            int startX = rng.RandiRange(2, _widthInTiles - width - 2);
            AddPlatform(startX, y, width);
        }

        AddChild(_lobbyTilemap);
    }

    private void AddPlatform(int x, int y, int width)
    {
        int endX = x + width - 1;

        // Draw left side
        _lobbyTilemap.SetCell(0, new Vector2I(x, y), 0, new Vector2I(17, 1));

        // Draw middle
        if (width > 2)
        {
            for (int i = x + 1; i < endX; i++)
            {
                _lobbyTilemap.SetCell(0, new Vector2I(i, y), 0, new Vector2I(18, 1));
            }
        }

        // Draw right side
        _lobbyTilemap.SetCell(0, new Vector2I(endX, y), 0, new Vector2I(19, 1));
    }

    private void OnGameTimerDone()
    {
        GetGameOverlay().Visible = false;

        _hasGameEnded = true;

        _timeSinceGameEnd = DateTime.Now.Ticks;

        var winners = ComputeStats();
        SaveAllPlayers();

        ShowEndScreen(winners);

        CreateEndArena(winners);
    }

    private void CreateEndArena(string[] winners)
    {
        // Draw border around the whole arena and clear any platforms
        for (int x = 0; x < _widthInTiles; x++)
        {
            for (int y = _heightInTiles; y > -_heightInTiles; y--)
            {
                if (x == 0 || x == _widthInTiles - 1 || y == _heightInTiles - 1)
                {
                    _lobbyTilemap.SetCell(0, new Vector2I(x, y), 0, new Vector2I(12, 1));
                }
                else
                {
                    _lobbyTilemap.SetCell(0, new Vector2I(x, y), -1);
                }
            }
        }

        var rng = new RandomNumberGenerator();
        var viewport = GetViewportRect();
        var xPadding = 100;

        // Put all players back in the arena
        for (int i = 0; i < _jumpers.Count; i++)
        {
            var jumper = _jumpers.ElementAt(i).Value;

            jumper.Position = new Vector2(
                rng.RandiRange(xPadding, (int)viewport.Size.X - xPadding),
                rng.RandiRange((int)(viewport.Size.Y / 2), (int)viewport.Size.Y - 100)
            );
            jumper.Scale = new Vector2(1, 1);
            jumper.Velocity = new Vector2(0, 0);
        }

        // Reset the camera position
        var camera = GetNode<Camera2D>(CameraNodeName);
        camera.PositionSmoothingEnabled = false;
        camera.Position = new Vector2(0, 0);

        // Draw podiums
        var numPodiums = 3;
        var podiumWidth = 6;
        var podiumHeight = 6; // has to be divisible by numPodiums
        var podiumHeightDifference = podiumHeight / numPodiums;
        var podiumX = _widthInTiles / 2;
        var podiumY = 13;

        DrawRectangleOfTiles(podiumX, podiumY, podiumWidth, podiumHeight, new Vector2I(12, 1));
        DrawRectangleOfTiles(
            podiumX - podiumWidth,
            podiumY + podiumHeightDifference,
            podiumWidth,
            podiumHeight - podiumHeightDifference,
            new Vector2I(12, 1)
        );
        DrawRectangleOfTiles(
            podiumX + podiumWidth,
            podiumY + podiumHeightDifference * 2,
            podiumWidth,
            podiumHeight - podiumHeightDifference * 2,
            new Vector2I(12, 1)
        );

        // Place winners on podiums
        for (int i = 0; i < winners.Length; i++)
        {
            var userId = winners[i];
            var jumper = _jumpers[userId];
            var tileX = podiumX + (podiumWidth / 2);
            if (i == 1)
            {
                tileX -= podiumWidth;
            }
            else if (i == 2)
            {
                tileX += podiumWidth;
            }
            jumper.Position = new Vector2(tileX * TileSetToUse.TileSize.X, 50);
            int scale = winners.Length + 1 - i;
            jumper.Scale = new Vector2(scale, scale);
            jumper.SetCrazyParticles();
        }

        // Add some platforms so that there are "fun" jumps to make
        for (int y = podiumY + podiumHeight + 15; y < _heightInTiles; y += 10)
        {
            int width = _widthInTiles / 3;
            int startX = _widthInTiles / 3;

            AddPlatform(startX, y, width);
        }
    }

    private void DrawRectangleOfTiles(int leftX, int topY, int width, int height, Vector2I tileIndex)
    {
        for (int x = leftX; x < leftX + width; x++)
        {
            for (int y = topY; y < topY + height; y++)
            {
                _lobbyTilemap.SetCell(0, new Vector2I(x, y), 0, tileIndex);
            }
        }
    }

    private void ShowEndScreen(string[] winners)
    {
        GetEndScreenOverlay().Visible = true;

        var endScreen = GetEndScreenOverlay();

        string text = "Winners:\n";
        for (int i = 0; i < winners.Length; i++)
        {
            var userId = winners[i];
            var playerData = _allPlayerData.players[userId];
            var jumper = _jumpers[userId];
            var height = GetHeightFromYPosition(jumper.Position.Y);
            var totalHeight = Formatter.FormatBigNumber(playerData.TotalHeightAchieved);
            text +=
                $"\t{i + 1}: {playerData.Name}. Height reached: {height}. Games played: {playerData.NumPlays}. Wins: {playerData.Num1stPlaceWins}/{playerData.Num2ndPlaceWins}/{playerData.Num3rdPlaceWins}. Lifetime height: {totalHeight}\n";
        }

        text += "\n";
        text += "Number of players this game: " + _jumpers.Count + "\n";
        text += "\n";
        text += "YOU CAN NOW JUMP FREELY (until Adam gets back)!\n";

        endScreen.GetNode<Label>("Output").Text = text;
    }

    private List<Tuple<string, int>> GetPlayersByHeight()
    {
        List<Tuple<string, int>> playersByHeight = _jumpers
            .OrderByDescending(o => GetHeightFromYPosition(o.Value.Position.Y))
            .Select(o => new Tuple<string, int>(o.Key, GetHeightFromYPosition((int)o.Value.Position.Y)))
            .ToList();
        return playersByHeight;
    }

    private string[] ComputeStats()
    {
        List<Tuple<string, int>> playersByHeight = GetPlayersByHeight();

        var winners = playersByHeight.Take(3).Select(p => p.Item1).ToArray();
        for (int i = 0; i < _jumpers.Count; i++)
        {
            var jumper = _jumpers.ElementAt(i).Value;
            var playerData = jumper.playerData;
            playerData.NumPlays++;
            playerData.TotalHeightAchieved += GetHeightFromYPosition(jumper.Position.Y);
            if (winners.Length > 0 && winners[0] == playerData.UserId)
            {
                playerData.Num1stPlaceWins++;
            }
            else if (winners.Length > 1 && winners[1] == playerData.UserId)
            {
                playerData.Num2ndPlaceWins++;
            }
            else if (winners.Length > 2 && winners[2] == playerData.UserId)
            {
                playerData.Num3rdPlaceWins++;
            }
        }

        return winners;
    }

    private void OnLobbyTimerDone()
    {
        for (int x = 1; x < _widthInTiles - 1; x++)
        {
            _lobbyTilemap.SetCell(0, new Vector2I(x, _ceilingHeight), -1);
        }
        GetLobbyOverlay().Visible = false;
        var gameOverlay = GetGameOverlay();
        gameOverlay.Visible = true;
        gameOverlay.init();
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
        foreach (var jumpersEntry in _jumpers)
        {
            var jumper = jumpersEntry.Value;
            if (jumper.playerData.Name.ToLower() == displayName.ToLower())
            {
                GD.Print("Reviving " + displayName);
                var playersByHeight = GetPlayersByHeight();
                if (playersByHeight.Count > 2)
                {
                    var thirdHighestPlayerId = playersByHeight[2].Item1;
                    var thirdHighestJumper = _jumpers[thirdHighestPlayerId];
                    GD.Print("Snapping to " + thirdHighestJumper.playerData.Name);

                    jumper.Position = thirdHighestJumper.Position;
                    jumper.Velocity = thirdHighestJumper.Velocity;
                }
                break;
            }
        }
    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        var lowercaseMessage = e.Message.ToLower();

        ChatCommandParser command = new(e.Message);

        string[] stringArguments = command.ArgumentsAsStrings();
        int?[] numericArguments = command.ArgumentsAsNumbers();

        if (lowercaseMessage.StartsWith("join"))
        {
            CallDeferred(nameof(AddPlayer), e.SenderId, e.SenderName, e.HexColor, e.IsPrivileged);
        }
        else if (lowercaseMessage.StartsWith("glow"))
        {
            CallDeferred(nameof(HandleGlow), e.SenderId, lowercaseMessage, e.HexColor, e.IsPrivileged);
        }
        else if (lowercaseMessage.StartsWith("unglow"))
        {
            CallDeferred(nameof(HandleUnglow), e.SenderId);
        }
        else if (lowercaseMessage.StartsWith("char"))
        {
            CallDeferred(nameof(HandleChangeCharacter), e.SenderId, lowercaseMessage);
        }
        else if (CommandAliasProvider.JumpCommandAliases.Any(alias => lowercaseMessage.StartsWith(alias)))
        {
            HandleJump(e.SenderId, command.Name, numericArguments[0], numericArguments[1]);
        }
    }

    private void HandleGlow(string senderId, string message, string hexColor, bool isPrivileged)
    {
        if (!isPrivileged)
        {
            return;
        }

        var userId = senderId;
        if (!_jumpers.ContainsKey(userId))
        {
            return;
        }

        Jumper jumper = _jumpers[userId];

        string[] parts = message.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 1)
        {
            jumper.SetGlow(hexColor);
        }
        else
        {
            jumper.SetGlow(parts[1]);
        }
    }

    private void HandleUnglow(string userId)
    {
        if (!_jumpers.ContainsKey(userId))
        {
            return;
        }

        Jumper jumper = _jumpers[userId];
        jumper.DisableGlow();
    }

    private void HandleChangeCharacter(string userId, string message)
    {
        if (!_jumpers.ContainsKey(userId))
        {
            return;
        }

        RandomNumberGenerator rng = new RandomNumberGenerator();
        int choice = rng.RandiRange(1, 18);

        string[] parts = message.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length > 1 && int.TryParse(parts[1], out int specifiedChoice))
        {
            choice = specifiedChoice;
        }

        choice = Math.Clamp(choice, 1, 18);

        Jumper jumper = _jumpers[userId];
        jumper.SetCharacter(choice);
    }

    private void HandleJump(string jumperId, string direction, int? angle, int? jumpPower)
    {
        /// We don't have to go through the logic if the sender does not exist on the jumpers list
        if (!_jumpers.ContainsKey(jumperId))
        {
            return;
        }

        if (!IsAllowedToJump())
        {
            return;
        }

        JumpCommand command = new(direction, angle, jumpPower);

        Jumper jumper = _jumpers[jumperId];

        jumper.CallDeferred(nameof(jumper.Jump), command.Angle, command.Power);
    }

    /// <summary>
    /// Players are allowed to jump only if the game is still running or if 5
    /// seconds have passed since the game ended (that way players don't jump
    /// off the podiums due to a stream delay)
    /// </summary>
    private bool IsAllowedToJump()
    {
        return _timeSinceGameEnd <= 0 || (DateTime.Now.Ticks - _timeSinceGameEnd) / TimeSpan.TicksPerMillisecond > 5000;
    }

    private void AddPlayer(string userId, string userName, string hexColor, bool isPrivileged)
    {
        if (_jumpers.ContainsKey(userId))
        {
            return;
        }

        RandomNumberGenerator rng = new RandomNumberGenerator();
        int randomCharacterChoice = rng.RandiRange(1, 18);

        var playerData = _allPlayerData.players.ContainsKey(userId)
            ? _allPlayerData.players[userId]
            : new PlayerData(hexColor, randomCharacterChoice);
        _allPlayerData.players[userId] = playerData;

        // Even if the player already existed, we may need to update their name.
        playerData.Name = userName;
        playerData.UserId = userId;

        if (!isPrivileged)
        {
            playerData.GlowColor = null;
        }

        Jumper jumper = JumperScene.Instantiate() as Jumper;
        int tileHeight = TileSetToUse.TileSize.Y;
        var viewport = GetViewportRect();
        int y = ((int)(viewport.Size.Y / tileHeight) - 1 - WallHeight) * tileHeight;
        int xPadding = TileSetToUse.TileSize.X * 3;
        jumper.Init(rng.RandiRange(xPadding, (int)viewport.Size.X - xPadding), y, userName, playerData);
        AddChild(jumper);

        _jumpers.Add(userId, jumper);

        EmitSignal(SignalName.PlayerCountChange, _jumpers.Count);
    }

    // Y decreases as you go up, so this converts it to a "height" property that
    // increases as you go up.
    //
    // Note that ideally, the height should return 0 when you're on the lowest
    // floor, but that's probably not the case at the time of writing.
    private int GetHeightFromYPosition(float y)
    {
        return (int)(-1 * y + GetViewportRect().Size.Y);
    }

    public override void _PhysicsProcess(double delta)
    {
        ModifyPlayerScales();

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
    }

    private void ModifyPlayerScales()
    {
        if (_hasGameEnded)
        {
            return;
        }
        for (int i = 0; i < _jumpers.Count; i++)
        {
            var jumper = _jumpers.ElementAt(i).Value;
            int height = GetHeightFromYPosition(jumper.Position.Y);
            if (height > 0)
            {
                var scale = height / 5000f + 1;
                jumper.Scale = new Vector2(scale, scale);
            }
        }
    }

    private void MoveCamera()
    {
        // Iterate over jumpers and check for the highest player
        if (_jumpers.Count == 0 || _hasGameEnded)
        {
            return;
        }

        int lowestYValue = 999999;
        string playerName = "";
        for (int i = 0; i < _jumpers.Count; i++)
        {
            var jumper = _jumpers.ElementAt(i).Value;
            if (jumper.Position.Y < lowestYValue)
            {
                lowestYValue = (int)jumper.Position.Y;
                playerName = jumper.Name;
            }
        }

        int maxHeight = GetHeightFromYPosition(lowestYValue);
        EmitSignal(SignalName.MaxHeightChanged, playerName, maxHeight);

        // Make sure the camera doesn't go higher than 0
        int tileHeight = TileSetToUse.TileSize.Y;
        lowestYValue = Math.Min(lowestYValue - tileHeight * 16, 0);

        var camera = GetNode<Camera2D>(CameraNodeName);
        camera.Position = new Vector2(0, lowestYValue);
    }
}
