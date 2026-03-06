using UnityEngine;

public class Building : MonoBehaviour
{
    public Vector2 Size => m_Size;
    public int BuildingID => m_BuildingID;
    public Vector2 OccupiedCell => occupiedCell;
    public int BuildingIndex => buildingIndex;
    public bool HasEntry => m_EntryPoint != null;
    public BuildingColor BuildingColor => m_BuildingColor;

    [SerializeField] private VisualizationSystem m_VisualizationSystem;
    [SerializeField] private Vector2 m_Size;
    [SerializeField] private int m_BuildingID;
    [SerializeField] private Transform m_EntryPoint;
    [SerializeField] private SpriteRenderer m_BuildingSprite;
    [SerializeField] private BuildingColor m_BuildingColor;

    private int buildingIndex;
    private Vector2 occupiedCell;
    
    [ContextMenu("CellSpawnSpawn")]
    public void CellSpawnSpawn()
    {
        m_VisualizationSystem.CellSpawn(m_Size);
    }

    public void BuildingPlacement(Vector2 cell, int buildingIndex)
    {
        BuildingIsLocated();
        this.buildingIndex = buildingIndex;
        occupiedCell = cell;
    }
    public void ClearOccupiedCell()
    {
        occupiedCell = Vector2.zero;
    }

    public void ImpossibleToPlaceABuilding()
    {
        m_VisualizationSystem.ImpossibleToPlaceABuilding();
    }
    public void PossibleToPlaceABuilding()
    {
        m_VisualizationSystem.PossibleToPlaceABuilding();
    }
    private void BuildingIsLocated()
    {
        m_VisualizationSystem.BuildingIsLocated();
    }   

    public Transform GetEntryPoint()
    {
        return m_EntryPoint;
    }

    public void SetBuildingSprite(Sprite sprite)
    {
        m_BuildingSprite.sprite = sprite;
    }

    public void SetBuildingColor(BuildingColor color)
    {
        m_BuildingColor = color;
    }
}
