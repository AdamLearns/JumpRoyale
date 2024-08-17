public record GoldObject : BaseObject
{
    public GoldObject()
        : base(
            GameObjectAtlasCoords.GoldBlock,
            GameObjectAtlasCoords.GoldPlatformLeft,
            GameObjectAtlasCoords.GoldPlatformMiddle,
            GameObjectAtlasCoords.GoldPlatformRight
        )
    { }
}
