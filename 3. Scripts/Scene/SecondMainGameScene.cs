using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondMainGaneScene : SceneBase
{
    private GameObject player;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        player = Instantiate(stageData.playerPrefab, stageData.playerSpawnPos, Quaternion.identity);
        
        SaveLoadManager.Instance.LoadPlayerDataFromLocal();

        if (CustomerManager.Instance.stageData == null)
        {
            CustomerManager.Instance.stageData = this.stageData;
        }
        
        if (NPCManager.Instance.stageData == null)
            NPCManager.Instance.stageData = this.stageData;
        
        if (ArchitectureManager.Instance.stageData == null)
            ArchitectureManager.Instance.stageData = this.stageData;

        UIManager.Instance.OpenUI<InventoryUI>();
        UIManager.Instance.GetUI<InventoryUI>().UnLockButtons();
        UIManager.Instance.OpenUI<MercenaryUI>();
        UIManager.Instance.CloseUI<MercenaryUI>();
        UIManager.Instance.OpenUI<JoystickUI>();
        // UIManager.Instance.OpenUI<QuestUI>();
        // UIManager.Instance.CloseUI<QuestUI>();
        UIManager.Instance.GetUI<OrderUI>().stageData = this.stageData;
        UIManager.Instance.OpenUI<OrderUI>();
        UIManager.Instance.CloseUI<OrderUI>();
        UIManager.Instance.OpenUI<LaboratoryUI>();
        UIManager.Instance.CloseUI<LaboratoryUI>();
        UIManager.Instance.OpenUI<ShopUI>();
        UIManager.Instance.CloseUI<ShopUI>();

        ArchitectureManager.Instance.counterCount = 2;
        ArchitectureManager.Instance.divideAmount = stageData.orderItems.Length / 2;
    }

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            ArchitectureManager.Instance.CreateArch(stageData.archSpawnData[i].archData,
                stageData.archSpawnData[i].archData.archPrefab, stageData.archSpawnData[i].position,
                stageData.archSpawnData[i].archData.archType, null);
        }
        
        for (int i = 4; i < 6; i++)
        {
            ArchitectureManager.Instance.CreateArchSpawner(stageData.archSpawnData[i].archData,
                Resources.Load<GameObject>("Prefabs/Architecture/SecondDungeonWallHorizontal"),
                stageData.archSpawnData[i].archData.archPrefab, stageData.archSpawnData[i].position,
                stageData.archSpawnData[i].archData.archType, stageData.archSpawnData[i].archData.unlockMoney);
        }
        
        for (int i = 6; i < 9; i++)
        {
            ArchitectureManager.Instance.CreateArchSpawner(stageData.archSpawnData[i].archData,
                Resources.Load<GameObject>("Prefabs/Architecture/SecondDungeonWallVertical"),
                stageData.archSpawnData[i].archData.archPrefab, stageData.archSpawnData[i].position,
                stageData.archSpawnData[i].archData.archType, stageData.archSpawnData[i].archData.unlockMoney);
        }
        
        for (int i = 9; i < stageData.archSpawnData.Length; i++)
        {
            ArchitectureManager.Instance.CreateArchSpawner(stageData.archSpawnData[i].archData,
                Resources.Load<GameObject>("Prefabs/Architecture/ArchSpawner"),
                stageData.archSpawnData[i].archData.archPrefab, stageData.archSpawnData[i].position,
                stageData.archSpawnData[i].archData.archType, stageData.archSpawnData[i].archData.unlockMoney);
        }

        StartCoroutine(LoadWait());
    }

    private IEnumerator LoadWait()
    {
        yield return new WaitForSeconds(3f);
        SaveLoadManager.Instance.ConfirmSaveData();

    }
}
