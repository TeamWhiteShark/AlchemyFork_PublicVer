using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCInven : MonoBehaviour
{
    [SerializeField] private NPC npc;
    [SerializeField] private LayerMask layerMask;
    
    [SerializeField] private int maxQuantity;
    public int MaxQuantity
    {
        get => maxQuantity;
        private set => maxQuantity = value;
    }
    
    [SerializeField] private int currentQuantity;
    public int CurrentQuantity
    {
        get => currentQuantity;
        private set => currentQuantity = value;
    }

    private float time;
    
    public void ResetInventory()
    {
        itemsDic[npc.homeItem] = 0;
        itemsDic[npc.awayItem] = 0;
        CurrentQuantity = 0;
    }
    
    public Dictionary<ItemSO, int> itemsDic= new Dictionary<ItemSO, int>();
    
    public Coroutine addItemCoroutine;
    public Coroutine getItemCoroutine;

    private void Start()
    {
        CurrentQuantity = 0;
    }


    private void CheckQuantity()
    {
        int quantity = 0;
        foreach (var itemInDic in itemsDic)
        {
            quantity += itemInDic.Value;
        }

        CurrentQuantity = quantity;
    }

    public IEnumerator AddItem(ItemSO item)
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

    public void RemoveItem(ItemSO requireItem)
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer)) && CurrentQuantity < MaxQuantity)
        {
            if(other.TryGetComponent(out Item item) && !item.get)
            {
                item.get = true;
                item.OnDespawn();
                StartCoroutine(ReleaseItemCoroutine(item.ItemData, item.transform.position, this.transform.position, 0.3f));
            }
        }
    }
    private IEnumerator ReleaseItemCoroutine(ItemSO itemData ,Vector3 startPos, Vector3 endPos, float duration)
    {
        yield return Utils.BezierMove(itemData.itemPrefab, startPos, endPos, duration);
        StartCoroutine(AddItem(itemData));
    }
}
