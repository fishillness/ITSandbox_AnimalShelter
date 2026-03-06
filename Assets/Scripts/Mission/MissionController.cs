using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MissionController : MonoBehaviour, 
    IDependency<ValueManager>, IDependency<Match3LevelManager>
{
    [HideInInspector]
    public UnityEvent<Mission> OnMissionAdd;
    [HideInInspector]
    public UnityEvent<Mission> OnMissionUpdate;
    [HideInInspector]
    public UnityEvent OnMissionEnd;

    [Serializable]
    public class MissionData
    {
        public int id;
        public int value;
        public bool isFinish;

        public MissionData(int id, int value, bool isFinish)
        {
            this.id = id;
            this.value = value;
            this.isFinish = isFinish;
        }
    }

    [SerializeField] private MissionList missionList;

    private ValueManager valueManager;
    private Match3LevelManager levelManager;

    private List<Mission> currentMissions = new List<Mission>();
    private List<MissionData> currentMissionData = new List<MissionData>();

    #region Constructs
    public void Construct(ValueManager valueManager) => this.valueManager = valueManager;
    public void Construct(Match3LevelManager levelManager) => this.levelManager = levelManager;
    #endregion

    public List<Mission> GetAllCurrentMissions => currentMissions;

    private void Awake()
    {
        LoadData();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void Start()
    {
        levelManager.OnLevelUp.AddListener(OnLevelUp);

        valueManager.GetEventOnValueAddByType(ValueType.Coins).AddListener(OnCoinsAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Boards).AddListener(OnBoardsAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Bricks).AddListener(OnBricksAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Nails).AddListener(OnNailsAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Energy).AddListener(OnEnergyAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Advancement).AddListener(OnAdvancementAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Cosiness).AddListener(OnCosinessAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Health).AddListener(OnHealthAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Joy).AddListener(OnJoyAdd);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;

        levelManager.OnLevelUp.RemoveListener(OnLevelUp);

        valueManager.GetEventOnValueAddByType(ValueType.Coins).RemoveListener(OnCoinsAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Boards).RemoveListener(OnBoardsAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Bricks).RemoveListener(OnBricksAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Nails).RemoveListener(OnNailsAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Energy).RemoveListener(OnEnergyAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Advancement).RemoveListener(OnAdvancementAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Cosiness).RemoveListener(OnCosinessAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Health).RemoveListener(OnHealthAdd);
        valueManager.GetEventOnValueAddByType(ValueType.Joy).RemoveListener(OnJoyAdd);
    }

    public void AddNewMission(MissionInfo missionInfo)
    {
        foreach(MissionData data in currentMissionData)
        {
            if (data.id == missionInfo.Id)
            {
                Debug.Log("Миссия уже есть в списке");
                return;
            }
        }

        Mission mission = new Mission(missionInfo, 0, false);
        MissionData missionData = new MissionData(missionInfo.Id, 0, false);

        currentMissions.Add(mission);
        currentMissionData.Add(missionData);

        OnMissionAdd?.Invoke(mission);

        Saver<List<MissionData>>.Save(SaverFilenames.MissionsFilename, currentMissionData);
    }

    public void OnMissionFinishClick(int id)
    {
        foreach (Mission mission in currentMissions)
        {
            if (mission.missionInfo.Id == id)
            { 
                if (mission.isFinish == false)
                {
                    Debug.Log("Миссия не была готова к завершению");
                    return;
                }

                currentMissions.Remove(mission);
                break;
            }
        }
        foreach (MissionData mission in currentMissionData)
        {
            if (mission.id == id)
            {
                currentMissionData.Remove(mission);
                break;
            }
        }

        Saver<List<MissionData>>.Save(SaverFilenames.MissionsFilename, currentMissionData);
        OnMissionEnd?.Invoke();
    }

    private void LoadData()
    {
        List<MissionData> uploadedMissionData = new List<MissionData>();

        if (Saver<List<MissionData>>.TryLoad(SaverFilenames.MissionsFilename, ref uploadedMissionData) == true)
        {
            foreach (MissionData data in uploadedMissionData)
            {
                currentMissionData.Add(data);
                Mission mission = new Mission(missionList.GetMissionInfoById(data.id), data.value, data.isFinish);
                currentMissions.Add(mission);
            }
        }
        else
        {
            Debug.Log("Сохранений миссий нет");
        }
    }

    private void ChangeMissionDataById(int id, int value, bool isFinish)
    {
        foreach(MissionData mission in currentMissionData)
        {
            if (mission.id == id)
            {
                mission.value = value;
                mission.isFinish = isFinish;
            }
        }

        Saver<List<MissionData>>.Save(SaverFilenames.MissionsFilename, currentMissionData);
    }

    #region OnEvent

    private void OnLevelUp(int level)
    {
        foreach (Mission mission in currentMissions)
        {
            if (mission.missionInfo.Type == MissionType.CompleteMatch3)
            {
                mission.value = 1;
                mission.isFinish = true;
                ChangeMissionDataById(mission.missionInfo.Id, mission.value, mission.isFinish);

                OnMissionUpdate?.Invoke(mission);
            }
        }
    }

    private void OnCoinsAdd(int value)
    {
        foreach (Mission mission in currentMissions)
        {
            if (mission.missionInfo.Type == MissionType.GetResource && mission.missionInfo.ValueType == ValueType.Coins)
            { 
                mission.value += value;
                if (mission.value >= mission.missionInfo.NeedGetValue)
                    mission.isFinish = true;

                ChangeMissionDataById(mission.missionInfo.Id, mission.value, mission.isFinish);

                OnMissionUpdate?.Invoke(mission);
            }
        }
    }

    private void OnBoardsAdd(int value)
    {
        foreach (Mission mission in currentMissions)
        {
            if (mission.missionInfo.Type == MissionType.GetResource && mission.missionInfo.ValueType == ValueType.Boards)
            {
                mission.value += value;
                if (mission.value >= mission.missionInfo.NeedGetValue)
                    mission.isFinish = true;

                ChangeMissionDataById(mission.missionInfo.Id, mission.value, mission.isFinish);

                OnMissionUpdate?.Invoke(mission);
            }
        }
    }

    private void OnBricksAdd(int value)
    {
        foreach (Mission mission in currentMissions)
        {
            if (mission.missionInfo.Type == MissionType.GetResource && mission.missionInfo.ValueType == ValueType.Bricks)
            {
                mission.value += value;
                if (mission.value >= mission.missionInfo.NeedGetValue)
                    mission.isFinish = true;

                ChangeMissionDataById(mission.missionInfo.Id, mission.value, mission.isFinish);

                OnMissionUpdate?.Invoke(mission);

            }
        }
    }

    private void OnNailsAdd(int value)
    {
        foreach (Mission mission in currentMissions)
        {
            if (mission.missionInfo.Type == MissionType.GetResource && mission.missionInfo.ValueType == ValueType.Nails)
            {
                mission.value += value;
                if (mission.value >= mission.missionInfo.NeedGetValue)
                    mission.isFinish = true;

                ChangeMissionDataById(mission.missionInfo.Id, mission.value, mission.isFinish);

                OnMissionUpdate?.Invoke(mission);
            }
        }
    }

    private void OnEnergyAdd(int value)
    {
        foreach (Mission mission in currentMissions)
        {
            if (mission.missionInfo.Type == MissionType.GetResource && mission.missionInfo.ValueType == ValueType.Energy)
            {
                mission.value += value;
                if (mission.value >= mission.missionInfo.NeedGetValue)
                    mission.isFinish = true;

                ChangeMissionDataById(mission.missionInfo.Id, mission.value, mission.isFinish);

                OnMissionUpdate?.Invoke(mission);
            }
        }
    }

    private void OnAdvancementAdd(int value)
    {
        foreach (Mission mission in currentMissions)
        {
            if (mission.missionInfo.Type == MissionType.GetResource && mission.missionInfo.ValueType == ValueType.Advancement)
            {
                mission.value += value;
                if (mission.value >= mission.missionInfo.NeedGetValue)
                    mission.isFinish = true;

                ChangeMissionDataById(mission.missionInfo.Id, mission.value, mission.isFinish);

                OnMissionUpdate?.Invoke(mission);
            }
        }
    }

    private void OnCosinessAdd(int value)
    {
        foreach (Mission mission in currentMissions)
        {
            if (mission.missionInfo.Type == MissionType.GetResource && mission.missionInfo.ValueType == ValueType.Cosiness)
            {
                mission.value += value;
                if (mission.value >= mission.missionInfo.NeedGetValue)
                    mission.isFinish = true;

                ChangeMissionDataById(mission.missionInfo.Id, mission.value, mission.isFinish);

                OnMissionUpdate?.Invoke(mission);
            }
        }
    }

    private void OnHealthAdd(int value)
    {
        foreach (Mission mission in currentMissions)
        {
            if (mission.missionInfo.Type == MissionType.GetResource && mission.missionInfo.ValueType == ValueType.Health)
            {
                mission.value += value;
                if (mission.value >= mission.missionInfo.NeedGetValue)
                    mission.isFinish = true;

                ChangeMissionDataById(mission.missionInfo.Id, mission.value, mission.isFinish);

                OnMissionUpdate?.Invoke(mission);
            }
        }
    }

    private void OnJoyAdd(int value)
    {
        foreach (Mission mission in currentMissions)
        {
            if (mission.missionInfo.Type == MissionType.GetResource && mission.missionInfo.ValueType == ValueType.Joy)
            {
                mission.value += value;
                if (mission.value >= mission.missionInfo.NeedGetValue)
                    mission.isFinish = true;

                ChangeMissionDataById(mission.missionInfo.Id, mission.value, mission.isFinish);

                OnMissionUpdate?.Invoke(mission);
            }
        }
    }
    #endregion

    #region TemporatyBuildingMission

    private Store store;

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    { 
        if (temporaty_linkOnStore.Instance != null) 
            store = temporaty_linkOnStore.Instance.Store;
         
        if (store != null)
        {
            store.BuyEvent += OnBuyBuilding;
        } 
    }

    private void OnSceneUnloaded(Scene scene)
    { 
        if (store != null)
        {
            store.BuyEvent -= OnBuyBuilding;
        } 
    }

    private void OnBuyBuilding(BuildingInfo buildingInfo, BuildingColor buildingColor)
    {
        foreach (Mission mission in currentMissions)
        {
            if (mission.missionInfo.Type == MissionType.GetBuilding)
            {
                if (mission.missionInfo.AnyBuilding)
                {
                    mission.value = 1;
                    mission.isFinish = true;
                    ChangeMissionDataById(mission.missionInfo.Id, mission.value, mission.isFinish);

                    OnMissionUpdate?.Invoke(mission);
                }
                else if (mission.missionInfo.BuildingInfo == buildingInfo)
                {
                    mission.value = 1;
                    mission.isFinish = true;
                    ChangeMissionDataById(mission.missionInfo.Id, mission.value, mission.isFinish);

                    OnMissionUpdate?.Invoke(mission);
                }
            }
        }
    }
    #endregion
}
