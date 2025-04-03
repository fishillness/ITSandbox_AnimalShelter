using UnityEngine;
using UnityEngine.Tilemaps;

public class WorkingWithGrid : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap field;
    [SerializeField] private Tilemap piecesSpawnersTilemap;
    [SerializeField] private Bound[] boundsElement;

    private BoundsInt bounds;
    protected int xDim;
    protected int yDim;

    protected virtual void SetBounds()
    {
        Vector3Int boundUp = field.WorldToCell(boundsElement[0].transform.position);
        Vector3Int boundBottom = field.WorldToCell(boundsElement[1].transform.position);

        bounds.xMin = boundUp.x;
        bounds.xMax = boundBottom.x;
        bounds.yMin = boundBottom.y;
        bounds.yMax = boundUp.y;
        xDim = bounds.xMax - bounds.xMin + 1;
        yDim = bounds.yMax - bounds.yMin + 1;
    }
    protected int GetXInGridPos(int x)
    {
        return (x + bounds.xMin);
    }

    protected int GetYInGridPos(int y)
    {
        return (bounds.yMax - y);
    }
    public Vector2 GetPiecePositionOnWorldInField(int x, int y)
    {
        Vector3 pos = field.CellToWorld(new Vector3Int(GetXInGridPos(x), GetYInGridPos(y), 0));
        pos.x += grid.cellSize.x / 2;
        pos.y += grid.cellSize.y / 2;

        return pos;
    }

    public Vector2 GetPiecePositionOnWorldInSpawners(int x, int y)
    {
        Vector3 pos = piecesSpawnersTilemap.CellToWorld(new Vector3Int(GetXInGridPos(x), GetYInGridPos(y), 0));
        pos.x += grid.cellSize.x / 2;
        pos.y += grid.cellSize.y / 2;

        return pos;
    }

    public bool IsTileInFieldNotEmpty(int x, int y)
    {
        return field.GetTile(new Vector3Int(GetXInGridPos(x), GetYInGridPos(y), 0)) != null;
    }

    public bool IsTileInSpawnersNotEmpty(int x, int y)
    {
        return piecesSpawnersTilemap.GetTile(new Vector3Int(GetXInGridPos(x), GetYInGridPos(y), 0)) != null;
    }
}
