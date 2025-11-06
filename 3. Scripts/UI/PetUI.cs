using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetUI : UIBase
{
    public override bool isDestroy => false;

    [SerializeField] private List<PetSlotUI> petSlots = new List<PetSlotUI>();

    public GameObject Neg;

    private void Start()
    {
        for (int i = 0; i < petSlots.Count; i++)
        {
            var slot = petSlots[i];
            slot.Init(this);
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
        UIManager.Instance.isUIOn = false;
    }
    public void CloseNeg()
    {
        Neg.SetActive(false);
    }
}
