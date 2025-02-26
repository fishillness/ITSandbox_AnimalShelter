using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITaskInfo : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textNumber;
    [SerializeField] private Image boolImage;

    [Header("Sprites")]
    [SerializeField] private Sprite taskDone;
    [SerializeField] private Sprite taskFailed;

    private TaskInfo taskInfo;
    private UITaskInfoType uiTaskInfoType;

    public void SetProperties(Sprite sprite, int number, TaskInfo taskInfo, UITaskInfoType uiTaskInfoType)
    {
        image.sprite = sprite;
        if (textNumber)
            textNumber.text = number.ToString();
        this.taskInfo = taskInfo;
        this.uiTaskInfoType = uiTaskInfoType;

        if (uiTaskInfoType == UITaskInfoType.Boolean)
        {
            if (textNumber)
                textNumber.gameObject.SetActive(false);
            if (boolImage)
                boolImage.gameObject.SetActive(true);
        }
        else if (uiTaskInfoType == UITaskInfoType.Numberal)
        {
            if (textNumber)
                textNumber.gameObject.SetActive(true);
            if (boolImage)
                boolImage.gameObject.SetActive(false);
        }
    }

    public void UpdateNumberText(int number)
    {
        if (uiTaskInfoType == UITaskInfoType.Boolean)
            return;

        textNumber.text = number.ToString();
    }

    public bool IsThisUITaskInfo(TaskInfo taskInfo)
    {
        if (this.taskInfo == taskInfo)
            return true;

        return false;
    }

    public void UpdateCompleteInfo(bool complete)
    {
        boolImage.sprite = complete ? taskDone : taskFailed;
    }
}