using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "ArchInfo", menuName = "GameData/CreateArchInfoData")]

public class ArchInfoSO : ScriptableObject
{
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

}
