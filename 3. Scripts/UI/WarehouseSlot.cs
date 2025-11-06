using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarehouseSlot : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    public TextMeshProUGUI text;

    public void Init(ItemSO item, Warehouse arch)
    {
        icon.sprite = item.itemSprite;
        button.onClick.AddListener(() => arch.ReleaseItem(item));
    }
}
