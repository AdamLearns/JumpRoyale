using System.Collections.Generic;

public static class GameObject
{
    private static readonly Dictionary<GameObjectType, BaseObject> _gameObjects =
        new()
        {
            [GameObjectType.Stone] = new Stone(),
            [GameObjectType.Concrete] = new Concrete(),
            [GameObjectType.Terracotta] = new Terracotta(),
            [GameObjectType.Brick] = new Brick(),
            [GameObjectType.Gold] = new Gold(),
        };

    public static BaseObject Get(GameObjectType type) => _gameObjects[type];
}
