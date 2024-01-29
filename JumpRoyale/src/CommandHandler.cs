using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

public class CommandHandler(string message, string senderId, string senderName, string hexColor, bool isPrivileged)
{
    private readonly string _message = message;
    private readonly string _senderId = senderId;
    private readonly string _senderName = senderName;
    private readonly string _hexColor = hexColor;
    private readonly bool _isPrivileged = isPrivileged;

    public delegate void Something(Jumper jumper);

    /// <summary>
    /// Gets the current instance of the executed command.
    /// </summary>
    [AllowNull]
    public ChatCommandParser ExecutedCommand { get; private set; }

    /// <summary>
    /// Reference to the Arena; has to be set at runtime.
    /// </summary>
    [AllowNull]
    public Arena Arena { get; set; }

    public void ParseMessage()
    {
        Something? command = GetExecutableCommand();

        if (command is null)
        {
            return;
        }

        // Small workaround to call Join before other commands, because we have to let it populate the Jumpers
        // dictionary with players
        if (ExecutedCommand.Name.Equals("join"))
        {
            // Automatically dispose unused object, this is only here because we need to match the delegate, even though
            // the jumper is not required in the Join command
            using (Jumper dummy = new())
            {
                command(dummy);
            }
        }

        if (!Arena.Jumpers.TryGetValue(_senderId, out Jumper? jumper))
        {
            return;
        }

        command(jumper);
    }

    public Something? GetExecutableCommand()
    {
        ExecutedCommand = new(_message.ToLower());

        string?[] stringArguments = ExecutedCommand.ArgumentsAsStrings();
        int?[] numericArguments = ExecutedCommand.ArgumentsAsNumbers();

        // Join is the only command that can be executed by everyone, whether joined or not.
        // All the remaining commands are only available to those who joined the game
        if (CommandMatcher.MatchesJoin(ExecutedCommand.Name))
        {
            return (jumper) => HandleJoin(_senderId, _senderName, _hexColor, _isPrivileged);
        }

        // Important: when working with aliases that collide with each other, remember to use the
        // proper order. E.g. Jump has `u` alias and if it was first on the list, it would
        // execute if `unglow` was sent in the chat, because we don't use exact matching
        return ExecutedCommand.Name switch
        {
            // -- Commands for all Chatters (active)
            string when CommandMatcher.MatchesUnglow(ExecutedCommand.Name) => HandleUnglow,
            string when CommandMatcher.MatchesJump(ExecutedCommand.Name)
                => (jumper) => HandleJump(jumper, ExecutedCommand.Name, numericArguments[0], numericArguments[1]),
            string when CommandMatcher.MatchesCharacterChange(ExecutedCommand.Name)
                => (jumper) => HandleCharacterChange(jumper, numericArguments[0]),

            // -- Commands for Mods, VIPs, Subs
            string when CommandMatcher.MatchesGlow(ExecutedCommand.Name, _isPrivileged)
                => (jumper) => HandleGlow(jumper, stringArguments[0], _hexColor),
            string when CommandMatcher.MatchesNamecolor(ExecutedCommand.Name, _isPrivileged)
                => (jumper) => HandleNamecolor(jumper, stringArguments[0]),
            _ => null,
        };
    }

    /// <summary>
    /// Streamer function, spawns a dummy on the arena.
    /// </summary>
    public void SpawnFakePlayers()
    {
        HandleJoin(_senderId, _senderName, _hexColor, _isPrivileged);
    }

    private void HandleJoin(string userId, string userName, string hexColor, bool isPrivileged)
    {
        Ensure.IsNotNull(Arena.JumperScene);
        Ensure.IsNotNull(Arena.TileSetToUse);

        if (Arena.Jumpers.ContainsKey(userId))
        {
            return;
        }

        int randomCharacterChoice = Rng.IntRange(1, 18);

        if (!Arena.AllPlayerData.Players.TryGetValue(userId, out PlayerData? playerData))
        {
            playerData = new(hexColor, randomCharacterChoice, Jumper.DefaultPlayerNameColor.ToHtml());
        }

        Arena.AllPlayerData.Players[userId] = playerData;

        // Even if the player already existed, we may need to update their name.
        playerData.Name = userName;
        playerData.UserId = userId;

        Jumper jumper = (Jumper)Arena.JumperScene.Instantiate();
        Rect2 viewport = Arena.GetViewportRect();
        int tileHeight = Arena.TileSetToUse.TileSize.Y;
        int xPadding = Arena.TileSetToUse.TileSize.X * 3;
        int x = Rng.IntRange(xPadding, (int)viewport.Size.X - xPadding);
        int y = ((int)(viewport.Size.Y / tileHeight) - 1 - Arena.WallHeightInTiles) * tileHeight;

        jumper.Init(x, y, userName, playerData);
        Arena.AddChild(jumper);

        // Note: the following block requires the jumper to be initialized before performing any changes,
        // either cosmetic or on the jumper himself, because we have to read the input from playerData,
        // which has to be sent to the jumper through .Init() first.
        jumper.SetCharacter(playerData.CharacterChoice);

        if (!isPrivileged)
        {
            // Reset the name with default color
            jumper.SetPlayerName();
            jumper.DisableGlow();
        }

        Arena.Jumpers.Add(userId, jumper);

        Arena.EmitSignal(Arena.SignalName.PlayerCountChange, Arena.Jumpers.Count);
    }

    private void HandleGlow(Jumper jumper, string? userHexColor, string twitchChatHexColor)
    {
        string? glowColor = userHexColor;

        // If just empty `glow` was sent, check if Glow existed in player data first, then fall back to Twitch color.
        // This prevents overriding the user choice with Twitch color when sending Unglow -> Glow.
        glowColor ??= jumper.PlayerData.GlowColor ?? twitchChatHexColor;

        jumper.SetGlow(glowColor);
    }

    private void HandleUnglow(Jumper jumper)
    {
        jumper.DisableGlow();
    }

    private void HandleCharacterChange(Jumper jumper, int? userChoice)
    {
        int choice = userChoice ?? Rng.IntRange(1, 18);

        choice = Math.Clamp(choice, 1, 18);

        jumper.SetCharacter(choice);
    }

    private void HandleNamecolor(Jumper jumper, string? nameColor)
    {
        string? color = nameColor;

        // We only want to pick a random color when requested by the user to not regenerate it if there was garbage
        // sent with the command or nothing at all to not give a false impression of the input having an
        // effect on the color, e.g. sending "znmxbc" and picking a random color for this
        if (color is not null && color.Equals("random", StringComparison.CurrentCultureIgnoreCase))
        {
            color = Rng.RandomHex();
        }

        // If the specified color was invalid (garbage message) or omitted, don't do anything
        // to not change the currently selected color
        if (!Color.HtmlIsValid(color) || color is null)
        {
            return;
        }

        jumper.PlayerData.NameColor = color;
        jumper.SetPlayerName();
        jumper.FlashPlayerName();
    }

    private void HandleJump(Jumper jumper, string direction, int? angle, int? jumpPower)
    {
        if (!Arena.IsAllowedToJump())
        {
            return;
        }

        JumpCommand command = new(direction, angle, jumpPower);

        jumper.Jump(command.Angle, command.Power);
        jumper.FlashPlayerName();
    }
}
