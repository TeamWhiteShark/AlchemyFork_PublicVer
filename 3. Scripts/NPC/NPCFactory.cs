using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface INPCFactory
{
    GameObject CreateNPC(NPCType type, Vector3 position, Transform parent = null);
}

public class NPCFactory : INPCFactory
{

    private readonly Dictionary<NPCType, string> npcPrefabPaths = new Dictionary<NPCType, string>()
    {
        { NPCType.Hunter, "Prefabs/NPC/Hunter" },
        { NPCType.Chef, "Prefabs/NPC/Chef" },
        { NPCType.Waiter, "Prefabs/NPC/Waiter" },
        { NPCType.Cashier, "Prefabs/NPC/Cashier" },
    };
    
    private readonly Dictionary<NPCType, GameObject> loadedPrefabs = new Dictionary<NPCType, GameObject>();
    
    public GameObject CreateNPC(NPCType type, Vector3 position, Transform parent = null)
    {
        GameObject prefab = GetPrefab(type);
        if (prefab == null)
        {
            Debug.LogError($"[NPCFactory] Failed to load prefab for NPCType : {type}]");
            return null;
        }
        
        GameObject npcInstance = ObjectPoolManager.Instance.GetObject(prefab, position, Quaternion.identity);
        if (npcInstance == null)
        {
            Debug.LogError($"[NPCFactory] Failed to get NPC instance from pool for NPCType : {type}]");
            return null;
        }

        if (parent != null)
        {
            npcInstance.transform.SetParent(parent);
        }

        var agent = npcInstance.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            var t= agent.Warp(position);
            Debug.Log(t);
        }
        else
        {
            Debug.LogError($"[NPCFactory] Created NPC of type {type} does not have a NavMeshAgent component.");
        }
        
        return npcInstance;
    }
    
    /// <summary>
    /// 지정된 NPCType에 해당하는 프리팹을 로드 (캐싱 활용).
    /// </summary>
    private GameObject GetPrefab(NPCType type)
    {
        if (loadedPrefabs.TryGetValue(type, out GameObject prefab))
        {
            return prefab;
        }

        if (!npcPrefabPaths.TryGetValue(type, out string prefabPath))
        {
            Debug.LogError($"[NPCFactory] Prefab path not defined for NPCType : {type}");
            return null;
        }
        
        prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab != null)
        {
            loadedPrefabs[type] = prefab;
        }
        else
        {
            Debug.LogError($"[NPCFactory] Prefab not found at path : Resources/{prefabPath}");
        }
        
        return prefab;
    }
}
