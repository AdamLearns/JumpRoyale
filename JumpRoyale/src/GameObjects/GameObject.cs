using System.Collections.Generic;

public static class GameObject
{
    private static readonly Dictionary<GameObjectType, BaseObject> _gameObjects =
        new()
        {
            [GameObjectType.Stone] = new StoneObject(),
            [GameObjectType.Concrete] = new ConcreteObject(),
            [GameObjectType.Clay] = new ClayObject(),
            [GameObjectType.Brick] = new BrickObject(),
            [GameObjectType.Gold] = new GoldObject(),
        };

    public static BaseObject Get(GameObjectType type) => _gameObjects[type];
}
