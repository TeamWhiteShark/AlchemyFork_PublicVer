using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Quest", menuName = "GameData/CreateQuestData")]

public class QuestSO : ScriptableObject
{
	public int questID;
	public string questCategory;
	public int difficulty;
	public string questTitle;
	public string questDescription;
	public int targetObjectID;
	public int targetValue;
	public int rewardObjectID;
	public int rewardValue;

}
