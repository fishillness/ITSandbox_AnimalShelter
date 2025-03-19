using UnityEngine;
using UnityEngine.UI; 

public class UISettingController : MonoBehaviour,
    IDependency<InputController>
{
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private Button openWarningPanel;
    [SerializeField] private Button closeWarningPanel;

    private InputController inputController;

    #region Constructs
    public void Construct(InputController inputController) => this.inputController = inputController;
    #endregion

    private void Start()
    {
        if (inputController == null)
            GlobalGameDependenciesContainer.Instance.Rebind(this);

        settingPanel.SetActive(true);
        UISettingsTogglee[] UISettingsTogglee = GetComponentsInChildren<UISettingsTogglee>();
        foreach (UISettingsTogglee button in UISettingsTogglee)
        {
            button.ApplyProperty();
        }
        settingPanel.SetActive(false);
        warningPanel.SetActive(false);


        openButton.onClick.AddListener(OpenPanel);
        closeButton.onClick.AddListener(ClosePanel);
        openWarningPanel.onClick.AddListener(OpenWarningPanel);
        closeWarningPanel.onClick.AddListener(CloseWarningPanel);
    }

    private void OnDestroy()
    {
        openButton.onClick.RemoveListener(OpenPanel);
        closeButton.onClick.RemoveListener(ClosePanel);
        openWarningPanel.onClick.RemoveListener(OpenWarningPanel);
        closeWarningPanel.onClick.RemoveListener(CloseWarningPanel);
    }


    private void OpenPanel()
    {
        inputController.SetInputControllerMode(InputControllerModes.NotepadMode);
        settingPanel.SetActive(true);
    }

    private void ClosePanel()
    {
        inputController.SetInputControllerMode(InputControllerModes.CityMode);
        settingPanel?.SetActive(false);
    }

    private void OpenWarningPanel()
    {
        settingPanel.SetActive(false);
        warningPanel.SetActive(true);
    }

    private void CloseWarningPanel()
    {
        settingPanel.SetActive(true);
        warningPanel.SetActive(false);
    }
}
