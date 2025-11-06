using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeBtn : MonoBehaviour
{
    [SerializeField] private NPCType type;
    [SerializeField] private int idx;
    [SerializeField] private List<MonsterData> monsters;
    [SerializeField] private TextMeshProUGUI kindText;
    private MonsterData monsterData;
    
    [SerializeField] private List<GameObject> buttons = new List<GameObject>();

    public void ResetButtons()
    {
        for (int i = 0; i < ArchitectureManager.Instance.standConditions.Count; i++)
        {
            buttons[i].SetActive(true);
        }
    }
    
    public void ChangeTarget(ItemSO awayItem)
    {
        if (NPCManager.Instance.npc[NPCType.Hunter].Count == 0) return;
        if (NPCManager.Instance.npc[NPCType.Hunter][idx].targetItem ==
            NPCManager.Instance.npc[NPCType.Hunter][idx].homeItem)
        {
            foreach (MonsterData monster in monsters)
            {
                if (monster.dropItem == awayItem.recipe[0])
                {
                    monsterData = monster;
                    break;
                }
            }

            NPCManager.Instance.npc[NPCType.Hunter][idx].targetMonsterData = monsterData;
            NPCManager.Instance.npc[NPCType.Hunter][idx].homeItem = awayItem.recipe[0];
            NPCManager.Instance.npc[NPCType.Hunter][idx].targetItem = NPCManager.Instance.npc[NPCType.Hunter][idx].homeItem;
            NPCManager.Instance.npc[NPCType.Hunter][idx].objType = awayItem.recipe[0].objType;
            kindText.text = awayItem.itemName;
        }
        else
        {
            NPCManager.Instance.npc[NPCType.Hunter][idx].homeItem = awayItem.recipe[0];
            NPCManager.Instance.npc[NPCType.Hunter][idx].objType = awayItem.recipe[0].objType;
            kindText.text = awayItem.itemName;
        }
    }
}
