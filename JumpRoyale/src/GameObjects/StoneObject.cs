public record StoneObject : BaseObject
{
    public StoneObject()
        : base(
            GameObjectAtlasCoords.StoneBlock,
            GameObjectAtlasCoords.StonePlatformLeft,
            GameObjectAtlasCoords.StonePlatformMiddle,
            GameObjectAtlasCoords.StonePlatformRight
        )
    { }
}
