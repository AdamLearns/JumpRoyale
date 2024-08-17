using Godot;

public class ArenaBuilder(TileMap tileMap)
{
    private readonly TileMap _tileMap = tileMap;

    public void DrawPoint(int x, int y)
    {
        DrawAt(x, y, SelectObjectAtlasCoords(y));
    }

    public void RemovePoint(int x, int y)
    {
        RemoveAt(x, y);
    }

    private void RemoveAt(int x, int y)
    {
        _tileMap.SetCell(0, new Vector2I(x, y), -1);
    }

    private Vector2I SelectObjectAtlasCoords(int y)
    {
        return y switch
        {
            < -195 => GameObjectAtlasCoords.GoldBlock,
            < -130 => GameObjectAtlasCoords.BrickBlock,
            < -65 => GameObjectAtlasCoords.ClayBlock,
            < 0 => GameObjectAtlasCoords.ConcreteBlock,
            _ => GameObjectAtlasCoords.StoneBlock,
        };
    }

    private void DrawAt(int x, int y, Vector2I obj)
    {
        _tileMap.SetCell(0, new Vector2I(x, y), 0, obj);
    }
}
