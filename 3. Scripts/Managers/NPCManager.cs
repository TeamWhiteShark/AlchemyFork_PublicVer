using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NPCManager : MonoSingleton<NPCManager>
{
    protected override bool isDestroy => true;
    
    [SerializeField] private List<GameObject> npcPrefabs = new List<GameObject>();
    public StageData stageData;
    public GameObject waitingPoint;
    public GameObject leftRelaxPos;
    public GameObject rightRelaxPos;
    
    public Dictionary<NPCType, List<NPC>> npc =  new Dictionary<NPCType, List<NPC>>();
    
    public HashSet<NPC> reservedHunters = new HashSet<NPC>();


    private int maxHunterCount;
    public int MaxHunterCount {get => maxHunterCount; private set => maxHunterCount = value; }
    
    public int remainHunterCount;
    
    private INPCFactory npcFactory;

    public int hunterAttackPoint = 1;
    public int hunterAttackBonus;
    public int hunterTotalAttack;

    protected override void Awake()
    {
        base.Awake();
        npcFactory = new NPCFactory();
    }

    private void Start()
    {
        if (SaveLoadManager.Instance.isClickedContinue)
        {
            LoadNpcData();
        }

        if (stageData != null)
        {
            maxHunterCount = stageData.orderItems.Length;
        }
        else
        {
            Debug.LogError("[NPCManager] StageData is not assigned. MaxHunterCount might be incorrect");
            maxHunterCount = 3;
        }

        if (!SaveLoadManager.Instance.isClickedContinue)
        {
            remainHunterCount = 0;
        }
    }

    public void CreateNPC(Vector3 position, NPCType npcType)
    {
        GameObject npcObject = npcFactory.CreateNPC(npcType, position, this.transform);

        if (npcObject == null)
        {
            Debug.LogError($"[NPCManager] Failed to create NPC of type {npcType}");
            return;
        }
        
        NPC npcComponent = npcObject.GetComponent<NPC>();
        if (npcComponent == null)
        {
            Debug.LogError($"[NPCManager] Created NPC object for type {npcType} is missing NPC component.");
            ObjectPoolManager.Instance.ReturnObject(npcObject);
            return;
        }

        if (!npc.ContainsKey(npcType))
        {
            npc.Add(npcType, new List<NPC>());
        }
        npc[npcType].Add(npcComponent);

        if (npcType == NPCType.Hunter)
        {
            remainHunterCount++;
            UpdateMercenaryUIHunterAssignment(npcComponent);
        }

        UpdateMercenaryUINPCCounts();
    }

    private void UpdateMercenaryUIHunterAssignment(NPC hunter)
    {
        var ui = UIManager.Instance.GetUI<MercenaryUI>();
        if (ui != null && hunter != null && hunter.npcType == NPCType.Hunter)
        {
            ItemSO homeItemKey = hunter.homeItem;
            if (homeItemKey == null)
            {
                if (stageData != null && stageData.orderItems.Length > 0)
                {
                    Debug.Log("[NPCManager] Create Hunter has no initial homeItem. Assign via MercenaryUI]");
                }
            }
            else
            {
                if (!ui.hunterDict.ContainsKey(homeItemKey))
                {
                    ui.hunterDict[homeItemKey] = new Stack<NPC>();
                }
                ui.hunterDict[homeItemKey].Push(hunter);
            }

            foreach (var slotUI in ui.hunterSetUIList)
            {
                if(slotUI != null) slotUI.ResetUI();
            }
        }
    }

    private void UpdateMercenaryUINPCCounts()
    {
        var mercenaryUI = UIManager.Instance.GetUI<MercenaryUI>();
        if (mercenaryUI != null)
        {
            mercenaryUI.UpdateCountUI();
        }
    }

    private void LoadNpcData()
    {
        if (SaveLoadManager.Instance.saveData == null)
            return;

        NPCType currentType = NPCType.None;
        foreach (var npc in SaveLoadManager.Instance.saveData.npcCounts)
        {
            switch (npc.Key)
            {
                case "Hunter":
                    currentType = NPCType.Hunter;
                    break;
                case "Chef":
                    currentType = NPCType.Chef;
                    break;
                case "Waiter":
                    currentType = NPCType.Waiter;
                    break;
                case "Cashier":
                    currentType = NPCType.Cashier;
                    break;
            }
            
            if (currentType != NPCType.None)
            {
                for (int i = 0; i < npc.Value; i++)
                {
                    CreateNPC(GameConstants.NPC.LOAD_NPC_SPAWN_POSITION, currentType);
                }
            }
        }

        RecalculateRemainHunterCount();
        UpdateMercenaryUINPCCounts();
    }

    public void ClearAndReturnAllNPCs()
    {
        foreach (var npcList in npc.Values)
        { 
            ReturnNPCListToPool(npcList);
            npcList.Clear();
        }
        
        reservedHunters.Clear();
    }

    private void ReturnNPCListToPool(List<NPC> npcList)
    {
        if (npcList == null) return;
        foreach (var npc in npcList)
        {
            if (npc != null && npc.gameObject != null)
            {
                ObjectPoolManager.Instance.ReturnObject(npc.gameObject);
            }
        }
    }

    private void RecalculateRemainHunterCount()
    {
        remainHunterCount = 0;
        if (npc.TryGetValue(NPCType.Hunter, out List<NPC> value))
            foreach (var hunter in value.Where(hunter => hunter != null && hunter.homeItem == null))
            {
                remainHunterCount++;
            }
        Debug.Log($"[NPCManager] Recalculated remainHunterCount : {remainHunterCount}");
    }
        
    public int GetNPCCount(NPCType type)
    {
        if (npc.TryGetValue(type, out List<NPC> value))
            return value.Count;

        return 0;
    }  
}
