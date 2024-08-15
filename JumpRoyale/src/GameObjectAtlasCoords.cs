using Godot;

public static class GameObjectAtlasCoords
{
    public static readonly Vector2I StoneBlock = new(12, 1);
    public static readonly Vector2I ConcreteBlock = new(12, 5);
    public static readonly Vector2I ClayBlock = new(12, 9);
    public static readonly Vector2I BrickBlock = new(20, 6);
    public static readonly Vector2I GoldBlock = new(17, 9);

    public static readonly Vector2I StonePlatformLeft = new(17, 1);
    public static readonly Vector2I StonePlatformMiddle = new(18, 1);
    public static readonly Vector2I StonePlatformRight = new(19, 1);

    public static readonly Vector2I ConcretePlatformLeft = new(17, 2);
    public static readonly Vector2I ConcretePlatformMiddle = new(18, 2);
    public static readonly Vector2I ConcretePlatformRight = new(19, 2);

    public static readonly Vector2I ClayPlatformLeft = new(13, 3);
    public static readonly Vector2I ClayPlatformMiddle = new(14, 3);
    public static readonly Vector2I ClayPlatformRight = new(15, 3);

    public static readonly Vector2I BrickPlatformLeft = new(17, 3);
    public static readonly Vector2I BrickPlatformMiddle = new(18, 3);
    public static readonly Vector2I BrickPlatformRight = new(19, 3);

    public static readonly Vector2I GoldPlatformLeft = new(17, 0);
    public static readonly Vector2I GoldPlatformMiddle = new(18, 0);
    public static readonly Vector2I GoldPlatformRight = new(19, 0);
}
