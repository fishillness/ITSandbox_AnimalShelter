using UnityEngine;
using UnityEngine.Events;

public class TaskInfoCounter : MonoBehaviour,
    IDependency<PieceCounter>, IDependency<UIMatch3LevelPanel>, 
    IDependency<FieldController>, IDependency<UIEndLevelPanel>,
    IDependency<Match3Level>
{
    [HideInInspector]
    public UnityEvent OnAllTaskComplite;

    private PieceCounter pieceCounter;
    private UIMatch3LevelPanel levelPanel;
    private FieldController fieldController;
    private UIEndLevelPanel endLevelPanel;
    private Match3Level level;

    #region Constructs
    public void Construct(PieceCounter pieceCounter) => this.pieceCounter = pieceCounter;
    public void Construct(UIMatch3LevelPanel levelPanel) => this.levelPanel = levelPanel;
    public void Construct(FieldController fieldController) => this.fieldController = fieldController;
    public void Construct(UIEndLevelPanel endLevelPanel) => this.endLevelPanel = endLevelPanel;
    public void Construct(Match3Level level) => this.level = level;
    #endregion

    private TaskInfo[] taskInfos;
    private int taskCount;
    private bool isAllTaskComplite = false;

    private void Start()
    {
        pieceCounter.OnPieceRemoved.AddListener(OnPieceRemoved);
        level.OnLevelResult.AddListener(UpdateTaskInfoOnLosePanel);
    }

    private void OnDestroy()
    {
        pieceCounter.OnPieceRemoved.RemoveListener(OnPieceRemoved);
        level.OnLevelResult.RemoveListener(UpdateTaskInfoOnLosePanel);
    }

    public void InitTasks(TaskInfo[] taskInfos)
    {
        this.taskInfos = taskInfos;
        levelPanel.InitUITaskInfo();
        endLevelPanel.InitUITaskInfo();

        foreach (var taskInfo in this.taskInfos)
        {
            taskInfo.SetProperties();
            levelPanel.AddTaskInfo(taskInfo);
            endLevelPanel.AddUITaskInfo(taskInfo);
        }

        taskCount = this.taskInfos.Length;
    }

    private void OnPieceRemoved(Piece piece)
    {
        if (fieldController.IsStartFillingField) return;
        if (isAllTaskComplite) return;

        foreach (var taskInfo in taskInfos)
        {
            if (taskInfo.Type == piece.Type && taskInfo.TaskType == TaskInfoType.ByType)
            {
                RemovePiece(taskInfo);
            }
            else if (taskInfo.Type == piece.Type && taskInfo.TaskType == TaskInfoType.ByColor
                && piece.IsColorable)
            {
                if (piece.Colorable.Color == taskInfo.ColorType)
                {
                    RemovePiece(taskInfo);
                }
            }
        }

        if (taskCount == 0)
        {
            isAllTaskComplite = true;
            OnAllTaskComplite?.Invoke();
        }
    }

    private void RemovePiece(TaskInfo taskInfo)
    {
        bool isLastPiece = taskInfo.RemovePiece();
        levelPanel.UpdateTaskInfo(taskInfo);

        if (isLastPiece)
            taskCount--;
    }
    
    private void UpdateTaskInfoOnLosePanel(bool isWin)
    {
        if (isWin) return;

        foreach (var taskInfo in taskInfos)
        {
            if (taskInfo.CurrentCount == 0)
                endLevelPanel.UpdateUITaskInfo(taskInfo, true);
            else
                endLevelPanel.UpdateUITaskInfo(taskInfo, false);
        }
    }
}
