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
    public string StatsFilePath { get; init; }

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
        // TODO: This currently throws an error if there were no players, which probably was not intended. If we are
        // running the game for the first time, we will have no players and it will throw an exception, never allowing
        // us to play the game. Change this to not throw on no players and only catch invalid json exception.
        // TODO: also, add tests for this.
        if (!File.Exists(StatsFilePath))
        {
            File.WriteAllText(StatsFilePath, "{}");
        }

        string jsonString = File.ReadAllText(StatsFilePath);

        AllPlayerData? jsonResult = JsonSerializer.Deserialize<AllPlayerData>(jsonString);

        // The following can only become null if the JSON input was literally `null`.
        if (jsonResult is null || jsonResult.Players.Count == 0)
        {
            throw new InvalidJsonDataException();
        }

        AllPlayerData = jsonResult;
    }
}
