public record Brick : BaseObject
{
    public Brick()
        : base(
            GameObjectAtlasCoords.BrickBlock,
            GameObjectAtlasCoords.BrickPlatformLeft,
            GameObjectAtlasCoords.BrickPlatformMiddle,
            GameObjectAtlasCoords.BrickPlatformRight
        )
    {
        // Class intentionally left blank
    }
}
