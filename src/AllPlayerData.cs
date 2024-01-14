using System.Collections.Generic;

public class AllPlayerData
{
#pragma warning disable // Exception, ignore warnings, because this has to match the existing json structure
    public Dictionary<string, PlayerData> players { get; set; } = new();
#pragma warning restore
}
