using System.IO;
using System.Text.Json;

/// <summary>
/// Class responsible for loading Player Data from specified Json file (serialization and deserialization).
/// </summary>
public class PlayerStats
{
    public PlayerStats(string filePath)
    {
        StatsFilePath = filePath;

        LoadPlayerData();
    }

    /// <summary>
    /// Gets currently deserialized Json data of all players.
    /// </summary>
    public AllPlayerData AllPlayerData { get; private set; } = new();

    /// <summary>
    /// Defines where the path for player stats is located.
    /// </summary>
    public string StatsFilePath { get; }

    /// <summary>
    /// Returns PlayerData indexed by specified player id, if he exists in the dictionary loaded from Json file. Returns
    /// null if no player was found.
    /// </summary>
    /// <param name="playerId">Twitch User id.</param>
    public PlayerData? GetPlayerById(string playerId)
    {
        return AllPlayerData.Players.TryGetValue(playerId, out PlayerData? playerData) ? playerData : null;
    }

    /// <summary>
    /// Updates the indexed player with new player data.
    /// </summary>
    /// <remarks>
    /// This only updates the dictionary entry, not the record in Json file.
    /// </remarks>
    /// <param name="playerId">Twitch User id.</param>
    /// <param name="playerData">New player data.</param>
    public void UpdatePlayerById(string playerId, PlayerData playerData)
    {
        AllPlayerData.Players[playerId] = playerData;
    }

    /// <summary>
    /// Serializes all players and stores them in the Json file.
    /// </summary>
    public void SaveAllPlayers()
    {
        string jsonString = JsonSerializer.Serialize(AllPlayerData);

        File.WriteAllText(StatsFilePath, jsonString);
    }

    private void LoadPlayerData()
    {
        if (!File.Exists(StatsFilePath))
        {
            return;
        }

        string jsonString = File.ReadAllText(StatsFilePath);

        AllPlayerData? jsonResult = JsonSerializer.Deserialize<AllPlayerData>(jsonString);

        // We don't really need to do anything here, because later on serialization the file is rewritten anyway, so we
        // don't care if there were no results. This will just automatically throw exception if the json was invalid,
        // malformed or something changed within the structure that deserialization couldn't match.
        if (jsonResult is null)
        {
            return;
        }

        AllPlayerData = jsonResult;
    }
}
