using System.Text.Json;

namespace Game;

[TestFixture]
public class PlayerStatsTests
{
    private readonly string _testFile = "data.json";
    private PlayerStats _playerStats;
    private PlayerData _fakePlayer;

    /// <summary>
    /// Location of the Test directory, where the data will be read/written to.
    /// </summary>
    private string TestPath
    {
        get => $"{TestContext.CurrentContext.WorkDirectory}\\_TestData\\";
    }

    /// <summary>
    /// Full path to the test file (path + file name).
    /// </summary>
    private string FullPath
    {
        get => TestPath + _testFile;
    }

    [SetUp]
    public void SetUp()
    {
        _playerStats = new(FullPath);
        _fakePlayer = new(Rng.RandomHex(), Rng.IntRange(1, 18), Rng.RandomHex()) { UserId = "1", };

        // Create the directory on fresh deploy or after project cleanup
        if (!Directory.Exists(TestPath))
        {
            Directory.CreateDirectory(TestPath);
        }
    }

    [TearDown]
    public void TearDown()
    {
        // Remove the stats file if it was created by any test.
        if (File.Exists(FullPath))
        {
            File.Delete(FullPath);
        }
    }

    /// <summary>
    /// This test checks if the player stat loading method returns false if the file does not exist. The purpose of this
    /// test is to make sure the cleanup is done correctly and the condition was not changed inside that class.
    /// </summary>
    [Test]
    public void DoesNotThrowWhenFileNotExists()
    {
        Assert.DoesNotThrow(() =>
        {
            _playerStats.LoadPlayerData();
        });
    }

    /// <summary>
    /// This test makes sure that we don't throw exceptions on allowed conditions of the json data being "null" or empty
    /// object, because the player data collection is still initialized as empty and later populated at runtime, when
    /// serializing players.
    /// </summary>
    [Test]
    public void DoesNotThrowWhenObjectIsEmptyOrNull()
    {
        bool state = false;

        Assert.DoesNotThrow(() =>
        {
            // Literal "null" can't be assigned to the AllPlayerData, so this should return early with false
            File.WriteAllText(FullPath, "null");

            state = _playerStats.LoadPlayerData();
        });

        Assert.That(state, Is.False);
        state = false;

        Assert.DoesNotThrow(() =>
        {
            // Having an empty object
            File.WriteAllText(FullPath, "{}");

            state = _playerStats.LoadPlayerData();
        });

        Assert.That(state, Is.True);
    }

    [Test]
    public void CanThrowExceptionOnMalformedJson()
    {
        File.WriteAllText(FullPath, "{fdf,}}");

        Assert.Throws<JsonException>(() =>
        {
            _playerStats.LoadPlayerData();
        });
    }

    /// <summary>
    /// This test makes sure that the player can be loaded properly after being saved into the file. Note: this does not
    /// need to be deeply equal, we are only comparing the json data.
    /// </summary>
    [Test]
    public void CanSerializePlayerThenDeserialize()
    {
        // We should start with an empty dictionary, so we will add a fake player, serialize him and then attempt to
        // read him from the file, then make sure the same data is loaded
        _playerStats.UpdatePlayerById(_fakePlayer.UserId, _fakePlayer);
        _playerStats.SaveAllPlayers();

        // Start on a fresh stats instance
        _playerStats = new(FullPath);
        _playerStats.LoadPlayerData();

        PlayerData? playerFromFile = _playerStats.GetPlayerById(_fakePlayer.UserId);

        if (playerFromFile is null)
        {
            Assert.Fail();
        }

        string fakeSerialized = JsonSerializer.Serialize(_fakePlayer);
        string playerFromFileSerialized = JsonSerializer.Serialize(playerFromFile);

        Assert.That(fakeSerialized, Is.EqualTo(playerFromFileSerialized));
    }
}
