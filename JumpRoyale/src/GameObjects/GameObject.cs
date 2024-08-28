using System.Collections.Generic;

public static class GameObject
{
    private static readonly Dictionary<GameObjectType, BaseObject> _gameObjects =
        new()
        {
            [GameObjectType.Stone] = new BaseObject(
                GameObjectAtlasCoords.StoneBlock,
                GameObjectAtlasCoords.StonePlatformLeft,
                GameObjectAtlasCoords.StonePlatformMiddle,
                GameObjectAtlasCoords.StonePlatformRight
            ),
            [GameObjectType.Concrete] = new BaseObject(
                GameObjectAtlasCoords.ConcreteBlock,
                GameObjectAtlasCoords.ConcretePlatformLeft,
                GameObjectAtlasCoords.ConcretePlatformMiddle,
                GameObjectAtlasCoords.ConcretePlatformRight
            ),
            [GameObjectType.Terracotta] = new BaseObject(
                GameObjectAtlasCoords.TerracottaBlock,
                GameObjectAtlasCoords.TerracottaPlatformLeft,
                GameObjectAtlasCoords.TerracottaPlatformMiddle,
                GameObjectAtlasCoords.TerracottaPlatformRight
            ),
            [GameObjectType.Brick] = new BaseObject(
                GameObjectAtlasCoords.BrickBlock,
                GameObjectAtlasCoords.BrickPlatformLeft,
                GameObjectAtlasCoords.BrickPlatformMiddle,
                GameObjectAtlasCoords.BrickPlatformRight
            ),
            [GameObjectType.Gold] = new BaseObject(
                GameObjectAtlasCoords.GoldBlock,
                GameObjectAtlasCoords.GoldPlatformLeft,
                GameObjectAtlasCoords.GoldPlatformMiddle,
                GameObjectAtlasCoords.GoldPlatformRight
            ),
        };

    public static BaseObject Get(GameObjectType type) => _gameObjects[type];
}
