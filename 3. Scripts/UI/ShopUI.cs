using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShopUI : UIBase
{
    public override bool isDestroy => false;    

    // 판매할 아이템 목록
    [SerializeField] private List<int> sellPrices = new List<int>();
    [SerializeField] public List<ShopItemSlotUI> itemSlots = new List<ShopItemSlotUI>();

    // 용병 버튼/가격/UI
    [SerializeField] private List<ShopMercenarySlotUI> mercenarySlot = new List<ShopMercenarySlotUI>();    
    [SerializeField] private List<int> mercenaryPrices = new List<int>();

    public GameObject Neg;
    public GameObject Nei;

    private void Start()
    {
        // 상점 아이템 등록
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if(SceneManager.GetActiveScene().name == "MainGameScene")
            {
                var slot = itemSlots[i];
                if (slot.ItemData != null)
                {
                    slot.price = sellPrices[i];
                    slot.Init(this);
                }
                itemSlots[4].gameObject.SetActive(false);
                itemSlots[5].gameObject.SetActive(false);
            }
            else
            {
                var slot = itemSlots[i];
                ChangeData();
                ChangePrice();
                if (slot.ItemData != null)
                {                    
                    slot.Init(this);
                }                       
                itemSlots[4].gameObject.SetActive(true);
                itemSlots[5].gameObject.SetActive(true);                
            }
        }
        
        // 용병 UI 초기화
        for (int i = 0; i < mercenarySlot.Count; i++)
        {
            var Mslot = mercenarySlot[i];
            if (Mslot.priceText != null)
            {
                Mslot.price = mercenaryPrices[i];
                Mslot.Init(this);
            }            
            Mslot.UpdateCurrentCountUI();
        }

        if (SaveLoadManager.Instance.isClickedContinue)
        {
            LoadShopData();
        }
    }

    private void LoadShopData()
    {
        var data = SaveLoadManager.Instance.saveData;
        if (data == null)
        {
            Debug.LogError("세이브 데이터가 존재하지 않습니다.");
            return;
        }

        foreach (var slot in itemSlots)
        {
            if (slot.ItemData == null)
                continue;

            string key = slot.ItemData.itemName;

            if (data.shopItemCooltime.TryGetValue(key, out string savedTime))
            {
                // 바로 문자열을 넘겨서 TimeSpan 파싱 처리
                slot.SetCooldown(savedTime);
            }
        }
    }
    
    public override void OpenUI()
    {
        base.OpenUI();
        UIManager.Instance.isUIOn = true;
    }
    
    public override void CloseUI()
    {
        base.CloseUI();
        CloseNeg();
        CloseNei();
        UIManager.Instance.isUIOn = false;
    }
    
    public void CloseShop() => CloseUI();
    public void CloseNeg()
    {
        Neg.SetActive(false);
    }
    public void CloseNei()
    {
        Nei.SetActive(false);
    }

    private void ChangeData()
    {
        itemSlots[0].itemData = Resources.Load<ItemSO>("Data/Item/1002");
        itemSlots[1].itemData = Resources.Load<ItemSO>("Data/Item/1007");
        itemSlots[2].itemData = Resources.Load<ItemSO>("Data/Item/1003");
        itemSlots[3].itemData = Resources.Load<ItemSO>("Data/Item/1005");
        itemSlots[4].itemData = Resources.Load<ItemSO>("Data/Item/1008");
        itemSlots[5].itemData = Resources.Load<ItemSO>("Data/Item/1006");
    }

    private void ChangePrice()
    {
        itemSlots[0].price = 2500;
        itemSlots[1].price = 3500;
        itemSlots[2].price = 4500;
        itemSlots[3].price = 6000;
        itemSlots[4].price = 7500;
        itemSlots[5].price = 9000;
        itemSlots[0].PriceText.text = itemSlots[0].ConvertPriceToUnit(itemSlots[0].price);
        itemSlots[1].PriceText.text = itemSlots[1].ConvertPriceToUnit(itemSlots[1].price);
        itemSlots[2].PriceText.text = itemSlots[2].ConvertPriceToUnit(itemSlots[2].price);
        itemSlots[3].PriceText.text = itemSlots[3].ConvertPriceToUnit(itemSlots[3].price);
        itemSlots[4].PriceText.text = itemSlots[4].ConvertPriceToUnit(itemSlots[4].price);
        itemSlots[5].PriceText.text = itemSlots[5].ConvertPriceToUnit(itemSlots[5].price);
    }
}