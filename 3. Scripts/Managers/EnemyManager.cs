using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyManager : MonoSingleton<EnemyManager>
{
    protected override bool isDestroy => true;
    
    [Header("오브젝트 풀링 매니저의 프리펩 인덱스")] 
    // 후에 오브젝트 풀링 매니저의 딕셔너리를 종류별로 분리해야 할 듯
    [SerializeField] private List<GameObject> monsterPrefab = new List<GameObject>();

    [Header("몬스터 스폰 관련")]
    [SerializeField] private List<Vector2> spawnPosList = new List<Vector2>();    
    [SerializeField] private float respawnDelay;
    public int maxMonsterCount;
    public int monsterCountBonus;
    public int totalMonsterCount;
    
    //[SerializeField] public List<GameObject> activeMonsters = new List<GameObject>();
    //[SerializeField] public List<GameObject> activeMonsters2 = new List<GameObject>();
    //[SerializeField] public List<GameObject> activeMonsters3 = new List<GameObject>();   

    public Dictionary<MonsterData, HashSet<Monster>> monsters = new();
    [SerializeField] private int spawnID = 0;

    private Dictionary<int, List<Monster>> spawnedMonsters = new Dictionary<int, List<Monster>>();

    private int monsterArea1Mask;
    private int monsterArea2Mask;

    protected override void Awake()
    {
        base.Awake();
        // Area 이름으로부터 비트 마스크 생성
        monsterArea1Mask = 1 << NavMesh.GetAreaFromName("MonsterArea");
        monsterArea2Mask = 1 << NavMesh.GetAreaFromName("MonsterArea_2");
    }

    public int GetSpawnID()
    {
        ++spawnID;
        if(spawnID >= 10000) spawnID = 1;
        return spawnID;
    }

    private void Register(Monster monster)
    {
        if (monster == null || monster.Condition.monsterData == null) return;

        if (!this.monsters.TryGetValue(monster.Condition.monsterData, out HashSet<Monster> monsters))
        {
            monsters = new HashSet<Monster>();
            this.monsters.Add(monster.Condition.monsterData, monsters);
        }

        monsters.Add(monster);
    }

    private void Unregister(Monster monster)
    {
        if (monster == null || monster.Condition.monsterData == null) return;

        if (this.monsters.TryGetValue(monster.Condition.monsterData, out HashSet<Monster> monsters))
        {
            monsters.Remove(monster);
            if (monsters.Count == 0)
                this.monsters.Remove(monster.Condition.monsterData);
        }
    }

    public Monster GetNearestMonster(MonsterData monsterData, Vector3 position)
    {
        if (!this.monsters.TryGetValue(monsterData, out HashSet<Monster> monsters) || monsters.Count == 0) return null;
        Monster nearest = null;
        float minDist = float.MaxValue;

        foreach (var monster in monsters)
        {
            var distance = (monster.transform.position - position).sqrMagnitude;
            if (distance < minDist)
            {
                nearest = monster;
                minDist = distance;
            }
        }
        
        return nearest;
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
            if (spawnedMonsters[prefabIndex].Count < totalMonsterCount)
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
        monster.GetComponent<Monster>().spawnID = GetSpawnID();
        Register(monster.GetComponent<Monster>());
        monster.transform.SetParent(this.transform);
        monster.name = monster.name.Replace("(Clone)", spawnID.ToString());

        NavMeshAgent agent = monsterObj.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            //// 해당 스폰 위치가 어떤 구역에 속하는지 판단
            //if (IsInArea1(pos))
            //    agent.areaMask = monsterArea1Mask;
            //else
            //    agent.areaMask = monsterArea2Mask;

            // 스폰 위치로 강제로 워프 (NavMesh 붙잡기)
            agent.Warp(pos);
        }

        monster.GetComponent<IPoolable>().OnBeforeReturn += obj =>
        {
            Unregister(monster.GetComponent<Monster>());
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

    //public void MonsterToWander()
    //{
    //    foreach (var monster in activeMonsters)
    //    {
    //        if (monster.GetComponent<Monster>().Controller.monsterState == MonsterState.Die)
    //        {
    //            continue;
    //        }
    //        else if (monster.GetComponent<Monster>().Controller.monsterState == MonsterState.Attacking)
    //        {
    //            monster.GetComponent<Monster>().Controller.monsterState = MonsterState.Wandering;
    //        }
    //    }
    //}
}
