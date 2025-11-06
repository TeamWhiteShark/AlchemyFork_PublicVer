using UnityEngine;
using UnityEditor;
[System.Serializable]
public class JsonToSO : MonoBehaviour
{
	[MenuItem("Tools/JsonToSO/CreateQuestSO")]
	static void QuestDataInit()
	{
		DynamicMenuCreator.CreateMenusFromJson<QuestData>("Quest.json", typeof(QuestSO));
	}
	[MenuItem("Tools/JsonToSO/CreateMonsterStatSO")]
	static void MonsterStatDataInit()
	{
		DynamicMenuCreator.CreateMenusFromJson<MonsterStatData>("MonsterStat.json", typeof(MonsterStatSO));
	}
	[MenuItem("Tools/JsonToSO/CreateArchInfoSO")]
	static void ArchInfoDataInit()
	{
		DynamicMenuCreator.CreateMenusFromJson<ArchInfoData>("ArchInfo.json", typeof(ArchInfoSO));
	}

}
