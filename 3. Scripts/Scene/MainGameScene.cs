using System.Collections;
using UnityEngine;

public class MainGameScene : SceneBase
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

        if (CustomerManager.Instance.stageData == null)
        {
            CustomerManager.Instance.stageData = this.stageData;
        }
        
        if (NPCManager.Instance.stageData == null)
            NPCManager.Instance.stageData = this.stageData;
        
        if(ArchitectureManager.Instance.stageData == null)
            ArchitectureManager.Instance.stageData = this.stageData;
        

        UIManager.Instance.OpenUI<InventoryUI>();
        UIManager.Instance.GetUI<InventoryUI>().UnLockButtons();
        UIManager.Instance.OpenUI<MercenaryUI>();
        UIManager.Instance.CloseUI<MercenaryUI>();
        UIManager.Instance.OpenUI<JoystickUI>();
        //UIManager.Instance.OpenUI<QuestUI>();
        //UIManager.Instance.CloseUI<QuestUI>();
        UIManager.Instance.GetUI<OrderUI>().stageData = this.stageData;
        UIManager.Instance.OpenUI<OrderUI>();
        UIManager.Instance.CloseUI<OrderUI>();
        UIManager.Instance.OpenUI<LaboratoryUI>();
        UIManager.Instance.CloseUI<LaboratoryUI>();
        UIManager.Instance.OpenUI<ShopUI>();
        UIManager.Instance.CloseUI<ShopUI>();
        
        if (!SaveLoadManager.Instance.isClickedContinue)
        {
            UIManager.Instance.OpenUI<TutorialUI>();
        }
        
        for (int i = 0; i < 3; i++)
        {
            ArchitectureManager.Instance.CreateArch(stageData.archSpawnData[i].archData,
                stageData.archSpawnData[i].archData.archPrefab, stageData.archSpawnData[i].position,
                stageData.archSpawnData[i].archData.archType, null);
        }
        
        for (int i = 3; i < 6; i++)
        {
            ArchitectureManager.Instance.CreateArchSpawner(stageData.archSpawnData[i].archData,
                Resources.Load<GameObject>("Prefabs/Architecture/DungeonWall"),
                stageData.archSpawnData[i].archData.archPrefab, stageData.archSpawnData[i].position,
                stageData.archSpawnData[i].archData.archType, stageData.archSpawnData[i].archData.unlockMoney);
        }
        
        for (int i = 6; i < stageData.archSpawnData.Length; i++)
        {
            ArchitectureManager.Instance.CreateArchSpawner(stageData.archSpawnData[i].archData,
                Resources.Load<GameObject>("Prefabs/Architecture/ArchSpawner"),
                stageData.archSpawnData[i].archData.archPrefab, stageData.archSpawnData[i].position,
                stageData.archSpawnData[i].archData.archType, stageData.archSpawnData[i].archData.unlockMoney);
        }
    }   
}
