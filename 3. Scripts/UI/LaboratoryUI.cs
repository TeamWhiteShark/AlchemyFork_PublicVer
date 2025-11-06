using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Numerics;
using System;
using UnityEngine.SceneManagement;

public class LaboratoryUI : UIBase
{
    public override bool isDestroy => false;

    public bool isOpen = false;
    public CanvasGroup canvasGroup;
    
    public Player player;
    public PlayerCondition playerCondition;
    public PlayerInventory playerInventory;
    [SerializeField] public LaboratorySlot[] slots;

    public void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Start()
    {
        player = PlayerManager.Instance.Player;
        playerCondition = player.playerCondition;
        playerInventory = player.playerInventory;
        
        if (SaveLoadManager.Instance.isClickedContinue)
        {
            LoadLaboratoryData();
        }
    }

    private void LoadLaboratoryData()
    {
        var data = SaveLoadManager.Instance.saveData;
        if (data == null)
        {
            Debug.LogError("세이브 데이터가 존재하지 않습니다.");
            return;
        }
    }

    public override void OpenUI()
    {
        isOpen = true;
        UIManager.Instance.isUIOn = true;
        ToggleUI();
    }

    public override void CloseUI()
    {
        isOpen = false;
        UIManager.Instance.isUIOn = false;
        ToggleUI();
    }

    private void ToggleUI()
    {
        if (isOpen)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
    
    public void OnClickCloseButton()
    {
        CloseUI();
    }

    public bool AllMaxLevelReached()
    {
        foreach (var slot in slots)
        {
            if(SceneManager.GetActiveScene().name == "MainGameScene")
            {
                if (slot.lv < 5)
                {
                    return false;
                }
            }
            else
            {
                if (slot.statUpgrader is MonsterSpawnUpgrader)
                {
                    if(slot.lv < 5)
                    {
                        return false;
                    }                    
                }
                else
                {
                    if (slot.lv < 10)
                    {
                        return false;
                    }
                }
            }
        }
        return true;        
    }
}
