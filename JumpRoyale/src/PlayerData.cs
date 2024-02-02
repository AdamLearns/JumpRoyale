using System.Text.Json.Serialization;

// Note to contributors: do not rename anything in this class without updating the corresponding JSON data on disk.
public class PlayerData(string glowColor, int characterChoice, string nameColor)
{
    public int Num1stPlaceWins { get; set; }

    public int Num2ndPlaceWins { get; set; }

    public int Num3rdPlaceWins { get; set; }

    public int NumPlays { get; set; }

    public int NumJumps { get; set; }

    public int TotalHeightAchieved { get; set; }

    // Looks like "#RRGGBB"
    public string GlowColor { get; set; } = glowColor;

    public int CharacterChoice { get; set; } = characterChoice;

    /// <summary>
    /// Current name color selected by the player. This will be used to automatically revert to default color if this
    /// player was unprivileged or to use it again when privileged so we don't overwrite it with default color.
    /// </summary>
    /// <remarks>
    /// This property is for Serialization purposes only and should not be used. If you need to return the color
    /// selected by players only if they are privileged, use <see cref="PlayerNameColor"/>.
    /// </remarks>
    public string NameColor { get; private set; } = nameColor ?? GameConstants.DefaultNameColor.ToHtml(false);

    // Note that this can change between games since you can change your name on
    // Twitch, so this is just for convenience of looking at the JSON file and
    // knowing who's who.
    public string Name { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets the current name color selected by the player, but will return default color if this player was
    /// unprivileged.
    /// </summary>
    /// <remarks>
    /// This property will not be serialized.
    /// </remarks>
    [JsonIgnore]
    public string PlayerNameColor
    {
        get => IsPrivileged ? NameColor : GameConstants.DefaultNameColor.ToHtml(false);
        set { NameColor = value; }
    }

    /// <summary>
    /// Defines this player's current <c>isPrivileged</c> status that was set upon joining the game. This property is
    /// mostly used by Join command to instantiate the player with previously customized privileged features, but it can
    /// also be used for commands, that are partially privileged, like extra cosmetics inside a command.
    /// <para>
    /// This property will not be a part of the Json data.
    /// </para>
    /// <para>
    /// Example: <c>char</c> command allows to select a character from given range, but some selections could only be
    /// applied to privileged users.
    /// </para>
    /// </summary>
    [JsonIgnore]
    public bool IsPrivileged { get; set; } = false;
}
