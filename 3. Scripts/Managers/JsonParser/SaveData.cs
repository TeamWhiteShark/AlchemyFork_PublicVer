using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class SaveData 
{
    private string fileName;
    private string directory;

    public SaveData(string fileName, string directory)
    {
        this.fileName = fileName;
        this.directory = directory;
    }
    
    public string GetDirectory() => Application.persistentDataPath + "/" + directory;

    public string GetFullPath() => GetDirectory() + "/" + fileName + ".json";
}

public class PlayerSaveData : SaveData
{
    public string playerMoney;
    public int playerDiamond;
    public Dictionary<int, string> counterMoney =  new Dictionary<int, string>();

    public string SceneName;

    // public PlayerStatData StatData = new PlayerStatData();
    public Dictionary<string, PlayerStatData> laboratoryStatData = new ();
    
    public Dictionary<string, int> itemCounts = new();
    public Dictionary<string, int> mushroomWarehouseItemCounts = new();
    public Dictionary<string, int> meatWarehouseItemCounts = new();
    public Dictionary<string, int> npcCounts = new();
    public Dictionary<string, string> shopItemCooltime = new();

    public List<ArchSaveData> unlockedArchs = new List<ArchSaveData>();
    public List<ArchSaveData> dungeonWalls = new List<ArchSaveData>();
    
    public PlayerSaveData(string fileName, string directory) : base(fileName, directory)
    {
    }
}

[System.Serializable]
public class ArchSaveData
{
    public int archID;
    public int currentLevel;
    public float upgradeMultiplier;
    public float productUpgradeMultiplier;
    public bool isUnlocked;
}

[System.Serializable]
public class PlayerStatData
{
    public int level;
    public int cost;
}