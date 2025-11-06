using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using static GameConstants;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public enum ArchType
{
    Cook,
    Stand,
    Counter,
    Warehouse,
    DungeonWall,
    None,
}

public class ArchitectureManager : MonoSingleton<ArchitectureManager>
{
    protected override bool isDestroy => true;
    public StageData stageData;

    public Dictionary<ItemSO, BaseArchitecture> cooks = new Dictionary<ItemSO, BaseArchitecture>();
    public Dictionary<ItemSO, BaseArchitecture> stands = new Dictionary<ItemSO, BaseArchitecture>();
    public List<ArchSpawner> dungeonWall = new List<ArchSpawner>();
    public List<ArchSpawner> archSpawners = new List<ArchSpawner>();
    
    public Dictionary<int, BaseArchitecture> counters =  new Dictionary<int, BaseArchitecture>();
    public List<BaseArchitecture> warehouses = new List<BaseArchitecture>();
    public List<ArchSpawner> walls = new List<ArchSpawner>();
    
    public Dictionary<ItemSO, bool> cookConditions = new();
    public Dictionary<ItemSO, bool> standConditions = new();

    private int effective;
    public int divideAmount;
    public int counterCount;

    private void LoadCounterMoneyData(BaseArchitecture arch)
    {
        var data = SaveLoadManager.Instance.saveData;
        if (data == null)
        {
            Debug.LogError("세이브 데이터가 존재하지 않습니다.");
            return;
        }

        foreach (var counterMoney in data.counterMoney)
        {
            if (counterMoney.Key == arch.archID)
            {
                if (BigInteger.TryParse(counterMoney.Value, out BigInteger money))
                {
                    // playerInventory.Money = money;
                    arch.GetComponent<Counter>().gold = money;
                }
            }
        }
    }

    private void AddArchitecture(ItemSO item, BaseArchitecture arch, ArchType archType)
    {
        switch (archType)
        {
            case ArchType.Cook:
                cooks.Add(item, arch);
                break;
            case ArchType.Stand:
                stands.Add(item, arch);
                break;
            case ArchType.Counter:
                counters.Add(arch.archID, arch);
                if (SaveLoadManager.Instance.isClickedContinue)
                {
                    LoadCounterMoneyData(arch);
                }
                var orderItems = stageData.orderItems;
                for (int i = orderItems.Length - divideAmount * counterCount; i < orderItems.Length - divideAmount * (counterCount - 1); i++)
                {
                    arch.GetComponent<Counter>().calculateItems.Add(orderItems[i]);
                }

                counterCount--;
                break;
            case ArchType.Warehouse:
                warehouses.Add(arch);
                break;
        }
    }

    public void CreateArchSpawner(ArchDataSO archData, GameObject spawnerPrefab, GameObject archPrefab, Vector3 createPos, ArchType archType, int targetMoney)
    {
        var archSpawner = Instantiate(spawnerPrefab, createPos, Quaternion.identity).GetComponent<ArchSpawner>();
        archSpawner.transform.SetParent(this.transform);
        archSpawner.archType = archType;
        archSpawner.archData = archData;
        archSpawner.archPrefab = archPrefab;

        if(archSpawner.archText != null)
            archSpawner.archText.text = archData.archName;
        archSpawner.targetMoney = targetMoney;
        if (SceneManager.GetActiveScene().name == GameConstants.SceneNames.TUTORIAL_SCENE && archData.archID == GameConstants.Architecture.TUTORIAL_ARCH_ID)
        {
            archSpawner.targetMoney = GameConstants.Architecture.TUTORIAL_TARGET_MONEY;
        }
        
        archSpawners.Add(archSpawner);
    }

    public void CreateArch(ArchDataSO archData, GameObject prefab, Vector3 createPos, ArchType archType, ArchSpawner archSpawner)
    {
        var arch = Instantiate(prefab, createPos, Quaternion.identity).GetComponent<BaseArchitecture>();
        arch.transform.SetParent(this.transform);
        arch.Init(archData);
        AddArchitecture(archData.productData, arch, archType);

        if (archSpawners.Contains(archSpawner) && archSpawner != null)
        {
            archSpawners.Remove(archSpawner);
        }

        if (NextSceneCheck())
        {
            EventManager.Instance.Publish(new NextMapEvent { triggerNum = (int)NextMap.A });
            Debug.Log("A조건 충족");
        }

        Analytics.AddEvent("arch_unlock", new Dictionary<string, object>
        {
            { "arch_id", archData.archID }
        });
    }

