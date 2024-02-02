using System.Text.Json;
using Tests;

namespace Game;

[TestFixture]
public class PlayerStatsTests
{
    private readonly string _testFile = "data.json";

    private PlayerData _fakePlayer;

    /// <summary>
    /// Location of the Test directory, where the data will be read from/written to.
    /// <para>
    /// This evaluates to: <c>absolute_path\\Tests\\bin\\Debug\\net8.0\\_TestData\\</c>.
    /// </para>
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
        PlayerStats.Instance.StatsFilePath = FullPath;
        _fakePlayer = FakePlayerData.Make();

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

        PlayerStats.Instance.ClearPlayers();
    }

    /// <summary>
    /// This test checks if the player stat loading method returns false if the file does not exist. The purpose of this
    /// test is to make sure the test cleanup is done correctly (file should not exist on start) and the condition was
    /// not changed inside that class. We don't care if the file exists when loading or not.
    /// </summary>
    [Test]
    public void DoesNotThrowWhenFileNotExists()
    {
        Assert.DoesNotThrow(() =>
        {
            PlayerStats.Instance.LoadPlayerData();
        });
    }

    /// <summary>
    /// Note: We only care if the path is correct during the serialization.
    /// </summary>
    [Test]
    public void CanThrowIfPathWasUninitialized()
    {
        PlayerStats.Instance.StatsFilePath = null;

        Assert.Throws<MissingStatsFilePathException>(() =>
        {
            PlayerStats.Instance.SaveAllPlayers();
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

            state = PlayerStats.Instance.LoadPlayerData();
        });

        Assert.That(state, Is.False);
        state = false;

        Assert.DoesNotThrow(() =>
        {
            // Having an empty object is still allowed, it just evaluates to empty collection
            File.WriteAllText(FullPath, "{}");

            state = PlayerStats.Instance.LoadPlayerData();
        });

        Assert.That(state, Is.True);
    }

    /// <summary>
    /// This test checks if the player loading method can throw an appropriate exception when the json file contained
    /// actual data. We only need to deliberately change the main field name, we don't care if anything is inside.
    /// </summary>
    [Test]
    public void CanThrowOnMismatchedJsonStructure()
    {
        File.WriteAllText(FullPath, "{\"SomeStats\":{}}");

        Assert.Throws<InvalidJsonDataException>(() =>
        {
            PlayerStats.Instance.LoadPlayerData();
        });
    }

    [Test]
    public void CanThrowExceptionOnMalformedJson()
    {
        File.WriteAllText(FullPath, "{fdf,}}");

        Assert.Throws<JsonException>(() =>
        {
            PlayerStats.Instance.LoadPlayerData();
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
        PlayerStats.Instance.StorePlayer(_fakePlayer);
        PlayerStats.Instance.SaveAllPlayers();

        // Start on a fresh stats instance
        PlayerStats.Instance.ClearPlayers();
        PlayerStats.Instance.LoadPlayerData();

        PlayerData? playerFromFile = PlayerStats.Instance.GetPlayerById(_fakePlayer.UserId);

        if (playerFromFile is null)
        {
            Assert.Fail();
        }

        string fakeSerialized = JsonSerializer.Serialize(_fakePlayer);
        string playerFromFileSerialized = JsonSerializer.Serialize(playerFromFile);

        Assert.That(fakeSerialized, Is.EqualTo(playerFromFileSerialized));
    }

    /// <summary>
    /// This test just makes sure that we return the player we just saved through implemented methods.
    /// </summary>
    [Test]
    public void CanSetAndGetPlayerById()
    {
        string id = _fakePlayer.UserId;

        // Kind of simulate that we save the players, then in the next session we load them and at some point we have to
        // take the specific player.
        PlayerStats.Instance.StorePlayer(_fakePlayer);
        PlayerStats.Instance.SaveAllPlayers();

        // Clear the dictionary to make sure the players are being loaded correctly.
        PlayerStats.Instance.ClearPlayers();
        PlayerStats.Instance.LoadPlayerData();

        PlayerData? player = PlayerStats.Instance.GetPlayerById(id);

        Assert.That(player?.UserId, Is.EqualTo(id));
    }

    /// <summary>
    /// Just a sanity check.
    /// </summary>
    [Test]
    public void CanThrowWhenAddingDuplicatePlayer()
    {
        PlayerStats.Instance.StorePlayer(_fakePlayer);

        Assert.Throws<DuplicatePlayerException>(() =>
        {
            PlayerStats.Instance.StorePlayer(_fakePlayer);
        });
    }

    /// <summary>
    /// Just a sanity check. In reality, this should never happen, but maybe at some point the player was removed from
    /// the dictionary for whatever reason or by mistake(?).
    /// </summary>
    [Test]
    public void CanThrowWhenPassingNullPlayerData()
    {
        Assert.Throws<NullPlayerDataException>(() =>
        {
            PlayerStats.Instance.StorePlayer(null);
        });
    }

    /// <summary>
    /// This test just makes sure the method returns null. The purpose of a nullable return is to inform that we
    /// probably did something wrong when loading or inserting new players.
    /// </summary>
    [Test]
    public void CanThrowIfPlayerNotExists()
    {
        Assert.Throws<NullPlayerDataException>(() =>
        {
            PlayerStats.Instance.GetPlayerById("????");
        });
    }

    /// <summary>
    /// This test makes sure that we can correctly replace the player data under user id from that data.
    /// </summary>
    [Test]
    public void CanUpdatePlayers()
    {
        PlayerStats.Instance.StorePlayer(_fakePlayer);

        _fakePlayer.Name = "NewName";

        PlayerStats.Instance.UpdatePlayer(_fakePlayer);

        PlayerData player = PlayerStats.Instance.GetPlayerById(_fakePlayer.UserId);

        Assert.That(player.Name, Is.EqualTo("NewName"));
    }

    /// <summary>
    /// Just a sanity check.
    /// </summary>
    [Test]
    public void CanThrowWhenUpdatingFromNullData()
    {
        Assert.Throws<NullPlayerDataException>(() =>
        {
            PlayerStats.Instance.UpdatePlayer(null);
        });
    }

    /// <summary>
    /// Just a sanity check.
    /// </summary>
    [Test]
    public void CanThrowWhenUpdatingNonExistingPlayer()
    {
        Assert.Throws<NonExistentPlayerException>(() =>
        {
            PlayerStats.Instance.UpdatePlayer(FakePlayerData.Make());
        });
    }
}
