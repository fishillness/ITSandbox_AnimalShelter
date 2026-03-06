using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;
using static PlacedBuildings;

public class PlacementSystem : MonoBehaviour
{   
    public event UnityAction<BuildingInfo> BuildingDeleteEvent;
    public event UnityAction<BuildingInfo> BuildingPlacementEvent;

    [SerializeField] private BuildingDataBase m_BuildingDataBase;
    [SerializeField] private ConstructionModeUI m_ConstructionModeUI;
    [SerializeField] private Indicator m_Indicator;
    [SerializeField] private ConstructionGrid m_Grid;
    [SerializeField] private RealtimeNavMesh m_RealtimeNavmesh;
    [SerializeField] private PlacedBuildings m_PlacedBuildings;

    private enum PlacemenSystemMode
    {
        Placement,
        Permutation,
        CheckingCells
    }

    
    private Building currentBuilding;
    private Vector2 currentCellLocalPosition;
    private PlacemenSystemMode placemenSystemMode;

    private void Awake()
    {
        placemenSystemMode = PlacemenSystemMode.CheckingCells;
        m_Indicator.CellSelected += OnCellSelected;
        CreatingUploadedBuildings();                
    }
    private void OnDestroy()
    {
        m_Indicator.CellSelected -= OnCellSelected;
    }    
    
    
    private void CreatingUploadedBuildings()
    {
        List<BuildingData> uploadedBuildingsInfo = m_PlacedBuildings.LoadBuildingsData();
        if (uploadedBuildingsInfo == null) return;

        for (int i = 0; i < uploadedBuildingsInfo.Count; i++)
        {
            Building building = Instantiate(m_BuildingDataBase.GetBuilding(uploadedBuildingsInfo[i].BuildingID));
            building.BuildingPlacement(uploadedBuildingsInfo[i].OccupiedCell, m_PlacedBuildings.GetBuildIndex());
            m_PlacedBuildings.AddBuilding(building, uploadedBuildingsInfo[i].BuildingColor);
            building.transform.position = m_Grid.ConvertCellLocalPositionToCellWorldPosition(building.OccupiedCell);
            BuildingColor color = uploadedBuildingsInfo[i].BuildingColor;
            building.SetBuildingColor(color);
            Sprite sprite = m_BuildingDataBase.GetBuildingInfo(building.BuildingID)
                                    .ColorSprites.Find(x => x.Color == color).Sprite;
            building.SetBuildingSprite(sprite);
            for (int x = 0; x < building.Size.x; x++)
            {
                for (int y = 0; y < building.Size.y; y++)
                {
                    Vector2 cellPosition = new Vector2(building.OccupiedCell.x + x, building.OccupiedCell.y + y);
                    m_Grid.OccupyACell(cellPosition, building.BuildingIndex);                    
                }
            }           
            
        }
        m_RealtimeNavmesh.UpdateNavMesh();
    }
    
    private void OnCellSelected(Vector2 cellLocalPosition)
    {
        currentCellLocalPosition = cellLocalPosition;

        if (placemenSystemMode == PlacemenSystemMode.CheckingCells)
        {
            CheckingCellOccupancy();               
        }
        else
        {
            SetBuildingPosition();
            CheckingPossibilityOfBuildingPlacement();
        }
    }

    private void CheckingCellOccupancy()
    {
        if (m_Grid.CheckingCellOccupancy(currentCellLocalPosition) == true)
        {
            Building building = m_PlacedBuildings.GetBuilding(m_Grid.GetCell(currentCellLocalPosition).BuildingIndex);            
            m_Indicator.BuildingSelect(building.Size, building.OccupiedCell);
            m_ConstructionModeUI.EnablingPermutation();
        }
        else
        {
            m_ConstructionModeUI.DisablingPermutation();
            m_Indicator.BuildingUnselect();
        }
    }

    private void CheckingPossibilityOfBuildingPlacement()
    {
        if (currentBuilding == null) return;   
        
        for (int x = 0; x < currentBuilding.Size.x; x++)
        {
            for (int y = 0; y < currentBuilding.Size.y; y++)
            {
                if (m_Grid.CheckingCellOccupancy(new Vector2(currentCellLocalPosition.x + x, currentCellLocalPosition.y + y)) == true)
                {
                    DisablingPlacement();
                    return;
                }
            }
        }
        EnablingPlacement();
    }

