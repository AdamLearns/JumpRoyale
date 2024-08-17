using Godot;

public class ArenaBuilder(TileMap tileMap)
{
    private readonly TileMap _tileMap = tileMap;

    public void DrawPlatform(int x, int y, int width)
    {
        BaseObject gameObject = GameObject.Get(GetObjectType(y));
        int endX = x + width - 1;

        // Draw left side of the platform
        DrawAt(x, y, gameObject.Left);

        // Draw middle if width > 2
        if (width > 2)
        {
            for (int i = x + 1; i < endX; i++)
            {
                DrawAt(i, y, gameObject.Middle);
            }
        }

        // Draw right side of the platform
        DrawAt(endX, y, gameObject.Right);
    }

    public void DrawPoint(int x, int y)
    {
        BaseObject gameObject = GameObject.Get(GetObjectType(y));

        DrawAt(x, y, gameObject.Point);
    }

    public void RemovePoint(int x, int y) => RemoveAt(x, y);

    private GameObjectType GetObjectType(int y)
    {
        return y switch
        {
            < -195 => GameObjectType.Gold,
            < -130 => GameObjectType.Brick,
            < -65 => GameObjectType.Clay,
            < 0 => GameObjectType.Concrete,
            _ => GameObjectType.Stone,
        };
    }

    private void DrawAt(int x, int y, Vector2I obj)
    {
        _tileMap.SetCell(0, new Vector2I(x, y), 0, obj);
    }

    private void RemoveAt(int x, int y)
    {
        _tileMap.SetCell(0, new Vector2I(x, y), -1);
    }
}
