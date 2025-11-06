using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HunterSetUI : MonoBehaviour
{
    public MercenaryUI mercenaryUI;
    public TextMeshProUGUI dungeonNameText;
    public TextMeshProUGUI hunterCountText;
    public ItemSO targetItemData;
    public Button minusBtn;
    public Button plusBtn;
    
    public GameObject pmBtn;
    public GameObject lockObj;

    public void ResetUI()
    {
        hunterCountText.text = mercenaryUI.hunterDict[targetItemData].Count.ToString();
    }
}
