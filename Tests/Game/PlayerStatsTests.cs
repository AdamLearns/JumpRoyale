namespace Game;

[TestFixture]
public class PlayerStatsTests
{
    private readonly AllPlayerData _allPlayerData = new();
    private readonly string _testFile = "data.json";
    private PlayerStats? _playerStats;

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
        // Remove any previously loaded data
        _allPlayerData.Players.Clear();

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
            _playerStats = new(FullPath);

            Assert.That(_playerStats.LoadPlayerData(), Is.False);
        });
    }
}
