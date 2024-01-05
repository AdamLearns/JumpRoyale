public class PlayerData
{
    public int Num1stPlaceWins { get; set; }
    public int Num2ndPlaceWins { get; set; }
    public int Num3rdPlaceWins { get; set; }
    public int NumPlays { get; set; }
    public int NumJumps { get; set; }
    public int TotalHeightAchieved { get; set; }

    // Looks like "#RRGGBB"
    public string GlowColor { get; set; }
    public int CharacterChoice { get; set; }

    // Note that this can change between games since you can change your name on
    // Twitch, so this is just for convenience of looking at the JSON file and
    // knowing who's who.
    public string Name { get; set; }

    public string UserId { get; set; }

    public PlayerData(string glowColor, int characterChoice)
    {
        GlowColor = glowColor;
        CharacterChoice = characterChoice;
    }
}