    public BaseArchitecture GetBestArch(NPC npc,
        BaseArchitecture current = null,
        float wDist = GameConstants.Architecture.WEIGHT_DISTANCE,
        float wStock = GameConstants.Architecture.WEIGHT_STOCK,
        float distGamma = GameConstants.Architecture.DISTANCE_GAMMA,
        float stockGamma = GameConstants.Architecture.STOCK_GAMMA,
        float stickiness = GameConstants.Architecture.STICKINESS,
        float etaEpsilon = GameConstants.Architecture.ETA_EPSILON)
    {
        List<BaseArchitecture> archList = new List<BaseArchitecture>();
        
        archList.AddRange(cooks.Values);
        
        if (archList.Count == 0) return null;
        
        float minDistance = float.MaxValue, maxDistance = 0f;
        int minCount = int.MaxValue, maxCount = 0;

        var dists = new Dictionary<BaseArchitecture, float>();
        var counts = new Dictionary<BaseArchitecture, int>();

        foreach (var arch in archList)
        {
            float dist = (arch.transform.position - npc.transform.position).sqrMagnitude;
            dists[arch] = dist;
            minDistance = Mathf.Min(dist, minDistance);
            maxDistance = Mathf.Max(dist, maxDistance);

            int count = Mathf.Max(0, arch.productCount);
            counts[arch] = count;
            minCount = Mathf.Min(minCount, count);
            maxCount = Mathf.Max(maxCount, count);
        }

        BaseArchitecture bestArch = null;
        float bestScore = float.NegativeInfinity;

        foreach (var arch in archList)
        {
            float dist = dists[arch];
            float count = counts[arch];
            
            float distNormalize = Mathf.Approximately(maxDistance, minDistance) ? 1f : Mathf.InverseLerp(maxDistance, minDistance, dist);
            float stockNormalize = (maxCount == minCount) ? (count > 0 ? 1f : 0f) : Mathf.InverseLerp(minCount, maxCount, count);
            
            distNormalize = Mathf.Pow(distNormalize, distGamma);
            stockNormalize = Mathf.Pow(stockNormalize, stockGamma);
            
            float score = wDist * distNormalize + wStock * stockNormalize;

            if (current != null && arch == current) score += stickiness;
            
            if (arch != null)
            {
                effective = arch.productCount;
                foreach (var waiter in NPCManager.Instance.npc[NPCType.Waiter])
                {
                    if (waiter != null && waiter.targetObj == arch.gameObject)
                    {
                        var waiterETA = GetPathLength(waiter.agent, arch.transform.position) / waiter.agent.speed;
                        var myETA = GetPathLength(npc.agent, arch.transform.position) / npc.agent.speed;
                        
                        if(waiterETA + etaEpsilon < myETA)
                            effective = Mathf.Max(effective - (waiter.npcInven.MaxQuantity - waiter.npcInven.CurrentQuantity), 0);
                    }
                }

                if (effective == 0) continue;
            }
            
            score += UnityEngine.Random.value * 1e-4f;

            if (score > bestScore)
            {
                bestScore = score;
                bestArch = arch;
            }
        }
        
        return bestArch;
    }
    
    private float GetPathLength(NavMeshAgent agent, Vector3 target)
    {
        if (!agent.isOnNavMesh) return Mathf.Infinity;
        
        var path = new NavMeshPath();
        if (!agent.CalculatePath(target, path)) return Mathf.Infinity;
        
        float len = 0f;
        for (int i = 1; i < path.corners.Length; i++)
            len += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        
        return len;
    }

    public bool DugeonWallCheck()
    {
        bool IsDungeonWall = true;
        foreach (var wall in dungeonWall)
        {
            if (wall.gameObject.activeSelf)
            {
                IsDungeonWall = false;
                break;
            }
        }
        return IsDungeonWall;
    }

    public bool NextSceneCheck()
    { 
        if(cooks.Count >= 4 && stands.Count >= 4 && DugeonWallCheck() == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
