public record Terracotta : BaseObject
{
    public Terracotta()
        : base(
            GameObjectAtlasCoords.TerracottaBlock,
            GameObjectAtlasCoords.TerracottaPlatformLeft,
            GameObjectAtlasCoords.TerracottaPlatformMiddle,
            GameObjectAtlasCoords.TerracottaPlatformRight
        )
    {
        // Class intentionally left blank
    }
}
