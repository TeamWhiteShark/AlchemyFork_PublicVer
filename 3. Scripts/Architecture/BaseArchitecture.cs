using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public abstract class BaseArchitecture : MonoBehaviour
{
    public GameObject CountUI;
    public bool playerIn;
    
    public PlayerInventory playerInven;
    public Customer customer;
    public NPC[] npc; // 0. Hunter 1. Chef 2. Waiter 3. Cashier
    
    public InteractionUI interactionUI;
    public bool canCalculate;
    public bool canUpgrade;
    public int searchPoint;
    
    [Header("Arch Info")]
    public int archID;
    public string archName;
    public string productName;
    public string upgradeType;
    public int unlockMoney;
    public BigInteger upgradePrice;
    public float upgradeMultiplier;
    public float upgradeMultiplierChangeRate;
    public BigInteger productPrice;
    public float productUpgradeMultiplier;
    public float productUpgradeMultiplierChangeRate;
    public float productTime;
    public int maxLevel;
    public ArchType archType;
    
    public ItemSO ingredientData;
    public ItemSO productData;
    public int productCount;
    
    public int releaseAmount;
    public int repeat;
    
    [Header("UpgradeInfo")]
    public int upgradeLevel = 1;
    public float currentValue;
    public float nextValue;
    
    public List<Customer> customerList = new List<Customer>();
    public Dictionary<NPCType, List<NPC>> npcDict = new Dictionary<NPCType, List<NPC>>();
    public Dictionary<ItemSO, int> itemsDic= new Dictionary<ItemSO, int>();
    
    private float time;
    public float upgradeWaitTime;
    public float pTime;
    protected Coroutine _releaseItemCoroutine;
    private Coroutine getCustomerCoroutine;
    private Coroutine upgradeWaitCoroutine;
    
    [SerializeField] private int maxQuantity;
    public int MaxQuantity
    {
        get => maxQuantity;
        protected set => maxQuantity = value;
    }
    
    [SerializeField] private int currentQuantity;
    public int CurrentQuantity
    {
        get => currentQuantity;
        set => currentQuantity = value;
    }

    public void Init(ArchDataSO archData)
    {
        archID = archData.archID;
        archName = archData.archName;
        archType = archData.archType;
        
        productData = archData.productData;
        productName = archData.productData.itemName;
        productPrice = archData.productPrice;
        productTime = archData.productTime;
        
        upgradeType = archData.upgradeType;
        upgradePrice = archData.upgradePrice;
        upgradeMultiplier = archData.upgradeMultiplier;
        upgradeMultiplierChangeRate = archData.upgradeMultiplierChangeRate;
        productUpgradeMultiplier = archData.productUpgradeMultiplier;
        productUpgradeMultiplierChangeRate = archData.productUpgradeMultiplierChangeRate;
        maxLevel = archData.maxLevel;

        upgradeLevel = GameConstants.Architecture.MIN_UPGRADE_LEVEL;
        
        npcDict[NPCType.Cashier] = new List<NPC>();
        npcDict[NPCType.Chef] = new List<NPC>();
        npcDict[NPCType.Hunter] = new List<NPC>();
        npcDict[NPCType.Waiter] = new List<NPC>();

        // 세이브 데이터에서 해당 건축물의 업그레이드 레벨 로드
        var saveData = SaveLoadManager.Instance.saveData;
        if (saveData != null && saveData.unlockedArchs != null)
        {
            var savedArch = saveData.unlockedArchs.Find(a => a.archID == archData.archID);
            if (savedArch != null)
            {
                if(archType == ArchType.Warehouse)
                    upgradeLevel = Mathf.Clamp(savedArch.currentLevel, GameConstants.Architecture.MIN_UPGRADE_LEVEL, maxLevel);
                else
                    upgradeLevel = savedArch.currentLevel;
            }
        }
        
        InitializeSpecificValue(archData);
        ApplyUpgradeLevel();
        if (upgradeLevel >= GameConstants.Architecture.SPECIAL_UPGRADE_LEVEL && archType == ArchType.Cook)
            ArchitectureManager.Instance.cookConditions.TryAdd(productData, true);
    }
    
    protected abstract void InitializeSpecificValue(ArchDataSO archData);
    
    private void ApplyUpgradeLevel()
    {
        // 레벨 1이면 초기 상태이므로 리턴
        if (upgradeLevel <= GameConstants.Architecture.MIN_UPGRADE_LEVEL)
            return;

        for (int i = GameConstants.Architecture.MIN_UPGRADE_LEVEL; i < upgradeLevel; i++)
        {
            ApplySingleUpgradeFormulas();
        }
    }

    protected abstract void ApplySingleUpgradeFormulas();
    
    public bool Upgrade()
    {
        if (playerInven.Money < upgradePrice) return false;
        if (maxLevel != 0 && upgradeLevel >= maxLevel) return false;
        
        if(!CanUpgrade()) return false;
        
        playerInven.Money -= upgradePrice;
        ApplySingleUpgradeFormulas();
        
        upgradeLevel++;

        interactionUI.ResetUI();

        OnUpgrade();
        
        interactionUI.ResetUI();
        EventManager.Instance.Publish(new ArchitectureUpgradedEvent { Architecture = this });
        
        return true;
    }
    
    protected virtual bool CanUpgrade(){ return true; }
    protected virtual void OnUpgrade() { }


    protected virtual void Awake()
    {
        npc = new NPC[4];
        upgradeWaitTime = GameConstants.Architecture.UPGRADE_WAIT_TIME;
    }
    
    protected virtual void Start() { }

    protected virtual void Update()
    {
        if (customerList.Count > 0 && customer == null)
        {
            customer = customerList[0];
            customerList.RemoveAt(0);
            customer.customerInven.itemCalCount = customer.customerInven.itemCount;
        }
        
        if(canUpgrade && upgradeWaitCoroutine == null)
            Upgrade();
    }

    public virtual void GetItem() { }
    public virtual void GetItem(NPC npc) { }
    public virtual void Produce() { }
    public virtual void ReleaseItem() { }
    public virtual void ReleaseItem(NPC npc) { }
    
    protected IEnumerator ReleaseItemCoroutine(GameObject prefab, Vector3 startPos, Transform endTransform, float duration)
    {
        try
        {
            yield return Utils.BezierMove(prefab, startPos, endTransform, duration);
        }
        finally
        {
            _releaseItemCoroutine = null;
        }
    }

    public virtual void CheckCustomer(Customer _customer) { }
}
