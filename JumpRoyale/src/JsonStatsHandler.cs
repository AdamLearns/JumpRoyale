public class JsonStatsHandler(string filePath)
{
    /// <summary>
    /// Defines where the path for player stats is located.
    /// </summary>
    public string StatsFilePath { get; init; } = filePath;
}
