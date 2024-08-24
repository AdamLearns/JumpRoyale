public record Gold : BaseObject
{
    public Gold()
        : base(
            GameObjectAtlasCoords.GoldBlock,
            GameObjectAtlasCoords.GoldPlatformLeft,
            GameObjectAtlasCoords.GoldPlatformMiddle,
            GameObjectAtlasCoords.GoldPlatformRight
        )
    {
        // Class intentionally left blank
    }
}
