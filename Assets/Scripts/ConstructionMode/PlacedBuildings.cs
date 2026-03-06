using System;
using System.Collections.Generic;
using UnityEngine;


public class PlacedBuildings : MonoBehaviour 
{
    [Serializable]
    public class BuildingData   
    {
        public Vector2 OccupiedCell;
        public int BuildingID;
        public int BuildingIndex;
        public BuildingColor BuildingColor;
    }

    private List<Building> buildings = new List<Building>();
    private List<BuildingData> buildingsData = new List<BuildingData>();
    private List<Building> buildingsWithAnEntry = new List<Building>();
    private int buildingIndex;
    public void AddBuilding(Building building, BuildingColor buildingColor)
    {
        buildings.Add(building);
        AddBuildingInfo(building, buildingColor);               
    }

    public int GetBuildIndex()
    {
        buildingIndex++;
        return buildingIndex;
    }
    
    private void AddBuildingInfo(Building building, BuildingColor buildingColor)
    {        
        BuildingData buildingInfo = new BuildingData();
        buildingInfo.OccupiedCell = building.OccupiedCell;
        buildingInfo.BuildingID = building.BuildingID;
        buildingInfo.BuildingIndex = building.BuildingIndex;
        buildingInfo.BuildingColor = buildingColor;
        buildingsData.Add(buildingInfo);

        Saver<List<BuildingData>>.Save(SaverFilenames.PlacedBuilddingsFilaname, buildingsData);
    }
    public Building GetBuilding(int buildingIndex)
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].BuildingIndex == buildingIndex)
            {
                return buildings[i];
            }
        }
        return null;
    }
    public List<Building> GetABuildingsWithAnEntry()
    {
        buildingsWithAnEntry.Clear();
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].HasEntry == true)
            {
                buildingsWithAnEntry.Add(buildings[i]);
            }
        }
        return buildingsWithAnEntry;
    }
    public Building GetARandomBuildingWithAnEntry()
    {
        GetABuildingsWithAnEntry();
        return buildingsWithAnEntry[UnityEngine.Random.Range(0, buildingsWithAnEntry.Count)];
    }
    public void RemoveBuilding(int buildingIndex)
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].BuildingIndex == buildingIndex)
            {
                buildings.Remove(buildings[i]);
            }            
        }

        RemoveBuildingInfo(buildingIndex);
    }

    private void RemoveBuildingInfo(int buildingIndex)
    {
        for (int i = 0; i < buildingsData.Count; i++)
        {
            if (buildingsData[i].BuildingIndex == buildingIndex)
            {
                buildingsData.Remove(buildingsData[i]);
            }
        }

        Saver<List<BuildingData>>.Save(SaverFilenames.PlacedBuilddingsFilaname, buildingsData);
    }

    public List<BuildingData> LoadBuildingsData()
    {        
        List<BuildingData> uploadedBuildingsInfo = new List<BuildingData>();
        if (Saver<List<BuildingData>>.TryLoad(SaverFilenames.PlacedBuilddingsFilaname, ref uploadedBuildingsInfo) == true)
        {
            return uploadedBuildingsInfo;
        }
        else
        {
            return null;
        }          
    }    
}
