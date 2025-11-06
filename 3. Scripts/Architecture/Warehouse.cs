using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Warehouse : BaseArchitecture
{
    [SerializeField] private Transform interactionPos;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image image;
    [SerializeField] private List<ItemSO> itemList;
    [SerializeField] private Image[] icons;
    [SerializeField] private List<TextMeshProUGUI> itemCountTexts = new List<TextMeshProUGUI>();
    [SerializeField] private Canvas canvas;
    [SerializeField] private ObjType objType;
    
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private RectTransform slotParent;

    private bool isReleasing;

    private Coroutine _removeNPCCoroutine;
    public int dicCount;
    
    public Coroutine addItemCoroutine;
    
    protected override void Start()
    {
        canvas.worldCamera = Camera.main;

        Init();
        
        if (SaveLoadManager.Instance.isClickedContinue)
        {
            LoadWarehouseData();
        }
        else
        {
            itemsDic.Clear();
            CurrentQuantity = 0;
        }
        
        CheckQuantity();
    }

    private void Init()
    {
        slotParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
            1.5f * ArchitectureManager.Instance.stageData.orderItems.Length / 2);
        foreach (var t in ArchitectureManager.Instance.stageData.orderItems)
        {
            if(t.objType == this.objType)
            {
                var slot = Instantiate(slotPrefab, slotParent).GetComponent<WarehouseSlot>();
                slot.Init(t.recipe[0], this);
                itemList.Add(t.recipe[0]);
                itemCountTexts.Add(slot.text);
            }
        }
    }


    protected override void Update()
    {
        base.Update();
        
        dicCount = itemsDic.Count;
        
        if (npc[0] == null && npcDict.TryGetValue(NPCType.Hunter, out var hunter) && hunter.Count != 0)
        {
            npc[0] = hunter[0];
            hunter.RemoveAt(0);
        }
        
        if (npc[1] == null && npcDict.TryGetValue(NPCType.Chef, out var chef) && chef.Count != 0)
        {
            npc[1] = chef[0];
            chef.RemoveAt(0);
        }

        if (npc[0] != null && npc[0].stateMachine.currentState == npc[0].stateMachine.NPCInteractState)
        {
            GetItem(npc[0]);
        }

        if (npc[1] != null && npc[1].stateMachine.currentState == npc[1].stateMachine.NPCInteractState)
        {
            ReleaseItem(npc[1]);
        }
    }

    private void LoadWarehouseData()
    {
        var data = SaveLoadManager.Instance.saveData;
        if (data == null)
        {
            Debug.LogError("세이브 데이터가 존재하지 않습니다.");
            return;
        }
        
        itemsDic.Clear();
        
        if(objType == ObjType.Mushroom)
        {
            foreach (var item in data.mushroomWarehouseItemCounts)
            {
                string itemID = item.Key;
                int itemCount = item.Value;

                // Resources 폴더에서 아이템 불러오기
                ItemSO itemSO = Resources.Load<ItemSO>($"{GameConstants.Paths.ITEM_DATA_PATH}{itemID}");

                if (itemSO != null && itemCount != 0)
                {
                    itemsDic[itemSO] = itemCount;
                }
                else
                {
                    Debug.LogWarning($"[로드 실패] 아이템 {itemID}를 Resources/{GameConstants.Paths.ITEM_DATA_PATH} 폴더에서 찾을 수 없습니다.");
                }
            }
        }
        else
        {
            foreach (var item in data.meatWarehouseItemCounts)
            {
                string itemID = item.Key;
                int itemCount = item.Value;

                // Resources 폴더에서 아이템 불러오기
                ItemSO itemSO = Resources.Load<ItemSO>($"{GameConstants.Paths.ITEM_DATA_PATH}{itemID}");

                if (itemSO != null && itemCount != 0)
                {
                    itemsDic[itemSO] = itemCount;
                }
                else
                {
                    Debug.LogWarning($"[로드 실패] 아이템 {itemID}를 Resources/{GameConstants.Paths.ITEM_DATA_PATH} 폴더에서 찾을 수 없습니다.");
                }
            }
        }
    }
    
    private void CheckQuantity()
    {
        int quantity = 0;
        foreach (var itemInDic in itemsDic)
        {
            quantity += itemInDic.Value;
        }

        for (int i = 0; i < itemCountTexts.Count; i++)
        {
            itemCountTexts[i].text = itemsDic.TryGetValue(itemList[i], out int value) ? value.ToString() : "0";  
        }

        CurrentQuantity = quantity;
        countText.text = CurrentQuantity.ToString();
    }

    private IEnumerator AddItem(ItemSO item)
    {
        try
        {
            if (itemsDic.ContainsKey(item))
            {
                itemsDic[item]++;
                CheckQuantity();
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                itemsDic[item] = 1;
                CheckQuantity();
                yield return new WaitForSeconds(0.1f);
            }
        }
        finally
        {
            addItemCoroutine = null;
        }
    }

    private void RemoveItem(ItemSO requireItem)
    {
        if (itemsDic == null) return;

        if (itemsDic.ContainsKey(requireItem))
        {
            itemsDic[requireItem]--;
            CheckQuantity();
            if (itemsDic[requireItem] <= 0)
            {
                itemsDic.Remove(requireItem);
            }
        }
    }
    
    public override void GetItem(NPC npc)
    {
        if (_removeNPCCoroutine == null)
        {
            _removeNPCCoroutine = StartCoroutine(RemoveItemCoroutine(npc));
        }
    }

    public override void ReleaseItem(NPC npc)
    {
        if (itemsDic.Count == 0) return;
        foreach (var item in itemList)
        {
            if (!itemsDic.TryGetValue(item, out int value) || value <= 0) continue;
            while (npc.npcInven.CurrentQuantity < npc.npcInven.MaxQuantity &&
                npc.npcInven.addItemCoroutine == null && _releaseItemCoroutine == null)
            {
                RemoveItem(item);
                CheckQuantity();
                _releaseItemCoroutine = StartCoroutine(ReleaseItemCoroutine(item.itemPrefab,
                    interactionPos.position, npc.transform, 0.3f));

                npc.npcInven.addItemCoroutine = StartCoroutine(npc.npcInven.AddItem(item));
            }

            if (npc.npcInven.CurrentQuantity == npc.npcInven.MaxQuantity) return;
        }
    }
    
    private IEnumerator RemoveItemCoroutine(NPC npc)
    {
        try
        {
            if (npc != null && npc.npcInven.itemsDic.Count > 0 && CurrentQuantity < MaxQuantity)
            {
                foreach (var itemSo in itemList)
                {
                    while (npc.npcInven.itemsDic.ContainsKey(itemSo) && npc.npcInven.itemsDic[itemSo] > 0)
                    {
                        npc.npcInven.RemoveItem(itemSo);
                        yield return Utils.BezierMove(itemSo.itemPrefab, npc.transform.position, interactionPos, 0.1f);
                        yield return AddItem(itemSo);
                    }
                }
            }
        }
        finally
        {
            _removeNPCCoroutine = null;
        }
    }

    public void ReleaseItem(ItemSO itemSo)
    {
        if (!itemsDic.ContainsKey(itemSo)) return;
        if (isReleasing) return;
        StartCoroutine(ReleaseItemCoroutine(itemSo));
    }

    private IEnumerator ReleaseItemCoroutine(ItemSO releaseItem)
    {
        isReleasing = true;
        while (itemsDic.ContainsKey(releaseItem) && itemsDic[releaseItem] != 0 && playerInven.CurrentQuantity < playerInven.MaxQuantity)
        {
            RemoveItem(releaseItem);
            yield return Utils.BezierMove(releaseItem.itemPrefab, interactionPos.position, playerInven.transform, 0.1f);
            yield return playerInven.AddItem(releaseItem, 1);
        }

        isReleasing = false;
        if (!playerIn) playerInven = null;
    }

    protected override void InitializeSpecificValue(ArchDataSO archData)
    {
        MaxQuantity = archData.productPrice;
        currentValue = MaxQuantity;
        nextValue = MaxQuantity + productUpgradeMultiplier;
    }

    protected override void ApplySingleUpgradeFormulas()
    {
        // Warehouse: 보관 수량 증가
        MaxQuantity += (int)productUpgradeMultiplier;
        upgradePrice = new BigInteger(Math.Round((decimal)upgradePrice * (decimal)(upgradeMultiplier + GameConstants.Math.PERCENTAGE_BASE) / GameConstants.Math.PERCENTAGE_BASE));
        currentValue = MaxQuantity;
        nextValue = MaxQuantity + productUpgradeMultiplier;
        upgradeMultiplier *= (GameConstants.Math.PERCENTAGE_BASE - productUpgradeMultiplierChangeRate) / GameConstants.Math.PERCENTAGE_BASE;
    }

    protected override bool CanUpgrade()
    {
        if (ArchitectureManager.Instance.cooks.Count == 0)
        {
            UIManager.Instance.OpenUI<WarningUI1>();
            return false;
        }

        if (ArchitectureManager.Instance.stands.Count == 0)
        {
            UIManager.Instance.OpenUI<WarningUI>();
            return false;
        }
        
        return true;
    }
}
