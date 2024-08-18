public record Concrete : BaseObject
{
    public Concrete()
        : base(
            GameObjectAtlasCoords.ConcreteBlock,
            GameObjectAtlasCoords.ConcretePlatformLeft,
            GameObjectAtlasCoords.ConcretePlatformMiddle,
            GameObjectAtlasCoords.ConcretePlatformRight
        )
    {
        // Class intentionally left blank
    }
}
