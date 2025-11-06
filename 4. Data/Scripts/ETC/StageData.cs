using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage/stage")]
public class StageData : ScriptableObject
{
    //public PoolData[] poolData;

    public ItemSO[] orderItems;
    
    public GameObject playerPrefab;
    public Vector3 playerSpawnPos;
    
    public GameObject spawnerPrefab;
    public ArchSpawnData[] archSpawnData;

    public GameObject[] npcPrefabs;
}

[System.Serializable]
public class ArchSpawnData
{
    public Vector3 position;
    public ArchDataSO archData;
}