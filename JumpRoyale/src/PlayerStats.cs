using System.IO;
using System.Text.Json;

/// <summary>
/// Class responsible for loading Player Data from specified Json file (serialization and deserialization).
/// </summary>
public class PlayerStats
{
    private static readonly object _lock = new();
    private static PlayerStats? _instance;

    /// <summary>
    /// Gets currently deserialized Json data of all players.
    /// </summary>
    private readonly AllPlayerData _allPlayerData = new();

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
    /// Defines where the path for player stats is located.
    /// </summary>
    public string? StatsFilePath { get; set; }

    /// <summary>
    /// Clears the Players dictionary.
    /// </summary>
    public void ClearPlayers()
    {
        _allPlayerData.Players.Clear();
    }

    public bool Exists(string userId)
    {
        return _allPlayerData.Players.ContainsKey(userId);
    }

    /// <summary>
    /// Returns PlayerData indexed by specified player id, if he exists in the dictionary loaded from Json file.
    /// </summary>
    /// <param name="playerId">Twitch User id.</param>
    /// <exception cref="NullPlayerDataException">When no player was found under specified id.</exception>
    public PlayerData GetPlayerById(string playerId)
    {
        return _allPlayerData.Players.TryGetValue(playerId, out PlayerData? playerData)
            ? playerData
            : throw new NullPlayerDataException();
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

        _allPlayerData.Players = jsonResult.Players;

        return true;
    }

    /// <summary>
    /// Serializes all players and stores them in the Json file.
    /// </summary>
    public void SaveAllPlayers()
    {
        string jsonString = JsonSerializer.Serialize(_allPlayerData);

        if (StatsFilePath is null || StatsFilePath.Length == 0)
        {
            throw new MissingStatsFilePathException();
        }

        File.WriteAllText(StatsFilePath, jsonString);
    }

    /// <summary>
    /// Updates (or stores) the indexed player with new player data. Automatically keyed by the <c>userId</c> from
    /// provided <c>playerData</c>.
    /// <para>
    /// Note: This was merged with Store, which called <c>dictionary.Add()</c>, but that requires exception handling,
    /// but we can just directly access the index, which automatically overwrites the existing record. <c>UserId</b> is
    /// unique anyway, so there is no way to get a duplicate unless we spawn bots with hardcoded ids.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This only updates the dictionary entry, not the record in Json file, this is handled later during serialization.
    /// </remarks>
    /// <param name="playerData">New player data.</param>
    /// <exception cref="NullPlayerDataException">When no player data was passed in.</exception>
    public void UpdatePlayer(PlayerData? playerData)
    {
        if (playerData is null)
        {
            throw new NullPlayerDataException();
        }

        _allPlayerData.Players[playerData.UserId] = playerData;
    }
}
