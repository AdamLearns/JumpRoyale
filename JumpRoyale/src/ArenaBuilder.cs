using Godot;

public class ArenaBuilder(TileMap tileMap)
{
    private readonly TileMap _tileMap = tileMap;

    public void DrawPoint(int x, int y)
    {
        DrawAt(x, y);
    }

    private Vector2I SelectObjectAtlasCoords(int y)
    {
        return y switch
        {
            < 750 => GameObjectAtlasCoords.StoneBlock,
            < 1500 => GameObjectAtlasCoords.ConcreteBlock,
            < 2250 => GameObjectAtlasCoords.ClayBlock,
            < 3000 => GameObjectAtlasCoords.BrickBlock,
            < 9999 => GameObjectAtlasCoords.GoldBlock,
            _ => GameObjectAtlasCoords.StoneBlock,
        };
    }

    private void DrawAt(int x, int y)
    {
        _tileMap.SetCell(0, new Vector2I(x, y), 0, new Vector2I(6, 0));
    }
}
