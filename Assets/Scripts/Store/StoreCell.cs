using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoreCell : MonoBehaviour
{
    public event UnityAction<BuildingInfo, BuildingColor> BuyEvent;

    [SerializeField] private Store m_Store;
    [SerializeField] private BuildingInfo m_BuildingInfo;

    [SerializeField] private Image m_CellImage;
    [SerializeField] private UIButton m_CellButton;
    [SerializeField] private UIButton m_LeftButton;
    [SerializeField] private UIButton m_RightButton;

    [SerializeField] private Image m_CoinsImage;
    [SerializeField] private Image m_BoardsImage;
    [SerializeField] private Image m_BricksImage;
    [SerializeField] private Image m_NailsImage;

    [SerializeField] private TMP_Text m_name;
    [SerializeField] private TMP_Text m_CoinsText;
    [SerializeField] private TMP_Text m_BoardsText;
    [SerializeField] private TMP_Text m_BricksText;
    [SerializeField] private TMP_Text m_NailsText;    

    [SerializeField] private Color m_DisabledColor;
    [SerializeField] private Color m_EnabledColor; 
    [SerializeField] private Color m_TheColorOfResourceShortage;

    private bool interactable;
    private ColorSpritePair[] m_ColorSprites;
    private int m_CurrentColorIndex = 0;

    //DEBUG
    [SerializeField] private TMP_Text buildingIDdebug;

    private void Start()
    {
        Initialize();
        m_LeftButton.OnClick.AddListener(ClickLeftButton);
        m_RightButton.OnClick.AddListener(ClickRightButton);
    }

    private void OnDestroy()
    {
        m_LeftButton.OnClick.RemoveListener(ClickLeftButton);
        m_RightButton.OnClick.RemoveListener(ClickRightButton);
    }

    private void Initialize()
    {  
        m_name.text = m_BuildingInfo.Name;
        m_CoinsText.text = m_BuildingInfo.NeededCoins.ToString();
        m_BoardsText.text = m_BuildingInfo.NeededBoards.ToString();
        m_BricksText.text = m_BuildingInfo.NeededBricks.ToString();
        m_NailsText.text = m_BuildingInfo.NeededNails.ToString();
        List<ColorSpritePair> colorSprites = new List<ColorSpritePair>(m_BuildingInfo.ColorSprites);
        colorSprites.RemoveAll(x => x.Color == BuildingColor.None);
        m_ColorSprites =  colorSprites.ToArray();
        m_CellImage.sprite = m_ColorSprites[m_CurrentColorIndex].Sprite;
        m_LeftButton.Disabled();
        m_RightButton.Enabled();

        if (buildingIDdebug != null)
        {
            buildingIDdebug.text = m_BuildingInfo.BuildingID.ToString();
        }
    }

    public void CellUpdate(int coinsCount, int boardsCount, int bricksCount, int nailsCount)
    {
        if (m_Store == null) return;

        interactable = true;        

        if (coinsCount < m_BuildingInfo.NeededCoins)
        {
            if (interactable == true)
            {
                SetCellColor(m_DisabledColor);
            }
            
            m_CoinsImage.color = m_TheColorOfResourceShortage;
            m_CoinsText.color = m_TheColorOfResourceShortage;
            interactable = false;
        }
        if (boardsCount < m_BuildingInfo.NeededBoards)
        {
            if (interactable == true)
            {
                SetCellColor(m_DisabledColor);
            }
            m_BoardsImage.color = m_TheColorOfResourceShortage;
            m_BoardsText.color = m_TheColorOfResourceShortage;
            interactable = false;
        }
        if (bricksCount < m_BuildingInfo.NeededBricks)
        {
            if (interactable == true)
            {
                SetCellColor(m_DisabledColor);
            }
            m_BricksImage.color = m_TheColorOfResourceShortage;
            m_BricksText.color = m_TheColorOfResourceShortage;
            interactable = false;
        }
        if (nailsCount < m_BuildingInfo.NeededNails)
        {
            if (interactable == true)
            {
                SetCellColor(m_DisabledColor);
            }
            m_NailsImage.color = m_TheColorOfResourceShortage;
            m_NailsText.color = m_TheColorOfResourceShortage;
            interactable = false;
        }

        if (interactable == true)
        {
            SetCellColor(m_EnabledColor);
            m_CellButton.Enabled();
        }
        else
        {
            m_CellButton.Disabled();
        }
        
    }

    private void SetCellColor(Color color)
    {
        m_CoinsImage.color = color;
        m_CoinsText.color = color;
        m_BoardsImage.color = color;
        m_BoardsText.color = color;
        m_BricksImage.color = color;
        m_BricksText.color = color;
        m_NailsImage.color = color;
        m_NailsText.color = color;
    }

    public void Buy()
    {
        if (m_BuildingInfo == null) return;

        BuyEvent?.Invoke(m_BuildingInfo, m_ColorSprites[m_CurrentColorIndex].Color);
    }

    private void ClickLeftButton()
    {
        if (m_CurrentColorIndex <= 0) return;

        m_CurrentColorIndex--;
        m_CellImage.sprite = m_ColorSprites[m_CurrentColorIndex].Sprite;

        if (m_CurrentColorIndex <= 0)
            m_LeftButton.Disabled();
        if  (m_CurrentColorIndex < m_ColorSprites.Length - 1)
            m_RightButton.Enabled();
    }

    private void ClickRightButton()
    {
        if (m_CurrentColorIndex >= m_ColorSprites.Length - 1) return;

        m_CurrentColorIndex++;
        m_CellImage.sprite = m_ColorSprites[m_CurrentColorIndex].Sprite;

        if (m_CurrentColorIndex >= m_ColorSprites.Length - 1)
            m_RightButton.Disabled();
        if (m_CurrentColorIndex > 0)
            m_LeftButton.Enabled();
    }
}
