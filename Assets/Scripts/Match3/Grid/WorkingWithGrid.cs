using UnityEngine;
using UnityEngine.Tilemaps;

public class WorkingWithGrid : MonoBehaviour,
    IDependency<TilemapsDictionary>
{
    [SerializeField] private Grid grid;
    [SerializeField] private Bound[] boundsElement;

    private BoundsInt bounds;
    protected int xDim;
    protected int yDim;
    private TilemapsDictionary tilemapsDict;

    #region Constructs
    public void Construct(TilemapsDictionary tilemapsDict) => this.tilemapsDict = tilemapsDict;
    #endregion

    protected void InitTilemapsGrid()
    {
        tilemapsDict.InitDictionary();
    }

    protected void SetBounds()
    {
        Tilemap tilemap = tilemapsDict.GetPrefabByEnum(TilemapsType.Field).GetComponent<Tilemap>();
        Vector3Int boundUp = tilemap.WorldToCell(boundsElement[0].transform.position);
        Vector3Int boundBottom = tilemap.WorldToCell(boundsElement[1].transform.position);

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
    public Vector2 GetPiecePositionOnWorld(int x, int y, TilemapsType tilemapsType)
    {
        Tilemap tilemap = tilemapsDict.GetPrefabByEnum(tilemapsType).GetComponent<Tilemap>();
        Vector3 pos = tilemap.CellToWorld(new Vector3Int(GetXInGridPos(x), GetYInGridPos(y), 0));
        pos.x += grid.cellSize.x / 2;
        pos.y += grid.cellSize.y / 2;

        return pos;
    }

    public bool IsTileNotEmpty(int x, int y, TilemapsType tilemapsType)
    {
        Tilemap tilemap = tilemapsDict.GetPrefabByEnum(tilemapsType).GetComponent<Tilemap>();
        return tilemap.GetTile(new Vector3Int(GetXInGridPos(x), GetYInGridPos(y), 0)) != null;
    }
}
