public record Stone : BaseObject
{
    public Stone()
        : base(
            GameObjectAtlasCoords.StoneBlock,
            GameObjectAtlasCoords.StonePlatformLeft,
            GameObjectAtlasCoords.StonePlatformMiddle,
            GameObjectAtlasCoords.StonePlatformRight
        )
    {
        // Class intentionally left blank
    }
}
