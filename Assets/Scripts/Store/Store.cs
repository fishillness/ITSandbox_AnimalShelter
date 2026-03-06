using System;
using UnityEngine;
using UnityEngine.Events;

public class Store : MonoBehaviour,
    IDependency<ValueManager>
{   
    public event UnityAction<BuildingInfo, BuildingColor> BuyEvent;

    [SerializeField] [Range(0, 1)] private float m_RefundPercentage;
    [SerializeField] private StoreCell[] m_Cells;

    private ValueManager m_ResourceManager;

    #region Constructs
    public void Construct(ValueManager m_ResourceManager) => this.m_ResourceManager = m_ResourceManager; 
    #endregion

    private void Start()
    {
        CellsUpdate();
        for (int i = 0; i < m_Cells.Length; i++)
        {
            m_Cells[i].BuyEvent += TryBuy;
        }
    }
    private void OnDestroy()
    {
        for (int i = 0; i < m_Cells.Length; i++)
        {
            m_Cells[i].BuyEvent -= TryBuy;
        }
    }

    public void TryBuy(BuildingInfo buildingInfo, BuildingColor buildingColor)
    {
        BuyEvent?.Invoke(buildingInfo, buildingColor);
    }

    public void Buy(BuildingInfo buildingInfo)
    {        
        m_ResourceManager.DeleteResources(buildingInfo.NeededCoins, buildingInfo.NeededBoards, buildingInfo.NeededBricks, buildingInfo.NeededNails, 0);
        CellsUpdate();               
    }

    public void Refund(BuildingInfo buildingInfo)
    {
        m_ResourceManager.AddResources(
            Mathf.CeilToInt(buildingInfo.NeededCoins * m_RefundPercentage),
            Mathf.CeilToInt(buildingInfo.NeededBoards * m_RefundPercentage),
            Mathf.CeilToInt(buildingInfo.NeededBricks * m_RefundPercentage),
            Mathf.CeilToInt(buildingInfo.NeededNails * m_RefundPercentage),
            0
            );
               
        CellsUpdate();
    }  
    private void CellsUpdate()
    {
        for (int i = 0; i < m_Cells.Length; i++)
        {
            m_Cells[i].CellUpdate(m_ResourceManager.CoinsCount, m_ResourceManager.BoardsCount, m_ResourceManager.BricksCount, m_ResourceManager.NailsCount);
        }
    }
}
