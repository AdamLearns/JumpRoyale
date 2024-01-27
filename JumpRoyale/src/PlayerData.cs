using System.Runtime.Serialization;

// Note to contributors: do not rename anything in this class without updating the corresponding JSON data on disk.
public class PlayerData(string glowColor, int characterChoice, string nameColor, bool isPrivileged)
{
    private string _nameColor = nameColor;

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
    /// Current name color selected by the player. This will automatically revert to default color if this player was
    /// unprivileged.
    /// </summary>
    public string NameColor
    {
        get => IsPrivileged ? _nameColor : GameConstants.DefaultNameColor.ToHtml(false);
        set { _nameColor = value; }
    }

    // Note that this can change between games since you can change your name on
    // Twitch, so this is just for convenience of looking at the JSON file and
    // knowing who's who.
    public string Name { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Defines this player's current <c>isPrivileged</c> status that was set upon joining the game. This property is
    /// mostly used by Join command to instantiate the player with previously customized privileged features, but it can
    /// also be used for commands, that are partially privileged, like extra cosmetics inside a some command.
    ///
    /// <para>
    /// This property will not be a part of the Json data.
    /// </para>
    ///
    /// <para>
    /// Example: <c>char</c> command allows to select a character from given range, but can some selections can only be
    /// applied to privileged users.
    /// </para>
    /// </summary>
    [IgnoreDataMember]
    public bool IsPrivileged { get; private set; } = isPrivileged;
}
