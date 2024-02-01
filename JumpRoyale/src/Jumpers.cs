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

    public bool Exists(string userId)
    {
        return _jumpers.ContainsKey(userId);
    }

    public Jumper GetById(string userId)
    {
        return _jumpers.TryGetValue(userId, out Jumper? jumper) ? jumper : throw new Exception();
    }

    public ReadOnlyCollection<Tuple<string, int>> SortJumpersByHeight()
    {
        return new(
            [
                .. _jumpers.OrderByDescending(o => GetHeightFromYPosition(o.Value.Position.Y))
                .Select(o => new Tuple<string, int>(o.Key, GetHeightFromYPosition((int)o.Value.Position.Y)))
                .ToList()
            ]
        );
    }

    // Y decreases as you go up, so this converts it to a "height" property that
    // increases as you go up.
    //
    // Note that ideally, the height should return 0 when you're on the lowest
    // floor, but that's probably not the case at the time of writing.
    private int GetHeightFromYPosition(float y)
    {
        return (int)(-1 * y);
    }
}
