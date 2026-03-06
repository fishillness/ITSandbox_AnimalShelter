using UnityEngine;

public class ConstructionSystem : MonoBehaviour,
    IDependency<ValueManager>
{
    [SerializeField] private Store m_Store;
    [SerializeField] private ConstructionModeActivator m_ConstructionModeActivator;
    [SerializeField] private PlacementSystem m_PlacementSystem;
    [SerializeField] private AnimalManager m_AnimalManager;

    private ValueManager m_ShelterCharacteristicsManager;

    #region Constructs
    public void Construct(ValueManager m_ShelterCharacteristicsManager) => this.m_ShelterCharacteristicsManager = m_ShelterCharacteristicsManager;
    #endregion

    private void Awake()
    {
        m_Store.BuyEvent += StartConstruction;
        m_PlacementSystem.BuildingDeleteEvent += BuildingDelete;
        m_PlacementSystem.BuildingPlacementEvent += BuildingPlacement;
    }
    private void OnDestroy()
    {
        m_Store.BuyEvent -= StartConstruction;
        m_PlacementSystem.BuildingDeleteEvent -= BuildingDelete;
        m_PlacementSystem.BuildingPlacementEvent -= BuildingPlacement;
    }

    private void StartConstruction(BuildingInfo buildingInfo, BuildingColor buildingColor)
    {
        m_ConstructionModeActivator.ConstructionModeActivate();
        m_PlacementSystem.StartPlacement(buildingInfo.Building, buildingColor);
    }
    private void BuildingDelete(BuildingInfo buildingInfo)
    {
        m_Store.Refund(buildingInfo);
        m_ShelterCharacteristicsManager.DeleteShelterCharacteristics(buildingInfo.Advancement, buildingInfo.Cosiness, buildingInfo.Health, buildingInfo.Joy);
        m_AnimalManager.UpdateAnimalCount();
    }
    private void BuildingPlacement(BuildingInfo buildingInfo)
    {
        m_Store.Buy(buildingInfo);
        m_ShelterCharacteristicsManager.AddShelterCharacteristics(buildingInfo.Advancement, buildingInfo.Cosiness, buildingInfo.Health, buildingInfo.Joy);
        m_AnimalManager.UpdateAnimalCount();
    }
}