    public void StartPlacement(Building building, BuildingColor color)
    {
        placemenSystemMode = PlacemenSystemMode.Placement;

        if (currentBuilding != null)
        {
            Destroy(currentBuilding.gameObject);
        }

        currentBuilding = Instantiate(building);
        currentBuilding.SetBuildingColor(color);
        Sprite sprite = m_BuildingDataBase.GetBuildingInfo(currentBuilding.BuildingID)
                                    .ColorSprites.Find(x => x.Color == color).Sprite;
        currentBuilding.SetBuildingSprite(sprite);
        currentBuilding.transform.position = m_Indicator.transform.position;
        m_Indicator.IndicatorVisualization(false);
        m_ConstructionModeUI.StartPlacement();
        CheckingPossibilityOfBuildingPlacement();
    }

    private void DisablingPlacement()
    {
        currentBuilding.ImpossibleToPlaceABuilding();
        m_ConstructionModeUI.DisablingPlacement();
    }

    private void EnablingPlacement()
    {
        currentBuilding.PossibleToPlaceABuilding();
        m_ConstructionModeUI.EnablingPlacement();
    }    

    public void BuildingPlacement()
    {        
        int buildIndex = m_PlacedBuildings.GetBuildIndex();
        currentBuilding.BuildingPlacement(currentCellLocalPosition, buildIndex);
        m_PlacedBuildings.AddBuilding(currentBuilding,  currentBuilding.BuildingColor);
        for (int x = 0; x < currentBuilding.Size.x; x++)
        {
            for (int y = 0; y < currentBuilding.Size.y; y++)
            {
                Vector2 cellPosition = new Vector2(currentCellLocalPosition.x + x, currentCellLocalPosition.y + y);
                m_Grid.OccupyACell(cellPosition, buildIndex);                
            }
        }

        if (placemenSystemMode == PlacemenSystemMode.Placement)
        {
            BuildingPlacementEvent?.Invoke(m_BuildingDataBase.GetBuildingInfo(currentBuilding.BuildingID));
        }

        EndPlacement();
    }

    public void CancellationBuildingPlacement()
    {        
        Destroy(currentBuilding.gameObject);    
        EndPlacement();
    }

    private void EndPlacement()
    {
        m_RealtimeNavmesh.UpdateNavMesh();
        m_Indicator.IndicatorVisualization(true);        
        m_ConstructionModeUI.EndPlacement();
        currentBuilding = null;
        placemenSystemMode = PlacemenSystemMode.CheckingCells;
        CheckingCellOccupancy();
    }      

    

    public void StartPermutation()
    {
        m_Indicator.IndicatorVisualization(false);
        m_ConstructionModeUI.DisablingPermutation();
        GridCleaning();
        placemenSystemMode = PlacemenSystemMode.Permutation;
        SetBuildingPosition();
        CheckingPossibilityOfBuildingPlacement();
    }
    public void BuildingDelete()
    {
        m_ConstructionModeUI.DisablingPermutation();
        GridCleaning();
        BuildingDeleteEvent?.Invoke(m_BuildingDataBase.GetBuildingInfo(currentBuilding.BuildingID));
        DestroyImmediate(currentBuilding.gameObject);
        currentBuilding = null;
        m_Indicator.BuildingUnselect();
        m_RealtimeNavmesh.UpdateNavMesh();
    }

    private void GridCleaning()
    {
        int buildingIndex = m_Grid.GetCell(currentCellLocalPosition).BuildingIndex;
        currentBuilding = m_PlacedBuildings.GetBuilding(buildingIndex);
        for (int i = 0; i < currentBuilding.Size.x; i++)
        {
            for (int j = 0; j < currentBuilding.Size.y; j++)
            {
                m_Grid.ClearACell(currentBuilding.OccupiedCell + new Vector2(i,j));
            }
            
        }
        currentBuilding.ClearOccupiedCell();
        m_PlacedBuildings.RemoveBuilding(buildingIndex);
    }
     
    private void SetBuildingPosition()
    {
        if (currentBuilding == null) return;

        currentBuilding.transform.position = m_Indicator.transform.position;     
    }
}
