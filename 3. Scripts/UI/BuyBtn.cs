using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyBtn : MonoBehaviour
{
    public MercenaryUI mercenaryUI;
    private NPCType npcType;
    public string npcName;

    private void Start()
    {
        if (SaveLoadManager.Instance.isClickedContinue)
        {
            ToggleBuyBtn();
        }
    }

    private void ToggleBuyBtn()
    {
        switch (npcName)
        {
            case "Hunters":
                npcType = NPCType.Hunter;
                break;
            case "Chefs":
                npcType = NPCType.Chef;
                break;
            case "Waiters":
                npcType = NPCType.Waiter;
                break;
            case "Cashiers":
                npcType = NPCType.Cashier;
                break;
            default:
                Debug.LogWarning($"[BuyBtn] Unknown npcName: {npcName}");
                return;
        }

        // NPCManager의 해당 리스트가 비어 있는지 확인
        // bool hasNpc = false;
        // switch (npcType)
        // {
        //     case NPCType.Hunter:
        //         hasNpc = NPCManager.Instance.npc[NPCType.Hunter].Count > 0;
        //         break;
        //     case NPCType.Chef:
        //         hasNpc = NPCManager.Instance.npc[NPCType.Chef].Count > 0;
        //         break;
        //     case NPCType.Waiter:
        //         hasNpc = NPCManager.Instance.npc[NPCType.Waiter].Count > 0;
        //         break;
        //     case NPCType.Cashier:
        //         hasNpc = NPCManager.Instance.npc[NPCType.Cashier].Count > 0;
        //         break;
        // }
    }
    
    public void BuyMercenary()
    {
        if (NPCManager.Instance.npc[NPCType.Hunter].Count >= NPCManager.Instance.MaxHunterCount) return;
        if (PlayerManager.Instance.Player.playerInventory.Money - 1000 < 0) return;
        
        switch (npcName)
        {
            case "Hunters":
                npcType = NPCType.Hunter;
                break;
            case "Chefs":
                npcType = NPCType.Chef;
                break;
            case "Waiters":
                npcType = NPCType.Waiter;
                break;
            case "Cashiers":
                npcType = NPCType.Cashier;
                break;
        }

        PlayerManager.Instance.Player.playerInventory.Money -= 1000;
        Analytics.AddEvent("gold_spent_total", new Dictionary<string, object>
        {
            { "cost", 1000 },
            { "sink", "unlockNpc" }
        });
        NPCManager.Instance.CreateNPC(NPCManager.Instance.waitingPoint.transform.position, npcType);
    }
}
