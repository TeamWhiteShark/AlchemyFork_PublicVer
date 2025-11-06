using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlotUI : MonoBehaviour
{
    public int price;
    [SerializeField] private Button itemButton;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Image icon;
    public ItemSO itemData;
    [SerializeField] private TMP_Text Time;

    private DateTime disabledTime;
    public DateTime DisabledTime => disabledTime;

    public Button Button => itemButton;
    public TMP_Text PriceText => priceText;
    public Image Icon => icon;
    public ItemSO ItemData => itemData;
    public GameObject Neg;
    public GameObject Nei;
    public ShopUI shopUI;

    public void Init(ShopUI shop)
    {
        shopUI = shop;
        icon.sprite = itemData.itemSprite;
        PriceText.text = ConvertPriceToUnit(price);
        Button.onClick.AddListener(BuyItem);
    }

    public void BuyItem()
    {
        var inventory = PlayerManager.Instance.Player.playerInventory;

        if (inventory.MaxQuantity - inventory.CurrentQuantity < 15)
        {
            shopUI.Nei.SetActive(true);
            return;
        }

        if (inventory.Money >= price)
        {
            inventory.Money -= price;

            for (int i = 0; i < 15; i++)
                inventory.StartCoroutine(inventory.AddItem(itemData, 1));

            // 쿨타임 10분 설정
            disabledTime = DateTime.Now.AddMinutes(10);
            Button.interactable = false;

            // 바로 남은 시간 표시
            TimeSpan remaining = disabledTime - DateTime.Now;
            Time.text = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
        }
        else
        {
            shopUI.Neg.SetActive(true);
        }

        Analytics.AddEvent("shop_buy_count", new Dictionary<string, object>
        {
            { "item_id", itemData.itemID }
        });
    }

    public void Update()
    {
        if (disabledTime > DateTime.Now)
        {
            TimeSpan remaining = disabledTime - DateTime.Now;
            int min = remaining.Minutes;
            int sec = remaining.Seconds;
            Time.text = $"{min:D2}:{sec:D2}";
            Button.interactable = false;
        }
        else
        {
            Time.text = "";
            Button.interactable = true;
        }
    }

    public void SetCooldown(string savedTimeSpanString)
    {
        if (TimeSpan.TryParse(savedTimeSpanString, out TimeSpan remainingTime))
        {
            if (remainingTime <= TimeSpan.Zero)
            {
                Button.interactable = true;
                disabledTime = DateTime.MinValue;
            }
            else
            {
                Button.interactable = false;
                disabledTime = DateTime.Now.Add(remainingTime);
            }
        }
        else
        {
            Button.interactable = true;
            disabledTime = DateTime.MinValue;
        }
    }

    public string ConvertPriceToUnit(int gold)
    {
        if (gold >= 1_000_000)
            return $"{gold / 1_000_000f:0.#}b";
        else if (gold >= 1_000)
            return $"{gold / 1_000f:0.#}a";
        else
            return $"{gold}";
    }
}
