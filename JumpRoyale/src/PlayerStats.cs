using System.IO;
using System.Text.Json;

/// <summary>
/// Class responsible for loading Player Data from specified Json file (serialization and deserialization).
/// </summary>
public class PlayerStats
{
    private static readonly object _lock = new();

    private static PlayerStats? _instance;

    private PlayerStats() { }

    public static PlayerStats Instance
    {
        get
        {
            lock (_lock)
            {
                _instance ??= new PlayerStats();
            }

            return _instance;
        }
    }

    /// <summary>
    /// Gets currently deserialized Json data of all players.
    /// </summary>
    public AllPlayerData AllPlayerData { get; private set; } = new();

    /// <summary>
    /// Defines where the path for player stats is located.
    /// </summary>
    public string? StatsFilePath { get; set; }

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
    /// Attempts to read all the player data from Json file and stores it internally in <see cref="AllPlayerData"/>. If
    /// there was no data inside (new file or empty object), returns early with state. It will only throw an exception
    /// if the json was malformed or the structure didn't match the type.
    /// </summary>
    public bool LoadPlayerData()
    {
        if (!File.Exists(StatsFilePath))
        {
            return false;
        }

        string jsonString = File.ReadAllText(StatsFilePath);

        AllPlayerData? jsonResult = JsonSerializer.Deserialize<AllPlayerData>(jsonString);

        if (jsonResult is null)
        {
            return false;
        }

        // If there was data in the json file, but we didn't get anything out of it, it probably means that main
        // "players" property was changed, so we didn't match the required structure. Not sure what's the optimal length
        // to test, but probably longer than: "{}". Maybe it doesn't matter here at all.
        if (jsonResult.Players.Count == 0 && jsonString.Length > 4)
        {
            throw new InvalidJsonDataException();
        }

        AllPlayerData = jsonResult;

        return true;
    }

    /// <summary>
    /// Serializes all players and stores them in the Json file.
    /// </summary>
    public void SaveAllPlayers()
    {
        string jsonString = JsonSerializer.Serialize(AllPlayerData);

        if (StatsFilePath is null || StatsFilePath.Length == 0)
        {
            throw new MissingStatsFilePathException();
        }

        File.WriteAllText(StatsFilePath, jsonString);
    }

    /// <summary>
    /// Stores the player in the Players dictionary. Automatically keyed by the <c>userId</c> from provided
    /// <c>playerData</c>.
    /// </summary>
    /// <param name="playerData">Game data of the player to store and to get <c>userId</c> from.</param>
    /// <exception cref="NullPlayerDataException">When no player data was passed in.</exception>
    /// <exception cref="DuplicatePlayerException">When trying to add an already existing player.</exception>
    /// <exception cref="System.ArgumentException">Other exceptions related to Dictionary.</exception>
    public void StorePlayer(PlayerData? playerData)
    {
        if (playerData is null)
        {
            throw new NullPlayerDataException();
        }

        if (AllPlayerData.Players.ContainsKey(playerData.UserId))
        {
            throw new DuplicatePlayerException();
        }

        AllPlayerData.Players.Add(playerData.UserId, playerData);
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
}
