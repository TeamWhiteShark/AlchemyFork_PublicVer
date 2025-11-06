using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class TutorialEnemyManager : MonoSingleton<TutorialEnemyManager>
{
    protected override bool isDestroy => true;
    
    [Header("오브젝트 풀링 매니저의 프리펩 인덱스")] 
    [SerializeField] private List<GameObject> monsterPrefab = new List<GameObject>();

    [Header("몬스터 스폰 관련")] 
    [SerializeField] private List<Vector2> spawnPosList = new List<Vector2>();
    [SerializeField] private float respawnDelay;
    [SerializeField] private int maxMonsterCount;
    
    private Dictionary<int, List<Monster>> spawnedMonsters = new Dictionary<int, List<Monster>>();
    
    // NavMesh Area 이름 (Navigation Modifier에서 설정한 이름과 동일해야 함)
    private int monsterArea1Mask;
    private int monsterArea2Mask;

    // public event Action<int, Monster> OnMonsterSpawned;

    protected override void Awake()
    {
        base.Awake();
        // Area 이름으로부터 비트 마스크 생성
        monsterArea1Mask = 1 << NavMesh.GetAreaFromName("MonsterArea");
        monsterArea2Mask = 1 << NavMesh.GetAreaFromName("MonsterArea_2");
    }

    private void Start()
    {        
        for (int i = 0; i < monsterPrefab.Count; i++)
        {
            spawnedMonsters[i] = new List<Monster>();
            StartCoroutine(SpawnCoroutine(i));
        }
    }

    private IEnumerator SpawnCoroutine(int prefabIndex)
    {
        while (true)
        {
            if (spawnedMonsters[prefabIndex].Count < maxMonsterCount)
            {
                SpawnEnemy(prefabIndex);
            }

            yield return new WaitForSeconds(respawnDelay);
        }
    }
    
    private void SpawnEnemy(int prefabIndex)
    {
        Vector2 pos = SpawnPos(prefabIndex);

        GameObject monsterObj = ObjectPoolManager.Instance.GetObject(monsterPrefab[prefabIndex], pos, Quaternion.identity);
        Monster monster = monsterObj.GetComponent<Monster>();
        spawnedMonsters[prefabIndex].Add(monster);
        monster.transform.SetParent(this.transform);
        
        EventManager.Instance.Publish(new MonsterSpawnedEvent { PrefabIndex = prefabIndex, MonsterInstance = monster });
        
        // NavMeshAgent 설정
        NavMeshAgent agent = monsterObj.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            // 해당 스폰 위치가 어떤 구역에 속하는지 판단
            if (IsInArea1(pos))
                agent.areaMask = monsterArea1Mask;
            else
                agent.areaMask = monsterArea2Mask;

            // 스폰 위치로 강제로 워프 (NavMesh 붙잡기)
            agent.Warp(pos);
        }
        
        monster.GetComponent<IPoolable>().OnBeforeReturn += obj =>
        {
            spawnedMonsters[prefabIndex].Remove(monster);
            obj.GetComponent<Monster>().die = true;
            ObjectPoolManager.Instance.ReturnObject(obj);
        };
    }
    
    private Vector2 SpawnPos(int prefabIndex)
    {
        int minIndex = prefabIndex * 2;
        int maxIndex = minIndex + 1;
        
        if (spawnPosList.Count <= maxIndex)
        {
            Debug.LogWarning($"[TutorialEnemyManager] spawnPosList가 {prefabIndex}번 몬스터의 스폰 구역 정보를 포함하지 않습니다.");
            return Vector2.zero;
        }

        Vector2 minPos = spawnPosList[minIndex];
        Vector2 maxPos = spawnPosList[maxIndex];
        
        return new Vector2(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y));
    }
    
    private bool IsInArea1(Vector2 pos)
    {
        for (int i = 0; i < spawnPosList.Count; i += 2)
        {
            Vector2 minPos = spawnPosList[i];
            Vector2 maxPos = spawnPosList[i + 1];

            // pos가 이 범위 안에 포함되면 해당 구역이 area1
            if (pos.x >= minPos.x && pos.x <= maxPos.x &&
                pos.y >= minPos.y && pos.y <= maxPos.y)
            {
                return (i / 2) % 2 == 0;
            }
        }

        return true;
    }

    public List<Monster> GetSpawnedMonsters(int prefabIndex)
    {
        if (spawnedMonsters.TryGetValue(prefabIndex, out var list))
        {
            return list;
        }

        return new List<Monster>();
    }
}