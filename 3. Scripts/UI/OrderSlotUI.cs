using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class OrderSlotUI : MonoBehaviour
{
    [SerializeField] OrderUI orderUI;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] private TextMeshProUGUI rewordText;
    [SerializeField] private GameObject slot;
    [SerializeField] private GameObject timer;
    [SerializeField] private TextMeshProUGUI timerText;
    
    [SerializeField] private float orderWaitTime;
    
    
    private float curTimer = 0f;

    private ItemSO orderItem;
    private int itemCount;
    private int orderCount;
    private int rewordCount;

    public bool isClear;
    public bool isFirstOrder = true;

    private void Update()
    {
        if (!isClear && orderUI.orderItems.Count != 0 && isFirstOrder)
        {
            CreateOrder();
        }
        
        if (isClear)
        {
            WaitingTimeCheck();
        }
    }

    private void InitSlot(ItemSO item, int count)
    {
        isClear = false;
        orderItem = item;
        itemCount = 0;
        orderCount = count;
        itemImage.sprite = item.itemSprite;
        itemCountText.text = $"0/{count}";
        switch (orderCount)
        {
            case 3:
                rewordCount = item.reword;
                break;
            case 5:
                rewordCount = item.reword * 3;
                break;
            case 7:
                rewordCount = item.reword * 9;
                break;
        }
        
        rewordText.text = rewordCount.ToString();
    }
    
    private void ClearSlot()
    {
        isClear = true;
        orderItem = null;
        itemImage.sprite = null;
        itemCountText.text = "";
    }
    
    private void WaitingTimeCheck()
    {
        curTimer += Time.deltaTime;
        float remainTime = Mathf.Floor(orderWaitTime - curTimer);

        if (remainTime < 0)
        {
            remainTime = 0;
            curTimer = 0f;
            CreateOrder();
        }

        int minutes = (int)(remainTime / 60);
        int seconds = (int)(remainTime % 60);
        
        timerText.text = minutes.ToString() + ":" + seconds.ToString();
    }
    
    public void GetOrderedItems()
    {
        if(!PlayerManager.Instance.Player.playerInventory.itemsDic.ContainsKey(orderItem)) return;
        
        while(itemCount < orderCount)
        {
            PlayerManager.Instance.Player.playerInventory.RemoveItem(orderItem);
            itemCount++;
            UpdateUI();
            if (!PlayerManager.Instance.Player.playerInventory.itemsDic.ContainsKey(orderItem))
                break;
        }

        if (itemCount == orderCount)
        {
            PlayerManager.Instance.Player.playerInventory.diamond += rewordCount;

            UpdateDia();
            ClearSlot();
            slot.SetActive(false);
            timer.SetActive(true);
        }
    }

    private void UpdateDia()
    {
        orderUI.diaText.text = PlayerManager.Instance.Player.playerInventory.diamond.ToString();
    }
    
    private void UpdateUI()
    {
        itemCountText.text = $"{itemCount}/{orderCount}";
    }
    
    private void CreateOrder()
    {
        if(orderUI == null)
        {
            Debug.LogError("orderUI is null");
            return;
        }
        
        if(isFirstOrder)
            isFirstOrder = false;

        var order = orderUI.orderItems[Random.Range(0, orderUI.orderItems.Count)];
        var count = 0;
        var num = Random.Range(0, 100);
        if (num < 50)
        {
            count = 3;
        }
        else if (num < 80)
        {
            count = 5;
        }
        else
        {
            count = 7;
        }

        slot.SetActive(true);
        timer.SetActive(false);
        InitSlot(order, count);
    }

    public void RefuseBtn()
    {
        ClearSlot();
        slot.SetActive(false);
        timer.SetActive(true);
    }
}
