public static partial class PlayerStatsMessages
{
    public const string DuplicatePlayerStoreError =
        "The player you are trying to store already exists in the dictionary. Did you mean to update instead?";

    public const string MissingPlayerStatsPath =
        $"Path to the stats file was not initialized. Make sure you set the path to stats file before loading players.";

    public const string NonExistentPlayer =
        "The player you are trying to update does not exist in the dictionary. Did you mean to store instead?";

    public const string NullPlayerData = "No PlayerData provided.";
}
