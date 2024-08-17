public record ClayObject : BaseObject
{
    public ClayObject()
        : base(
            GameObjectAtlasCoords.ClayBlock,
            GameObjectAtlasCoords.ClayPlatformLeft,
            GameObjectAtlasCoords.ClayPlatformMiddle,
            GameObjectAtlasCoords.ClayPlatformRight
        )
    { }
}
