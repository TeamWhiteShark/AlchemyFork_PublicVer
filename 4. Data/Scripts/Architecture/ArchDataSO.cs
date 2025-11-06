using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArchDataSO", menuName = "Data/ArchDataSO")]
public class ArchDataSO : ScriptableObject
{
    [Header("Arch Info")]
    public int archID;
    public string archName;
    public string productName;
    public string upgradeType;
    public int unlockMoney;
    public int upgradePrice;
    public float upgradeMultiplier;
    public float upgradeMultiplierChangeRate;
    public int productPrice;
    public float productUpgradeMultiplier;
    public float productUpgradeMultiplierChangeRate;
    public float productTime;
    public int maxLevel;
    public string conditionText;
    
    
    [Header("Arch Data")]
    public ArchType archType;
    public GameObject archPrefab;
    public ItemSO conditionArchData;
    public ItemSO productData;

    [Header("Quest")]
    public QuestSO[] questData;

}
