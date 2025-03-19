using UnityEngine;

public class UIMainInGUI : MonoBehaviour,
    IDependency<InputController>
{
    [SerializeField] private GameObject shopButton;
    [SerializeField] private GameObject notepadButton;
    [SerializeField] private GameObject mainMenuButton;
    [SerializeField] private GameObject settingButton;
    [SerializeField] private GameObject bottomPanel;
    
    private InputController inputController;

    #region Constructs
    public void Construct(InputController inputController) => this.inputController = inputController;
    #endregion

    private void Start()
    {
        inputController.OnInputControllerModeChanges += CheckInputControllerMode;
    }

    private void OnDestroy()
    {
        inputController.OnInputControllerModeChanges -= CheckInputControllerMode;
    }

    private void CheckInputControllerMode(InputControllerModes inputControllerMode)
    {
        if (inputControllerMode == InputControllerModes.DialogMode)
        {
            ChangeObjectsVisibility(false);
        }
        else if (notepadButton.activeSelf == false)
        {
            ChangeObjectsVisibility(true);
        }
    }

    private void ChangeObjectsVisibility(bool value)
    {
        shopButton.SetActive(value);
        notepadButton.SetActive(value);
        mainMenuButton.SetActive(value);
        settingButton.SetActive(value);
        bottomPanel.SetActive(value);
    }
}
