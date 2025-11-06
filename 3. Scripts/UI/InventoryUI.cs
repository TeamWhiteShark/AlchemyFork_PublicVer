using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : UIBase
{
    public override bool isDestroy => false;

    [SerializeField] private List<ItemSlotUI> itemSlots = new List<ItemSlotUI>();
    [SerializeField] private List<ItemSO> items = new List<ItemSO>();
    [SerializeField] private GameObject itemSlotUIPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private ItemSlotUI moneyUI;
    [SerializeField] private ItemSlotUI diamondUI;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GameObject savingPanel;
    [SerializeField] private CanvasGroup savingPanelCanvasGroup;
    [SerializeField] private TextMeshProUGUI inventoryQuantityText;
    [SerializeField] private GameObject tutorialIndicator;
    [SerializeField] private TextMeshProUGUI indicatorText;

    public List<GameObject> buttons = new List<GameObject>();
    
    private Coroutine timerCoroutine;
    private WaitForSeconds turnOffTimer = new WaitForSeconds(GameConstants.UI.SAVE_PANEL_TURN_OFF_TIME);
    private float duration = GameConstants.UI.SAVE_PANEL_DURATION;
    
    private void Update()
    {
        if (playerInventory == null)
        {
            playerInventory = PlayerManager.Instance.Player.playerInventory;
        }
        
        UpdateMoney();
        UpdateDiamond();
        UpdateItemSlot();
        UpdateMaxQuantity();
    }

    private void UpdateMaxQuantity()
    {
        inventoryQuantityText.text =
            playerInventory.CurrentQuantity.ToString() + " / " + playerInventory.MaxQuantity.ToString();
    }
    
    private void UpdateMoney()
    {
        moneyUI.quantityText.text = Utils.MoneyFormat(playerInventory.Money);
    }
    
    private void UpdateDiamond()
    {
        diamondUI.quantityText.text = playerInventory.diamond.ToString();
    }
    
    private void UpdateItemSlot()
    {
        if (playerInventory.itemsDic != null)
        {
            foreach (var item in playerInventory.itemsDic)
            {
                items.Add(item.Key);
            }

            // if (playerInventory.itemsDic.Count > itemSlots.Count)
            // {
            //     itemSlots.Add(Instantiate(itemSlotUIPrefab, content).GetComponent<ItemSlotUI>());
            // }
            
            while (itemSlots.Count < playerInventory.itemsDic.Count)
            {
                var slot = Instantiate(itemSlotUIPrefab, content).GetComponent<ItemSlotUI>();
                itemSlots.Add(slot);
            }
            
            for (int i = 0; i < playerInventory.itemsDic.Count; i++)
            {
                itemSlots[i].gameObject.SetActive(true);
                itemSlots[i].icon.sprite = items[i].inventorySprite;
                itemSlots[i].quantityText.text = playerInventory.itemsDic[items[i]].ToString();
            }

            for (int i = playerInventory.itemsDic.Count; i < itemSlots.Count; i++)
            {
                itemSlots[i].gameObject.SetActive(false);
            }
            items.Clear();
        }
    }

    private void OpenMercenaryUI()
    {
        if (UIManager.Instance.isUIOn == false)
        {
            UIManager.Instance.OpenUI<MercenaryUI>();
        }
    }

    public void OpenSettingUI()
    {
        if (UIManager.Instance.isUIOn == false)
        {
            UIManager.Instance.OpenUI<SetPanelUI>();
        }
    }

    public void OpenTutorialUI()
    {
        if (UIManager.Instance.isUIOn == false)
        {
            UIManager.Instance.OpenUI<TutorialUI>();
        }
    }

    private void OpenQuestUI()
    {
        if (UIManager.Instance.isUIOn == false)
        {
            UIManager.Instance.OpenUI<QuestUI>();
        }
    }

    private void OpenLaboratoryUI()
    {
        if (UIManager.Instance.isUIOn == false)
        {
            UIManager.Instance.OpenUI<LaboratoryUI>();
        }
    }

    public void TurnOnSavingPanel()
    {
        savingPanel.SetActive(true);
        savingPanelCanvasGroup.alpha = 1f;
        
        timerCoroutine = StartCoroutine(ExitCoroutine());
    }

    private void OpenShopUI()
    {
        if (UIManager.Instance.isUIOn == false)
        {
            UIManager.Instance.OpenUI<ShopUI>();
        }        
    }

    private void OpenPetUI()
    {
        if (UIManager.Instance.isUIOn == false)
        {
            UIManager.Instance.OpenUI<PetUI>();
        }
    }

    private void OpenSecondMainScene()
    {
        if (UIManager.Instance.isUIOn == false)
        {
            UIManager.Instance.OpenUI<MapUI>();
        }
    }

    private void OpenOrderUI()
    {
        if(UIManager.Instance.isUIOn == false)
        {
            UIManager.Instance.OpenUI<OrderUI>();
        }
    }

    private IEnumerator ExitCoroutine()
    {
        yield return turnOffTimer;
        
        float startAlpha = savingPanelCanvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            savingPanelCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            yield return null;
        }

        savingPanelCanvasGroup.alpha = 0f;
        savingPanel.SetActive(false);
    }

    public void UnLockButtons()
    {
        foreach (var btn in buttons)
        {
            if (btn.name == "MapButton")
                continue;
            btn.GetComponent<InventoryButton>().UnlockFunctionOfButton();
            btn.GetComponent<Button>().onClick.AddListener(() => HandleButton(btn.name));
        }
    }

    private void HandleButton(string btnName)
    {
        switch (btnName)
        {
            case "QuestButton":
                OpenQuestUI();
                break;
            case "LaboratoryButton":
                OpenLaboratoryUI();
                break;
            case "ShopButton":
                OpenShopUI();
                break;
            case "PetButton":
                OpenPetUI();
                break;
            case "MercenaryButton":
                OpenMercenaryUI();
                break;
            case "MapButton":
                OpenSecondMainScene();
                break;
            case "OrderButton":
                OpenOrderUI();
                break;
        }
    }

    public void HandleIndicator(bool active)
    {
        tutorialIndicator.SetActive(active);
    }

    public void HandleIndicatorText(string currentText)
    {
        indicatorText.text = currentText;
    }
}
