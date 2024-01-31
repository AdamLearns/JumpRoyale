public static partial class PlayerStatsMessages
{
    public const string DuplicatePlayerStoreError =
        "The player you are trying to store already exists in the dictionary. Did you mean to update instead?";

    public const string MissingPlayerStatsPath =
        $"Path to the stats file was not initialized. Make sure you set the path to stats file before loading players.";

    public const string NullPlayerData = "No PlayerData provided.";
}
