public record BrickObject : BaseObject
{
    public BrickObject()
        : base(
            GameObjectAtlasCoords.BrickBlock,
            GameObjectAtlasCoords.BrickPlatformLeft,
            GameObjectAtlasCoords.BrickPlatformMiddle,
            GameObjectAtlasCoords.BrickPlatformRight
        )
    { }
}
