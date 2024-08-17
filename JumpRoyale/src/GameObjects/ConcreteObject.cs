public record ConcreteObject : BaseObject
{
    public ConcreteObject()
        : base(
            GameObjectAtlasCoords.ConcreteBlock,
            GameObjectAtlasCoords.ConcretePlatformLeft,
            GameObjectAtlasCoords.ConcretePlatformMiddle,
            GameObjectAtlasCoords.ConcretePlatformRight
        )
    { }
}
