using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEndLevelPanel : MonoBehaviour,
    IDependency<Match3Level>, IDependency<Match3LevelManager>,
    IDependency<ValueManager>, IDependency<SoundsPlayer>
{
    [SerializeField] private GameObject endLevelPanel;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Button[] returnButtons;
    [SerializeField] private Button retryButton;
    [SerializeField] private TextMeshProUGUI retryButtonText;

    [Header("ResourcesText")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI boardsText;
    [SerializeField] private TextMeshProUGUI bricksText;
    [SerializeField] private TextMeshProUGUI nailsText;

    [Header("Sounds")]
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    [Header("CompleteTaskInfo")]
    [SerializeField] private GameObject taskGroup;
    [SerializeField] private UITaskInfo taskInfoPrefab;

    private Match3Level level;
    private Match3LevelManager levelManager;
    private ValueManager valueManager;
    private SoundsPlayer soundPlayer;

    private List<UITaskInfo> uiTaskInfos;

    #region Constructs
    public void Construct(Match3Level level) => this.level = level;
    public void Construct(Match3LevelManager levelManager) => this.levelManager = levelManager;
    public void Construct(ValueManager valueManager) => this.valueManager = valueManager;
    public void Construct(SoundsPlayer soundPlayer) => this.soundPlayer = soundPlayer;
    #endregion

    private void Awake()
    {
        if (levelManager == null)
            GlobalGameDependenciesContainer.Instance.Rebind(this);

        SetReceivedResource(levelManager.CurrentLevelInfo.Coins, levelManager.CurrentLevelInfo.Boards,
            levelManager.CurrentLevelInfo.Bricks, levelManager.CurrentLevelInfo.Nails);

        retryButtonText.text = $"-{levelManager.CurrentLevelInfo.CostInEnergy}";
    }

    private void Start()
    {
        background.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        level.OnLevelResult.AddListener(OnLevelResult);
        retryButton.onClick.AddListener(RetryLevel);

        foreach (var button in returnButtons)
        {
            button.onClick.AddListener(OpenCity);
        }
    }

    private void OnDestroy()
    {
        level.OnLevelResult.RemoveListener(OnLevelResult);
        retryButton.onClick.RemoveListener(RetryLevel);

        foreach (var button in returnButtons)
        {
            button.onClick.RemoveListener(OpenCity);
        }
    }

    private void SetReceivedResource(int coins, int boards, int bricks, int nails)
    {
        coinsText.text = coins.ToString();
        boardsText.text = boards.ToString();
        bricksText.text = bricks.ToString();
        nailsText.text = nails.ToString();
    }

    private void OnLevelResult(bool result)
    {
        endLevelPanel.SetActive(false);
        background.SetActive(true);

        if (result)
        {
            winPanel.SetActive(true);
            soundPlayer.LaunchSound(winSound);
        }
        else
        {
            losePanel.SetActive(true);
            soundPlayer.LaunchSound(loseSound);

            if (valueManager.EnergyCount < levelManager.CurrentLevelInfo.CostInEnergy)
                retryButton.interactable = false;
            else
                retryButton.interactable = true;
        }
    }

    private void RetryLevel()
    {
        valueManager.DeleteResources(0, 0, 0, 0, levelManager.CurrentLevelInfo.CostInEnergy);
        SceneController.Restart();
    }

    private void OpenCity()
    {
        SceneController.LoadSceneCity();
    }

    public void InitUITaskInfo()
    {
        uiTaskInfos = new List<UITaskInfo>();
    }

    public void AddUITaskInfo(TaskInfo taskInfo)
    {
        UITaskInfo uiTaskInfo = Instantiate(taskInfoPrefab, taskGroup.transform);
        uiTaskInfo.SetProperties(taskInfo.Sprite, taskInfo.Count, taskInfo, UITaskInfoType.Boolean);
        uiTaskInfos.Add(uiTaskInfo);
    }

    public void UpdateUITaskInfo(TaskInfo taskInfo, bool complete)
    {
        foreach (var uiTaskInfo in uiTaskInfos)
        {
            if (uiTaskInfo.IsThisUITaskInfo(taskInfo))
            {
                uiTaskInfo.UpdateCompleteInfo(complete);
                break;
            }
        }
    }
}
