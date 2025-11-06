using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Ingredient,
    Product,
    Gold
}

[CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Item/ItemDatabase")]
public class ItemSO : PoolData
{
    [SerializeField] public ObjType objType;
    [SerializeField] public ItemType itemType;
    [SerializeField] public string itemName;
    [SerializeField] public string itemID;
    [SerializeField] public int itemPrice;
    [SerializeField] public Sprite itemSprite;
    [SerializeField] public Sprite inventorySprite;
    [SerializeField] public GameObject itemPrefab;
    [SerializeField] public int reword;

    [SerializeField] public List<ItemSO> recipe;
    [SerializeField] public List<int> ingredientQuantity;
}
