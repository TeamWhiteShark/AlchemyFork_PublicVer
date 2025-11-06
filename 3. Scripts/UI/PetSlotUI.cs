using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PetSlotUI : MonoBehaviour
{
    public int price;
    public Button itemButton;
    public TMP_Text functionText;
    public TMP_Text petName;
    public TMP_Text priceText;
    public TMP_Text limitTimeText;
    public Image icon;
    public PetSO petSO;
    public PetUI petUI;

    private GameObject currentPet;

    private float durationTime = 1800f; // 30분
    private bool isTimerActive = false;

    private DateTime startTime;    // 시작 시각 저장
    private float extraTime = 0f;  // 추가 구매 시 누적 시간

    public void Init(PetUI pet)
    {
        petUI = pet;
        isTimerActive = false;
        limitTimeText.enabled = false;
        itemButton.onClick.AddListener(BuyPet);
        priceText.text = price.ToString();
        icon.sprite = petSO.PetImage;
        petName.text = petSO.PetName;
        functionText.text = petSO.PetInfo;
        petSO.DurationTime = durationTime;
    }

    public void Update()
    {
        if (!isTimerActive) return;

        double elapsed = (DateTime.Now - startTime).TotalSeconds; // 경과 시간
        double remaining = durationTime + extraTime - elapsed;    // 남은 시간

        if (remaining <= 0)
        {
            isTimerActive = false;
            limitTimeText.text = "00:00";
            if (currentPet != null)
            {                
                petSO.StartTime = DateTime.MinValue;
                currentPet = null;                
            }
            return;
        }

        int min = Mathf.FloorToInt((float)remaining / 60);
        int sec = Mathf.FloorToInt((float)remaining % 60);
        limitTimeText.text = $"{min:D2}:{sec:D2}";
    }

    public void BuyPet()
    {
        var inventory = PlayerManager.Instance.Player.playerInventory;        

        if (inventory.Money >= price)
        {
            inventory.Money -= price;
            if (!isTimerActive)
            {
                startTime = DateTime.Now;  // 현재 시각 저장
                petSO.StartTime = startTime;
                isTimerActive = true;
                extraTime = 0f;
                currentPet = Instantiate(petSO.PetPrefab, PlayerManager.Instance.Player.transform);
                PetActive();
            }
            else
            {
                extraTime += 1800f; // 추가 구매 시 30분 누적
            }
            petSO.ExtraTime = extraTime;            
            limitTimeText.enabled = true;
        }
        else
        {
            petUI.Neg.SetActive(true);
        }
    }
    public void PetActive()
    {
        var Player = PlayerManager.Instance.Player;
        Player.moveSpeed += petSO.PetSpeed;
        Player.playerInventory.MaxQuantity += petSO.PetInven;
    }
   
}
