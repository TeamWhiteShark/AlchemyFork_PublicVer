using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameConstants;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class ArchSpawner : MonoBehaviour
{
    public GameObject moneyPrefab;
    public GameObject archPrefab;
    public GameObject lockObj;
    public GameObject moneyObj;
    public ArchType archType;
    public ArchDataSO archData;
    public TextMeshProUGUI archText;
    public TextMeshProUGUI moneyText;
    private float waitTime = 0.1f;       
    private const float accel = 0.9f;    
    private const float minWait = 1f / 60f;
    public bool isUnlockedOnTutorial = false;

    private Coroutine _moneyCoroutine;
    
    public int targetMoney;
    public int repeat;
    private int repeatMax = 25;
    public int number = 1;
    
    private PlayerInventory playerInventory;
    
    private void Start()
    {
        if (archData.archType == ArchType.DungeonWall)
        {
            ArchitectureManager.Instance.dungeonWall.Add(this);
        }
        
        if (SaveLoadManager.Instance.isClickedContinue)
        {
            if (archType == ArchType.DungeonWall)
            {
                LoadDungeonWallData();
            }
            else
            {
                LoadArchData();
            }
        }

        if (SceneManager.GetActiveScene().name == GameConstants.SceneNames.TUTORIAL_SCENE)
        {
            isUnlockedOnTutorial = false;
        }
        
        UpdateMoney();
        if (ArchitectureManager.Instance.NextSceneCheck())
        {
            EventManager.Instance.Publish(new NextMapEvent { triggerNum = (int)NextMap.A });            
            Debug.Log("A조건 충족");
        }
    }

    private void Update()
    {
        if (archData.conditionArchData == null)
        {
            moneyObj.SetActive(true);
            lockObj.SetActive(false);
        }
        else
        {
            switch (archData.archType)
            {
                case ArchType.Stand:
                    if (ArchitectureManager.Instance.cooks.ContainsKey(archData.conditionArchData))
                    {
                        moneyObj.SetActive(true);
                        lockObj.SetActive(false);
                    }
                    break;
                case ArchType.DungeonWall:
                    if (ArchitectureManager.Instance.cooks.ContainsKey(archData.conditionArchData))
                    {
                        moneyObj.SetActive(true);
                        lockObj.SetActive(false);
                    }
                    break;
                default:
                    if (ArchitectureManager.Instance.cookConditions.ContainsKey(archData.conditionArchData))
                    {
                        moneyObj.SetActive(true);
                        lockObj.SetActive(false);
                    }
                    break;
            }
            
            if (ArchitectureManager.Instance.cookConditions.ContainsKey(archData.conditionArchData))
            {
                moneyObj.SetActive(true);
                lockObj.SetActive(false);
            }
        }
        
        
        
        if (playerInventory != null)
        {
            if (_moneyCoroutine == null)
            {
                if (archData.conditionArchData != null)
                {
                    switch (archData.archType)
                    {
                        case ArchType.Stand
                            when !ArchitectureManager.Instance.cooks.ContainsKey(archData.conditionArchData):
                            var ui1 = UIManager.Instance.GetUI<ConditionUI>();
                            ui1.archSpawner = this;
                            ui1.transform.position = this.transform.position + Vector3.up * 2;
                            UIManager.Instance.OpenUI<ConditionUI>();
                            return;
                        case ArchType.DungeonWall
                            when !ArchitectureManager.Instance.cooks.ContainsKey(archData.conditionArchData):
                            var ui2 = UIManager.Instance.GetUI<ConditionUI>();
                            ui2.archSpawner = this;
                            ui2.transform.position = this.transform.position + Vector3.up * 2;
                            UIManager.Instance.OpenUI<ConditionUI>();
                            return;
                        case ArchType.Cook:
                        case ArchType.Warehouse:
                        case ArchType.Counter:
                            if (ArchitectureManager.Instance.cookConditions.Count == 0 ||
                                ArchitectureManager.Instance.standConditions.Count == 0)
                            {
                                var ui = UIManager.Instance.GetUI<ConditionUI>();
                                ui.archSpawner = this;
                                ui.transform.position = this.transform.position + Vector3.up * 2;
                                UIManager.Instance.OpenUI<ConditionUI>();
                                return;
                            }

                            if (!ArchitectureManager.Instance.cookConditions.ContainsKey(archData.conditionArchData) ||
                                !ArchitectureManager.Instance.standConditions.ContainsKey(archData.conditionArchData))
                            {
                                var ui = UIManager.Instance.GetUI<ConditionUI>();
                                ui.archSpawner = this;
                                ui.transform.position = this.transform.position + Vector3.up * 2;
                                UIManager.Instance.OpenUI<ConditionUI>();
                                return;
                            }

                            if (!ArchitectureManager.Instance.cookConditions[archData.conditionArchData] ||
                                !ArchitectureManager.Instance.standConditions[archData.conditionArchData])
                            {
                                var ui = UIManager.Instance.GetUI<ConditionUI>();
                                ui.archSpawner = this;
                                ui.transform.position = this.transform.position + Vector3.up * 2;
                                UIManager.Instance.OpenUI<ConditionUI>();
                                return;
                            }

                            break;
                    }
                }
                
                _moneyCoroutine = StartCoroutine(GetMoney());
                
                if (targetMoney <= 0)
                {
                    if (archType != ArchType.DungeonWall)
                    {
                        if (SceneManager.GetActiveScene().name == "TutorialScene")
                        {
                            isUnlockedOnTutorial = true;
                        }
                        ArchitectureManager.Instance.CreateArch(archData, archPrefab, this.transform.position, archType, this);                                            
                    }
                    // if(archData.questData.Length != 0)
                    //     QuestManager.Instance.AddQuest(archData.questData);

                    if (archData.archType == ArchType.DungeonWall)
                    {
                        Analytics.AddEvent("dungeon_unlock", new Dictionary<string, object>
                        {
                            { "arch_id", archData.archID }
                        });

                        // if (archData.archID == 8001)
                        // {
                        //     UIManager.Instance.OpenUI<ReviewUI>();
                        // }
                        
                        var ui = UIManager.Instance.GetUI<OrderUI>();
                        ui.orderItems.Add(archData.productData);
                        ui.CloseUI();
                        
                        gameObject.SetActive(false);
                        ArchitectureManager.Instance.walls.Add(this);
                        
                        UpdateMercenaryUIForDungeonWall();
                        if (ArchitectureManager.Instance.NextSceneCheck())
                        {
                            EventManager.Instance.Publish(new NextMapEvent { triggerNum = (int)NextMap.A });
                            Debug.Log("A조건 충족");
                        }
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInventory = other.gameObject.GetComponent<PlayerInventory>();
        }
    }

    private IEnumerator GetMoney()
    {
        try
        {
            if (playerInventory.Money == 0)
            {
                
            }
            else if (playerInventory.Money - targetMoney < 0)
            {
                targetMoney -= (int)playerInventory.Money;
                Analytics.AddEvent("gold_spent_total", new Dictionary<string, object>
                {
                    { "cost", (int)playerInventory.Money },
                    { "sink", "unlockArch" }
                });
                playerInventory.ReleaseMoney(moneyPrefab, this.transform.position);
                playerInventory.Money = 0;
            }
            else if (targetMoney - number < 0)
            {
                playerInventory.Money -= targetMoney;
                Analytics.AddEvent("gold_spent_total", new Dictionary<string, object>
                {
                    { "cost", targetMoney },
                    { "sink", "unlockArch" }
                });
                playerInventory.ReleaseMoney(moneyPrefab, this.transform.position);                
                targetMoney = 0;
            }
            else
            {
                playerInventory.Money-= number;
                Analytics.AddEvent("gold_spent_total", new Dictionary<string, object>
                {
                    { "cost", number },
                    { "sink", "unlockArch" }
                });
                playerInventory.ReleaseMoney(moneyPrefab, this.transform.position);
                targetMoney -= number;
            }
            
            UpdateMoney();
            yield return new WaitForSeconds(waitTime);
            
            waitTime = Mathf.Max(waitTime*accel, minWait);
            repeat++;
            if (repeat >= repeatMax)
            {
                if (number >= 100) repeatMax = 50;
                number = Mathf.Min(number * 2, 500);
            }
        }
        finally
        {
            _moneyCoroutine = null;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInventory = null;
            waitTime = 0.2f;
            repeat = 0;
            repeatMax = 25;
            number = 1;
            UIManager.Instance.CloseUI<ConditionUI>();
        }
    }
    
    private void UpdateMoney()
    {
        moneyText.text = Utils.MoneyFormat(targetMoney);
    }

    private void LoadArchData()
    {
        var data = SaveLoadManager.Instance.saveData;
        if (data == null)
        {
            Debug.LogError("세이브 데이터가 존재하지 않습니다.");
            return;
        }
        
        if (data.unlockedArchs == null || data.unlockedArchs.Count == 0)
            return;
        
        bool isUnlocked = data.unlockedArchs.Exists(a => a.archID == archData.archID);

        if (isUnlocked)
        {
            // 이미 해금된 건축물이므로 즉시 생성
            ArchitectureManager.Instance.CreateArch(
                archData,
                archPrefab,
                transform.position,
                archType,
                this
            );
            
            Destroy(gameObject);

            if (ArchitectureManager.Instance.NextSceneCheck())
            {
                EventManager.Instance.Publish(new NextMapEvent { triggerNum = (int)NextMap.A });
                Debug.Log("A조건 충족 (LoadArchData)");
            }
        }
    }
    
    private void LoadDungeonWallData()
    {
        var data = SaveLoadManager.Instance.saveData;
        if (data == null)
        {
            Debug.LogError("세이브 데이터가 존재하지 않습니다.");
            return;
        }

        if (data.dungeonWalls == null || data.dungeonWalls.Count == 0)
            return;

        ArchSpawner targetWall = null;
        foreach (var wall in ArchitectureManager.Instance.dungeonWall)
        {
            if (wall.archData.archID == archData.archID)
            {
                targetWall = wall;
                break;
            }
        }

        if (targetWall == null)
            return;
        
        foreach (var savedWall in data.dungeonWalls)
        {
            if (savedWall.archID == archData.archID)
            {
                bool isUnlocked = savedWall.isUnlocked;
                targetWall.gameObject.SetActive(!isUnlocked);

                if (isUnlocked)
                {
                    var ui = UIManager.Instance.GetUI<OrderUI>();
                    ui.orderItems.Add(archData.productData);
                    ui.CloseUI();
                    
                    // 이미 해금된 상태이므로 MercenaryUI도 갱신
                    UpdateMercenaryUIForDungeonWall();
                    ArchitectureManager.Instance.walls.Add(this);

                    if (ArchitectureManager.Instance.NextSceneCheck())
                    {
                        EventManager.Instance.Publish(new NextMapEvent { triggerNum = (int)NextMap.A });
                        Debug.Log("A조건 충족 (LoadArchData)");
                    }
                }
                
                break;
            }
        }
        
        // var wallData = data.dungeonWalls.Find(w => w.archID == archData.archID);
        // if (wallData == null)
        //     return;
        //
        // gameObject.SetActive(!wallData.isUnlocked);
    }
    
    private void UpdateMercenaryUIForDungeonWall()
    {
        if (SceneManager.GetActiveScene().name == GameConstants.SceneNames.TUTORIAL_SCENE) return;
        
        var ui = UIManager.Instance.GetUI<MercenaryUI>();
        if (ui == null) return;

        foreach (var hunterSetUI in ui.hunterSetUIList)
        {
            if (hunterSetUI.targetItemData == archData.productData)
            {
                hunterSetUI.pmBtn.SetActive(true);
                hunterSetUI.lockObj.SetActive(false);
            }
        }
    }
}
