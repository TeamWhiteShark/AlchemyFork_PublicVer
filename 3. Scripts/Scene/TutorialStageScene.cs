using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStageScene : SceneBase
{
    private GameObject player;
    
    protected override void Awake()
    {
        base.Awake();

        ArchitectureManager.Instance.counterCount = 1;
        ArchitectureManager.Instance.divideAmount = stageData.orderItems.Length;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        player = Instantiate(stageData.playerPrefab, stageData.playerSpawnPos, Quaternion.identity);
        player.GetComponent<Player>().playerInventory.SetMoneySilentlyInTutorial(400);
        
        UIManager.Instance.OpenUI<InventoryUI>();
        UIManager.Instance.OpenUI<JoystickUI>();
        // UIManager.Instance.OpenUI<QuestUI>();
        // UIManager.Instance.CloseUI<QuestUI>();
        // UIManager.Instance.OpenUI<LoadingUI>();
        
        if (CustomerManager.Instance.stageData == null)
        {
            CustomerManager.Instance.stageData = this.stageData;
        }
        
        if(ArchitectureManager.Instance.stageData == null)
            ArchitectureManager.Instance.stageData = this.stageData;
        
        ArchitectureManager.Instance.CreateArch(stageData.archSpawnData[0].archData, stageData.archSpawnData[0].archData.archPrefab, stageData.archSpawnData[0].position, stageData.archSpawnData[0].archData.archType, null);
        
        ArchitectureManager.Instance.CreateArchSpawner(stageData.archSpawnData[1].archData,
            Resources.Load<GameObject>("Prefabs/Architecture/DungeonWall"),
            stageData.archSpawnData[1].archData.archPrefab, stageData.archSpawnData[1].position,
            stageData.archSpawnData[1].archData.archType, stageData.archSpawnData[1].archData.unlockMoney);
        
        // ArchitectureManager.Instance.CreateArch(stageData.archSpawnData[1].archData, stageData.archSpawnData[1].archData.archPrefab, stageData.archSpawnData[1].position, stageData.archSpawnData[1].archData.archType);
        // ArchitectureManager.Instance.CreateArchSpawner(stageData.archSpawnData[3].archData,
        //     Resources.Load<GameObject>("Prefabs/Architecture/DungeonWall"),
        //     stageData.archSpawnData[3].archData.archPrefab, stageData.archSpawnData[3].position,
        //     stageData.archSpawnData[3].archData.archType, stageData.archSpawnData[3].archData.unlockMoney);
        //
        for (int i = 2; i < stageData.archSpawnData.Length; i++)
        {
            ArchitectureManager.Instance.CreateArchSpawner(stageData.archSpawnData[i].archData,
                Resources.Load<GameObject>("Prefabs/Architecture/ArchSpawner"),
                stageData.archSpawnData[i].archData.archPrefab, stageData.archSpawnData[i].position,
                stageData.archSpawnData[i].archData.archType, stageData.archSpawnData[i].archData.unlockMoney);
        }
    }
}
