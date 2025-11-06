using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using TMPro;

public class Stand : BaseArchitecture
{
    
    private Coroutine _getCoroutine;
    private Coroutine _getNPCCoroutine;
    private Coroutine _removeCoroutine;
    [SerializeField] List<GameObject> products = new List<GameObject>();
    private int bottleIdx;
    
    protected override void Start()
    {
        ArchitectureManager.Instance.standConditions.Add(productData, true);
        //CustomerManager.Instance.PlusMaxSpawnCount(5);

        foreach (var item in products)
        {
            var sprite = item.GetComponent<SpriteRenderer>();
            sprite.sprite = productData.itemSprite;
        }
    }
    
    protected override void Update()
    {
        base.Update();
        if (npc[2] == null && npcDict.TryGetValue(NPCType.Waiter, out var value) && value.Count != 0)
        {
            npc[2]= value[0];
            value.RemoveAt(0);
        }
        
        if (playerInven != null)
        {
            GetItem();
        }

        if (npc[2] != null && npc[2].stateMachine.currentState == npc[2].stateMachine.NPCInteractState)
        {
            GetNPCItem();
        }

        if (customer != null && customer.stateMachine.currentState == customer.stateMachine.NPCInteractState)
        {
            ReleaseItem();
        }
    }

    public override void GetItem()
    {
        if (playerInven.itemsDic.ContainsKey(productData) && playerInven.itemsDic[productData] != 0 && _getCoroutine == null)
        {
            _getCoroutine = StartCoroutine(GetItemCoroutine());
            if (playerInven == null) return;
        }
    }

    public void GetNPCItem()
    {
        if (npc[2].npcInven.itemsDic.ContainsKey(productData) && npc[2].npcInven.itemsDic[productData] != 0 && _getNPCCoroutine == null)
        {
            _getNPCCoroutine = StartCoroutine(GetNPCItemCoroutine());
            if (npc == null) return;
        }
    }
    
    private IEnumerator GetNPCItemCoroutine()
    {
        try
        {
            npc[2].npcInven.RemoveItem(productData);
            bottleIdx = Mathf.Min(productCount, 15);
            yield return Utils.BezierMove(productData.itemPrefab, npc[2].transform.position, products[bottleIdx].transform, 0.1f);
            products[bottleIdx].SetActive(true);
            productCount++;
        }
        finally
        {
            _getNPCCoroutine = null;
        }
    }

    public override void ReleaseItem()
    {
        if (productCount > 0 && customer.customerInven.itemCount < customer.targetCount && productData == customer.targetItem && _removeCoroutine == null)
        {
            if (customer == null) return;
            _removeCoroutine = StartCoroutine(RemoveItemCoroutine());
        }
    }
    private IEnumerator GetItemCoroutine()
    {
        try
        {
            playerInven.RemoveItem(productData);
            bottleIdx = Mathf.Min(productCount, 15);
            yield return Utils.BezierMove(productData.itemPrefab, playerInven.transform.position, products[bottleIdx].transform, 0.1f);
            products[bottleIdx].SetActive(true);
            productCount++;
        }
        finally
        {
            _getCoroutine = null;
        }
    }
    
    private IEnumerator RemoveItemCoroutine()
    {
        try
        {
            productCount--;
            bottleIdx = Mathf.Min(productCount, 15);
            products[bottleIdx].SetActive(false);
            bottleIdx = Mathf.Min(productCount, 15);
            yield return Utils.BezierMove(productData.itemPrefab, products[bottleIdx].transform.position, customer.transform, 0.1f);
            customer.customerInven.itemCount++;
        }
        finally
        {
            _removeCoroutine = null;
        }
    }

    public override void CheckCustomer(Customer _customer)
    {
        if (!_customer.satisfied && _customer.targetItem == productData)
        {
            customerList.Add(_customer);
        }
    }

    protected override void InitializeSpecificValue(ArchDataSO archData)
    {
    }

    protected override void ApplySingleUpgradeFormulas()
    {
    }
}
