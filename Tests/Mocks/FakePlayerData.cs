namespace Tests;

public static class FakePlayerData
{
    public static PlayerData Make()
    {
        PlayerData fakePlayer =
            new(Rng.RandomHex(), Rng.RandomInt(), Rng.RandomHex())
            {
                UserId = Rng.RandomInt().ToString(),
                Num1stPlaceWins = Rng.RandomInt(),
                Num2ndPlaceWins = Rng.RandomInt(),
                Num3rdPlaceWins = Rng.RandomInt(),
                NumJumps = Rng.RandomInt(),
                NumPlays = Rng.RandomInt(),
                Name = Path.GetRandomFileName(), /* looks like: "zewrsrzg.rfp" */
            };

        return fakePlayer;
    }
}
