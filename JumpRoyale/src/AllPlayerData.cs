using System.Collections.Generic;
using System.Text.Json.Serialization;

public class AllPlayerData
{
    /// <summary>
    /// Dictionary of Players
    /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
    [JsonPropertyName("players")]
    public Dictionary<string, PlayerData> Players { get; set; } = [];
#pragma warning restore CA2227 // Collection properties should be read only
}
