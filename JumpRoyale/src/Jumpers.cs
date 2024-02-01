using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class Jumpers
{
    private static readonly object _lock = new();

    private static Jumpers? _instance;

    private readonly Dictionary<string, Jumper> _jumpers = [];

    private Jumpers() { }

    public static Jumpers Instance
    {
        get
        {
            lock (_lock)
            {
                _instance ??= new Jumpers();
            }

            return _instance;
        }
    }

    /// <summary>
    /// Amount of jumpers currently in the game.
    /// </summary>
    public int Count
    {
        get => _jumpers.Count;
    }

    public ReadOnlyCollection<Jumper> AllJumpers()
    {
        return new([.. _jumpers.Values]);
    }

    /// <summary>
    /// Adds a new jumper to the collection.
    /// </summary>
    /// <exception cref="ArgumentNullException">When <c>jumper</c> was null.</exception>
    public void AddJumper(Jumper jumper)
    {
        ArgumentNullException.ThrowIfNull(jumper);

        _jumpers.Add(jumper.PlayerData.UserId, jumper);
    }

    /// <summary>
    /// Takes all jumpers and increments their stats with the data from the current session. Winners will have their
    /// special properties recalculated too (1st/2nd/3rd places).
    /// </summary>
    public string[] ComputeStats()
    {
        ReadOnlyCollection<Tuple<string, int>> playersByHeight = SortJumpersByHeight();
        string[] winners = playersByHeight.Take(3).Select(p => p.Item1).ToArray();

        foreach (Jumper jumper in AllJumpers())
        {
            PlayerData playerData = jumper.PlayerData;
            bool showName = false;

            playerData.NumPlays++;
            playerData.TotalHeightAchieved += HeightToPosition(jumper.Position.Y);

            if (winners.Length > 0 && winners[0] == playerData.UserId)
            {
                playerData.Num1stPlaceWins++;
                showName = true;
            }
            else if (winners.Length > 1 && winners[1] == playerData.UserId)
            {
                playerData.Num2ndPlaceWins++;
                showName = true;
            }
            else if (winners.Length > 2 && winners[2] == playerData.UserId)
            {
                playerData.Num3rdPlaceWins++;
                showName = true;
            }

            if (showName)
            {
                jumper.DisableNameFadeout();
            }
        }

        return winners;
    }

    /// <summary>
    /// Returns true if jumper with the given id exists in the collection.
    /// </summary>
    public bool Exists(string userId)
    {
        return _jumpers.ContainsKey(userId);
    }

    /// <summary>
    /// Returns the jumper indexed by specified id.
    /// </summary>
    public Jumper GetById(string userId)
    {
        return _jumpers.TryGetValue(userId, out Jumper? jumper) ? jumper : throw new Exception();
    }

    /// <summary>
    /// Returns current top jumper.
    /// </summary>
    public Jumper GetHighestJumper()
    {
        return AllJumpers().MaxBy(jumper => jumper.Position.Y) ?? throw new Exception("No jumpers in the collection.");
    }

    /// <summary>
    /// Returns all jumpers sorted descending.
    /// </summary>
    public ReadOnlyCollection<Tuple<string, int>> SortJumpersByHeight()
    {
        return new(
            [
                .. _jumpers.OrderByDescending(o => HeightToPosition(o.Value.Position.Y))
                    .Select(o => new Tuple<string, int>(o.Key, HeightToPosition((int)o.Value.Position.Y)))
                    .ToList()
            ]
        );
    }

    /// <summary>
    /// Y decreases as you go up, so this converts it to a "height" property that
    /// increases as you go up.
    /// <para>
    ///  Note that ideally, the height should return 0 when you're on the lowest floor, but that's probably not the case
    ///  at the time of writing.
    /// </para>
    /// </summary>
    public int HeightToPosition(float y)
    {
        return (int)(-1 * y + Arena.ViewportHeight);
    }
}
