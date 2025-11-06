using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoSingleton<SaveLoadManager>
{
    protected override bool isDestroy => false;

    [SerializeField] private float saveTimer = 20f;
    private float curTime = 0f;

    public PlayerSaveData saveData;
    public bool isClickedContinue = false;
    public bool isClickedNext = false;
    
    private const string CloudFileName = "PlayerSave.json";

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == GameConstants.SceneNames.MAIN_GAME_SCENE || SceneManager.GetActiveScene().name == GameConstants.SceneNames.SECOND_MAIN_GAME_SCENE)
        {
            curTime += Time.deltaTime;
            if (curTime >= saveTimer)
            {
                Debug.Log("세이브 중...");
                curTime = 0f;
                ConfirmSaveData();
            }
        }
    }

    public async void ConfirmSaveData()
    {
        UIManager.Instance.GetUI<InventoryUI>().TurnOnSavingPanel();
        
        var data = new PlayerSaveData("SaveData", "SaveData");
        
        data.SceneName = SceneManager.GetActiveScene().name;
        data.playerMoney = (PlayerManager.Instance.Player.playerInventory.Money).ToString();
        data.playerDiamond = PlayerManager._instance.Player.playerInventory.diamond;

        if (UIManager.Instance.GetUI<LaboratoryUI>())
        {
            foreach (var slot in UIManager.Instance.GetUI<LaboratoryUI>().slots)
            {
                var slotData = new PlayerStatData() { level = slot.lv, cost = (int)slot.cost };

                data.laboratoryStatData[slot.key] = slotData;
            }
        }
        
        /*data.StatData.attackPointLevel = UIManager.Instance.GetUI<LaboratoryUI>().atkLv;
        data.StatData.healthPointLevel = UIManager.Instance.GetUI<LaboratoryUI>().healthLv;
        data.StatData.attackRateLevel = UIManager.Instance.GetUI<LaboratoryUI>().atkRateLv;
        data.StatData.maxQuantityLevel = UIManager.Instance.GetUI<LaboratoryUI>().maxQuantityLv;
        data.StatData.walkSpeedLevel = UIManager.Instance.GetUI<LaboratoryUI>().moveSpeedLv;
        data.StatData.saleBonusLevel = UIManager.Instance.GetUI<LaboratoryUI>().saleBonusLv;
        data.StatData.productionSpeedBonusLevel = UIManager.Instance.GetUI<LaboratoryUI>().productionSpeedBounsLv;*/
        
        if (PlayerManager.Instance.Player.playerInventory.itemsDic.Count > 0)
        {
            foreach (var item in PlayerManager.Instance.Player.playerInventory.itemsDic)
            {
                data.itemCounts[item.Key.itemID] = item.Value;
            }
        }

        if (ArchitectureManager.Instance.warehouses[0].itemsDic.Count > 0)
        {
            foreach (var item in ArchitectureManager.Instance.warehouses[0].itemsDic)
            {
                data.mushroomWarehouseItemCounts[item.Key.itemID] = item.Value;
            }
        }
        
        if (ArchitectureManager.Instance.warehouses[1].itemsDic.Count > 0)
        {
            foreach (var item in ArchitectureManager.Instance.warehouses[1].itemsDic)
            {
                data.meatWarehouseItemCounts[item.Key.itemID] = item.Value;
            }
        }

        foreach (var npcKey in NPCManager.Instance.npc.Keys)
        {
            if (NPCManager.Instance.npc[npcKey].Count > 0)
            {
                data.npcCounts[npcKey.ToString()] = NPCManager.Instance.npc[npcKey].Count;
            }
        }

        foreach (var counter in ArchitectureManager.Instance.counters.Values)
        {
            if (counter != null)
            {
                var archData = new ArchSaveData()
                {
                    archID = counter.archID,
                    currentLevel = counter.upgradeLevel,
                    upgradeMultiplier = counter.upgradeMultiplier,
                    productUpgradeMultiplier =  counter.productUpgradeMultiplier,
                };
            
                data.counterMoney.Add(counter.archID, counter.GetComponent<Counter>().gold.ToString()); 
                data.unlockedArchs.Add(archData);
            }
        }

        if (ArchitectureManager.Instance.warehouses.Count > 0)
        {
            foreach (var warehouse in ArchitectureManager.Instance.warehouses)
            {
                var archData = new ArchSaveData()
                {
                    archID = warehouse.archID,
                    currentLevel = warehouse.upgradeLevel,
                    upgradeMultiplier = warehouse.upgradeMultiplier,
                    productUpgradeMultiplier = warehouse.productUpgradeMultiplier,
                };
                
                data.unlockedArchs.Add(archData);
            }
        }

        if (ArchitectureManager.Instance.cooks.Count > 0)
        {
            foreach (var cook in ArchitectureManager.Instance.cooks)
            {
                var archData = new ArchSaveData()
                {
                    archID = cook.Value.archID,
                    currentLevel = cook.Value.upgradeLevel,
                    upgradeMultiplier = cook.Value.upgradeMultiplier,
                    productUpgradeMultiplier =  cook.Value.productUpgradeMultiplier,
                    isUnlocked = true
                };
                
                data.unlockedArchs.Add(archData);
            }
        }
        
        if (ArchitectureManager.Instance.stands.Count > 0)
        {
            foreach (var stand in ArchitectureManager.Instance.stands)
            {
                var archData = new ArchSaveData()
                {
                    archID = stand.Value.archID,
                    currentLevel = stand.Value.upgradeLevel,

                    isUnlocked = true
                };
                
                data.unlockedArchs.Add(archData);
            }
        }

        if (ArchitectureManager.Instance.dungeonWall.Count > 0)
        {
            foreach (var wall in ArchitectureManager.Instance.dungeonWall)
            {
                var wallData = new ArchSaveData()
                {
                    archID = wall.archData.archID, 
                    currentLevel = 0, 
                    isUnlocked = !(wall.gameObject.activeSelf)
                };
                
                data.dungeonWalls.Add(wallData);
            }
        }

        if (UIManager.Instance.GetUI<ShopUI>() != null)
        {
            foreach (var slot in UIManager.Instance.GetUI<ShopUI>().itemSlots)
            {
                if (slot.ItemData == null)
                    continue;

                string key = slot.ItemData.itemName;
                string timeString = (slot.DisabledTime - DateTime.Now).ToString();

                data.shopItemCooltime[key] = timeString;
            }
        }
        
        SaveLoadHelper.SaveData(data);

        await UploadJsonToCloudFilesAsync(data);
    }
    
    private async Task UploadJsonToCloudFilesAsync(PlayerSaveData data)
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogWarning("Cloud Save 불가: 로그인 상태가 아닙니다.");
            return;
        }

        try
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            
            await CloudSaveService.Instance.Files.Player.SaveAsync(CloudFileName, bytes);

            Debug.Log("Player Files 업로드 완료!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Cloud 파일 업로드 실패: {ex.Message}");
        }
    }
    
    public PlayerSaveData LoadPlayerDataFromLocal()
    {
        var data = SaveLoadHelper.LoadData<PlayerSaveData>("SaveData", "SaveData");

        if (data == null)
        {
            Debug.LogError("플레이어 데이터를 로드할 수 없었습니다.");
        }
        else
        {
            Debug.Log("플레이어 데이터를 로드했습니다.");
            saveData = data;
        }
        return data;
    }

    public async Task<PlayerSaveData> LoadPlayerDataFromCloudAsync()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogWarning("Cloud Load 불가: 로그인 상태가 아닙니다.");
            return null;
        }

        try
        {
            // Player Files에 업로드된 파일 다운로드
            var bytes = await CloudSaveService.Instance.Files.Player.LoadBytesAsync(CloudFileName);

            // 바이트 배열 → 문자열(JSON)
            string json = Encoding.UTF8.GetString(bytes);

            // JSON → PlayerSaveData 역직렬화
            var data = JsonConvert.DeserializeObject<PlayerSaveData>(json);

            if (data == null)
            {
                Debug.LogWarning("Cloud 파일은 존재하지만 JSON 파싱 실패");
                return null;
            }

            // 세이브 데이터를 게임 내에 반영
            saveData = data;

            // 로컬에도 백업
            SaveLoadHelper.SaveData(data);

            Debug.Log("클라우드 파일 불러오기 완료!");
            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Cloud 파일 다운로드 실패: {ex.Message}");
            return null;
        }
    }
}
