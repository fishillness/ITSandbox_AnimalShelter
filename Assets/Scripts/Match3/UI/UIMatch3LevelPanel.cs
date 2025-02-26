using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMatch3LevelPanel : MonoBehaviour,
    IDependency<Match3LevelManager>
{
    [SerializeField] private TextMeshProUGUI textLevelNumber;
    [SerializeField] private TextMeshProUGUI textMoves;
    [SerializeField] private GameObject taskGroup;
    [SerializeField] private UITaskInfo taskInfoPrefab;

    private List<UITaskInfo> uiTaskInfos;
    private Match3LevelManager levelManager;

    #region Constructs
    public void Construct(Match3LevelManager levelManager) => this.levelManager = levelManager;
    #endregion

    private void Awake()
    {
        if (levelManager == null)
            GlobalGameDependenciesContainer.Instance.Rebind(this);

        textLevelNumber.text = levelManager.CurrentLevel.ToString();
    }

    public void InitUITaskInfo()
    {
        uiTaskInfos = new List<UITaskInfo>();
    }

    public void SetMoves(int startMoves)
    {
        textMoves.text = startMoves.ToString();
    }

    public void UpdateMoves(int moves)
    {
        textMoves.text = moves.ToString();
    }

    public void AddTaskInfo(TaskInfo taskInfo)
    {
        UITaskInfo uiTaskInfo = Instantiate(taskInfoPrefab, taskGroup.transform);
        uiTaskInfo.SetProperties(taskInfo.Sprite, taskInfo.Count, taskInfo, UITaskInfoType.Numberal);
        uiTaskInfos.Add(uiTaskInfo);
    }

    public void UpdateTaskInfo(TaskInfo taskInfo)
    {
        foreach (var uiTaskInfo in uiTaskInfos)
        {
            if (uiTaskInfo.IsThisUITaskInfo(taskInfo))
            {
                uiTaskInfo.UpdateNumberText(taskInfo.CurrentCount);
                break;
            }
        }
    }
}
