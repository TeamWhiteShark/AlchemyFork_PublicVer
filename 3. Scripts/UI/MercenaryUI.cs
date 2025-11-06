using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MercenaryUI : UIBase
{
    public override bool isDestroy => false;
    private bool isFirstOpen;
    [SerializeField] private GameObject HunterSetUI;
    [SerializeField] private GameObject HunterSetting;
    
    public List<HunterSetUI> hunterSetUIList = new List<HunterSetUI>();
    public Dictionary<ItemSO, Stack<NPC>> hunterDict = new Dictionary<ItemSO, Stack<NPC>>();
    
    [SerializeField] private List<TextMeshProUGUI> hunterCountList = new List<TextMeshProUGUI>();

    public override void OpenUI()
    {
        base.OpenUI();
        UIManager.Instance.isUIOn = true;
        
        if (!isFirstOpen)
        {
            for (int i = 0; i < NPCManager.Instance.stageData.orderItems.Length; i++)
            {
                var hunterSet = Instantiate(HunterSetUI, HunterSetting.transform).GetComponent<HunterSetUI>();
                hunterSetUIList.Add(hunterSet);
                
                hunterSet.mercenaryUI = this;
                hunterSet.dungeonNameText.text = $"던전{(char)('A' + i)}";
                hunterSet.targetItemData = NPCManager.Instance.stageData.orderItems[i].recipe[0];
                
                hunterDict.Add(NPCManager.Instance.stageData.orderItems[i].recipe[0], new Stack<NPC>());
                
                hunterSet.minusBtn.onClick.AddListener(() => MinusButton(hunterSet.targetItemData));
                hunterSet.plusBtn.onClick.AddListener(() => PlusButton(hunterSet.targetItemData));
                
                if (i == 0)
                {
                    hunterSet.pmBtn.SetActive(true);
                    hunterSet.lockObj.SetActive(false);
                }
                
                if (hunterDict.ContainsKey(NPCManager.Instance.stageData.orderItems[i]))
                    hunterSet.ResetUI();
            }
            
            isFirstOpen = true;
        }
    }

    public override void CloseUI()
    {
        base.CloseUI();
        UIManager.Instance.isUIOn = false;
    }

    private void UpdateUI()
    {
        for (int i = 0; i < hunterSetUIList.Count; i++)
        {
            hunterSetUIList[i].hunterCountText.text = hunterDict.TryGetValue(NPCManager.Instance.stageData.orderItems[i].recipe[0], out var value) ? value.Count.ToString() : "0"; 
        }
    }
    
    private void MinusButton(ItemSO targetItem)
    {
        if (NPCManager.Instance.npc[NPCType.Hunter].Count == 0) return;
        if (!hunterDict.ContainsKey(targetItem) || hunterDict[targetItem].Count == 0) return;
        
        if (hunterDict[targetItem].Count != 0)
        {
            var targetHunter = hunterDict[targetItem].Pop();
            
            if (NPCManager.Instance.reservedHunters.Contains(targetHunter))
                NPCManager.Instance.reservedHunters.Remove(targetHunter);
            
            targetHunter.homeItem = null;
            targetHunter.targetItem = null;
            targetHunter.targetMonsterData = null;
            targetHunter.targetObj = null;
            targetHunter.targetArchType = ArchType.Warehouse;
            
            NPCManager.Instance.remainHunterCount++;
        }
        
        UpdateUI();
    }

    private IEnumerator PlusButtonCoroutine(ItemSO targetItem)
    {
        if (NPCManager.Instance.npc[NPCType.Hunter].Count == 0) yield break;
        if (NPCManager.Instance.remainHunterCount == 0) yield break;
        if (!hunterDict.ContainsKey(targetItem) || hunterDict[targetItem].Count >= NPCManager.Instance.MaxHunterCount) yield break;

        NPC chosenHunter = null;

        foreach (var hunter in NPCManager.Instance.npc[NPCType.Hunter])
        {
            if (hunter == null) continue;
            if (hunter.homeItem != null) continue;
            if (NPCManager.Instance.reservedHunters.Contains(hunter)) continue;
            
            chosenHunter = hunter;
            break;
        }
        
        if (chosenHunter == null) yield break;
        
        chosenHunter.homeItem = targetItem;
        chosenHunter.targetItem = chosenHunter.awayItem;

        hunterDict[targetItem].Push(chosenHunter);
        UpdateUI();
        
        NPCManager.Instance.reservedHunters.Add(chosenHunter);
        NPCManager.Instance.remainHunterCount--;

        foreach (var monster in EnemyManager.Instance.monsters.Keys)
        {
            if (monster.dropItem == targetItem)
            {
                chosenHunter.targetMonsterData = monster;
                break;
            }
        }

        while (true)
        {
            if (chosenHunter == null || chosenHunter.npcInven == null) break;
            if(chosenHunter.npcInven.CurrentQuantity <= 0) break;
            if (!NPCManager.Instance.reservedHunters.Contains(chosenHunter)) yield break;
            
            yield return null;
        }
        
        bool ok = (chosenHunter != null && chosenHunter.npcInven != null && chosenHunter.npcInven.CurrentQuantity <= 0);
        if (!ok)
        {
            NPCManager.Instance.reservedHunters.Remove(chosenHunter);
            NPCManager.Instance.remainHunterCount++;
            yield break;
        }

        if (!hunterDict.ContainsKey(targetItem) || hunterDict[targetItem].Count > NPCManager.Instance.MaxHunterCount)
        {
            NPCManager.Instance.reservedHunters.Remove(chosenHunter);
            NPCManager.Instance.remainHunterCount++;
            yield break;
        }

        chosenHunter.targetItem = targetItem;
        chosenHunter.targetObj = null;
        chosenHunter.objType = targetItem.objType;
        chosenHunter.targetArchType = ArchType.DungeonWall;
        NPCManager.Instance.reservedHunters.Remove(chosenHunter);
    }

    private void PlusButton(ItemSO targetItem)
    {
        StartCoroutine(PlusButtonCoroutine(targetItem));
    }

    public void UpdateCountUI()
    {
        
        hunterCountList[0].text = NPCManager.Instance.npc.ContainsKey(NPCType.Hunter) ? $"{NPCManager.Instance.npc[NPCType.Hunter].Count}" : "0";
        hunterCountList[1].text = NPCManager.Instance.npc.ContainsKey(NPCType.Chef) ? $"{NPCManager.Instance.npc[NPCType.Chef].Count}" : "0";
        hunterCountList[2].text = NPCManager.Instance.npc.ContainsKey(NPCType.Waiter) ? $"{NPCManager.Instance.npc[NPCType.Waiter].Count}" : "0";
        hunterCountList[3].text = NPCManager.Instance.npc.ContainsKey(NPCType.Cashier) ? $"{NPCManager.Instance.npc[NPCType.Cashier].Count}" : "0";
    }
}
