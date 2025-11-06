using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameConstants;

public interface IStatUpgrader
{
    string Key { get; } // 어떤 스탯인지 식별 (예: "Attack")
    void ApplyUpgrade(PlayerCondition condition, PlayerInventory inventory,  ref int level, ref int cost); // BigInteger로 변경
    string GetCurrentValue(PlayerCondition condition, PlayerInventory inventory);
    string GetNextValue(PlayerCondition condition, PlayerInventory inventory, int level);
    void Initialize(PlayerCondition condition, PlayerInventory inventory, int level); // 초기 값 적용 로직 추가
}
public class LaboratorySlot : MonoBehaviour
{
    public Player player;
    public PlayerCondition playerCondition;
    public PlayerInventory playerInventory;
    public CustomerManager customerManager;
    public NPCManager npcManager;
    
    public LaboratoryUI laboratoryUI;
    public Button Upgradebtn;
    public TextMeshProUGUI CurLv;
    public TextMeshProUGUI CurValue;
    public TextMeshProUGUI NextValue;
    public TextMeshProUGUI CostText;
    public int lv = 0;


    public int cost = 1;  
    public IStatUpgrader statUpgrader;


    [SerializeField]public string key; // 스탯을 식별하는 키
    public bool CanUpgrade() => statUpgrader != null && lv < 20 && playerInventory.diamond >= cost;

    void Start()
    {
        laboratoryUI = GetComponentInParent<LaboratoryUI>();

        player = PlayerManager.Instance.Player;
        playerCondition = player.playerCondition;
        playerInventory = player.playerInventory;
        customerManager = CustomerManager.Instance;
        npcManager = NPCManager.Instance;

        
        switch (key)
        {
            case "Attack":
                statUpgrader = new AttackUpgrader();
                break;
            
            case "Health":
                statUpgrader = new HealthUpgrader();
                break;

            case "MoveSpeed":
                statUpgrader = new MoveSpeedUpgrader();
                break;

            case "SaleBonus":
                statUpgrader = new SaleBonusUpgrader();
                break;

            case "ProductionSpeedBonus":
                statUpgrader = new ProductionSpeedBonusUpgrader();
                break;
            
            case "SpawnCount":
                statUpgrader = new SpawnCountUpgrader();
                break;
            
            case "HunterAttack":
                statUpgrader = new HunterAttackUpgrader();
                break;
            
            case "MonsterSpawn":
                statUpgrader = new MonsterSpawnUpgrader();
                break;
            
            default:
                Debug.LogError($"[LaboratorySlot] Invalid stat key: {key} in GameObject {gameObject.name}");
                Upgradebtn.interactable = false; // 예시: 버튼 비활성화
                return;
        }
        if (SaveLoadManager.Instance.isClickedContinue || SaveLoadManager.Instance.isClickedNext)
        {
            LoadLaboratoryStatData();
        }
        else
        {
            statUpgrader.Initialize(playerCondition, playerInventory, 1);
        }

        if (laboratoryUI.AllMaxLevelReached())
        {
            EventManager.Instance.Publish(new NextMapEvent { triggerNum = (int)NextMap.B });
            Debug.Log("B조건 충족");
        }

        ApplyStats(lv);
        UpdateUI();
    }

    private void LoadLaboratoryStatData()
    {
        var data = SaveLoadManager.Instance.saveData;
        if (data == null || data.laboratoryStatData == null) return;

        if (data.laboratoryStatData.TryGetValue(key, out PlayerStatData slotData))
        {
            lv = slotData.level;
            cost = slotData.cost;
        }
    }

    public void OnClickUpgradeButton()
    {
        if (statUpgrader == null) return;
        Analytics.AddEvent("player_upgrade_count", new Dictionary<string, object>
        {
            { "upgrade_name", key }
        });
        
        if (SceneLoadManager.Instance.NowSceneName == "MainGameScene" && lv >= 10) return;
        if (lv >= (statUpgrader is MonsterSpawnUpgrader ? 5 : 20) || playerInventory.diamond < cost) return; // 모든 업그레이드 최대지 20 고정

        playerInventory.diamond -= cost;
        
        statUpgrader.ApplyUpgrade(playerCondition, playerInventory, ref lv, ref cost);        
        
        if (key == "Health")
            playerCondition.Heal(2);

        UpdateUI();
        if (laboratoryUI.AllMaxLevelReached())
        {
            EventManager.Instance.Publish(new NextMapEvent { triggerNum = (int)NextMap.B });
            Debug.Log("B조건 충족");
        }
    }

    private void UpdateUI()
    {
        if (statUpgrader == null) return;
        
        CurLv.text = "Lv. " + lv.ToString();
        CurValue.text = statUpgrader.GetCurrentValue(playerCondition, playerInventory);
        string nextValText = statUpgrader.GetNextValue(playerCondition, playerInventory, lv);
        NextValue.text = nextValText;
        if (nextValText == "Max")
        {
            CostText.text = "Max";
        }
        else
        {
            CostText.text = "비용: " + cost; 
        }
    }

    private void ApplyStats(int level)
    {
        if (statUpgrader == null) return;

        statUpgrader.Initialize(playerCondition, playerInventory, level);
    }    
}
