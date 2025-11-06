using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrderUI : UIBase
{
    public override bool isDestroy => true;
    
    public StageData stageData;
    public List<ItemSO> orderItems = new List<ItemSO>();
    public bool isOpen = false;
    public CanvasGroup canvasGroup;
    
    public TextMeshProUGUI diaText;
    private bool isFirstOpen = true;
    
    public override void OpenUI()
    {
        if(isFirstOpen)
        {
            orderItems.Add(stageData.orderItems[0].recipe[0]);
            isFirstOpen = false;
        }
        
        diaText.text = PlayerManager.Instance.Player.playerInventory.diamond.ToString();
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
}
