using UnityEngine;

[CreateAssetMenu]
public class BuildingInfo : ScriptableObject
{
    public Building Building => m_Building;
    public Sprite StoreCellImage => m_StoreCellImage;
    public string Name => nameBuilding;
    public int NeededBoards => m_NeededBoards;
    public int NeededCoins => m_NeededCoins;
    public int NeededNails => m_NeededNails;
    public int NeededBricks => m_NeededBricks;
    public int Advancement => m_Advancement;
    public int Cosiness => m_Cosiness;
    public int Health => m_Health;
    public int Joy => m_Joy;

    [SerializeField] private Building m_Building;
    [SerializeField] private Sprite m_StoreCellImage;
    [SerializeField] private string nameBuilding;

    [Header("Needed Resources")]
    [SerializeField] private int m_NeededCoins;
    [SerializeField] private int m_NeededBoards;
    [SerializeField] private int m_NeededBricks;
    [SerializeField] private int m_NeededNails;

    [Header("ShelterCharacteristics")]
    [SerializeField] private int m_Advancement;
    [SerializeField] private int m_Cosiness;
    [SerializeField] private int m_Health;
    [SerializeField] private int m_Joy;

    ////// Debug
    public int BuildingID => m_Building.BuildingID;
}   
