using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopMercenarySlotUI : MonoBehaviour
{
    public NPCType npcType;    
    public TMP_Text priceText;
    public TMP_Text currentCountText;
    public TMP_Text maxCountText;
    public Button button;
    public int price;
    public int currentCount;
    public int MaxCount;
    public GameObject Neg;
    public ShopUI shopUI;
    
    public void Init(ShopUI shop)
    {        
        shopUI = shop;
        button.onClick.AddListener(BuyMercenary);
        priceText.text = ConvertPriceToUnit(price);
        currentCountText.text = currentCount.ToString();
        maxCountText.text = MaxCount.ToString();    
        if(SceneManager.GetActiveScene().name == "SecondMainGameScene")
        {
            if (npcType == NPCType.Cashier)
            {
                MaxCount = 2;
                maxCountText.text = MaxCount.ToString();
            }
            else
            {
                MaxCount = 6;
                maxCountText.text = MaxCount.ToString();
            }
        }
    }
    
    private void BuyMercenary()
    {
        var inventory = PlayerManager.Instance.Player.playerInventory;
        int currentCount = NPCManager.Instance.GetNPCCount(npcType);

        if (currentCount >= MaxCount)
            return;
        
        if (inventory.Money < price)
        {
            shopUI.Neg.SetActive(true);
            return;
        }
        inventory.Money -= price;
        NPCManager.Instance.CreateNPC(NPCManager.Instance.waitingPoint.transform.position, npcType);

        price *= 2;
        priceText.text = ConvertPriceToUnit(price);

        UpdateCurrentCountUI();

        Analytics.AddEvent("shop_buy_count", new Dictionary<string, object>
        {
            { "npc_type", npcType.ToString() }
        });
    }
    
    public void UpdateCurrentCountUI()
    {
        currentCount = NPCManager.Instance.npc.ContainsKey(npcType) ? NPCManager.Instance.GetNPCCount(npcType) : 0; 
        currentCountText.text = currentCount.ToString();

        button.interactable = currentCount < MaxCount;
    }
    
    private string ConvertPriceToUnit(int gold)
    {
        if (gold >= 1_000_000)
            return $"{gold / 1_000_000f:0.#}b";
        else if (gold >= 1_000)
            return $"{gold / 1_000f:0.#}a";
        else
            return $"{gold}";
    }   
}
