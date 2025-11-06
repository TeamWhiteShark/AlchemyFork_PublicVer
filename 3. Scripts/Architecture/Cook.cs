using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Cook : BaseArchitecture
{
    [SerializeField] private Animator animator;
    [SerializeField] private readonly int isCooking = Animator.StringToHash("IsCooking");

    [SerializeField] private Transform ingrediantPos;

    [SerializeField] private GameObject productPrefab;
    [SerializeField] private Transform productSpawnPos; 
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private int ingredientCount;
    private int bottleIdx;

    private int _recipeID;

    private float _remainTime;
    private bool _canMake = true;
    
    [SerializeField] List<GameObject> products = new List<GameObject>();
    [SerializeField] private TextMeshProUGUI ingredientCountText;
    [SerializeField] private Image frontImage;

    [SerializeField] private SpriteRenderer ingredientImage;
    [SerializeField] private SpriteRenderer productImage;

    [SerializeField] private AudioClip CookSound;

    private Coroutine _removeCoroutine;
    private Coroutine _removeNPCCoroutine;

    protected override void Start()
    {
        ingredientData = productData.recipe[0];
        ingredientCount = 0;
        ingredientPrefab = productData.recipe[0].itemPrefab;

        ingredientImage.sprite = ingredientData.itemSprite;
        productImage.sprite = productData.itemSprite;
        productPrefab = productData.itemPrefab;

        foreach (var item in products)
        {
            var sprite = item.GetComponent<SpriteRenderer>();
            sprite.sprite = productData.itemSprite;
        }
    }

    protected override void Update()
    {
        base.Update();
        
        if(npc[1] == null && npcDict.TryGetValue(NPCType.Chef, out var chef) && chef.Count != 0)
        {
            npc[1] = chef[0];
            chef.RemoveAt(0);;
        }
        
        if (npc[2] == null && npcDict.TryGetValue(NPCType.Waiter, out var waiter) && waiter.Count != 0)
        {
            npc[2] = waiter[0];
            waiter.RemoveAt(0);
        }
        
        if (playerInven != null)
        {
            GetItem();
            ReleaseItem();
        }
        if (npc[1] != null && npc[1].stateMachine.currentState == npc[1].stateMachine.NPCInteractState)
        {
            GetNPCItem();
        }

        if (npc[2] != null && npc[2].stateMachine.currentState == npc[2].stateMachine.NPCInteractState)
        {
            ReleaseNPCItem();
        }
        
        if (_canMake && ingredientCount != 0)
        {
            Produce();
        }

    }

    public override void GetItem()
    {
        if (_removeCoroutine == null)
        {
            _removeCoroutine = StartCoroutine(RemoveItemCoroutine());
        }
    }
    
    private void GetNPCItem()
    {
        if (_removeNPCCoroutine == null)
        {
            _removeNPCCoroutine = StartCoroutine(RemoveNPCItemCoroutine());
        }
    }
    
    private void ReleaseNPCItem()
    {
        while (productCount != 0 && npc[2].npcInven.CurrentQuantity < npc[2].npcInven.MaxQuantity &&
               npc[2].npcInven.addItemCoroutine == null && _releaseItemCoroutine == null)
        {
            productCount--;
            bottleIdx = Mathf.Min(productCount, 15);
            products[bottleIdx].SetActive(false);

            _releaseItemCoroutine = StartCoroutine(ReleaseItemCoroutine(productPrefab,
                products[bottleIdx].transform.position,
                npc[2].transform, 0.3f));
            
            npc[2].npcInven.addItemCoroutine = StartCoroutine(npc[2].npcInven.AddItem(productData));
        }
    }
    
    private IEnumerator RemoveNPCItemCoroutine()
    {
        try
        {
            if (npc[1] != null && npc[1].npcInven.itemsDic.ContainsKey(ingredientData) &&
                   npc[1].npcInven.itemsDic[ingredientData] > 0)
            {
                npc[1].npcInven.RemoveItem(ingredientData);
                yield return Utils.BezierMove(ingredientPrefab, npc[1].transform.position, ingrediantPos, 0.1f);
                ingredientCount++;

                ingredientCountText.text = ingredientCount.ToString();
            }
        }
        finally
        {
            _removeNPCCoroutine = null;
        }
    }
    
    private IEnumerator RemoveItemCoroutine()
    {
        try
        {
            if (playerInven != null && playerInven.itemsDic.ContainsKey(ingredientData) &&
                   playerInven.itemsDic[ingredientData] > 0)
            {
                playerInven.RemoveItem(ingredientData);
                yield return Utils.BezierMove(ingredientPrefab, playerInven.transform.position, ingrediantPos, 0.1f);
                ingredientCount++;

                ingredientCountText.text = ingredientCount.ToString();
            }
        }
        finally
        {
            _removeCoroutine = null;
        }
    }

    public override void ReleaseItem()
    {
        while (productCount != 0 && playerInven.CurrentQuantity < playerInven.MaxQuantity &&
               playerInven.addItemCoroutine == null && _releaseItemCoroutine == null)
        {
            productCount--;
            bottleIdx = Mathf.Min(productCount, 15);
            products[bottleIdx].SetActive(false);

            _releaseItemCoroutine = StartCoroutine(ReleaseItemCoroutine(productPrefab,
                products[bottleIdx].transform.position,
                playerInven.transform, 0.1f));
            
            playerInven.addItemCoroutine = StartCoroutine(playerInven.AddItem(productData, 1));
        }
    }
    
    public override void Produce()
    {
        StartCoroutine(MakeFood());       
    }

    private IEnumerator MakeFood()
    {
        _canMake = false;
        animator.SetBool(isCooking, true);        
        AudioManager.Instance.PlaySFX3D(CookSound, transform.position);

        yield return new WaitForSeconds(productTime*PlayerManager.Instance.Player.playerCondition.totalProductionSpeedBonus);
        ingredientCount--;
        ingredientCountText.text = ingredientCount.ToString();

        bottleIdx = Mathf.Min(productCount, 15);
        yield return Utils.BezierMove(productPrefab, productSpawnPos.position, products[bottleIdx].transform, 0.1f);
        products[bottleIdx].SetActive(true);
        productCount++;
        
        EventManager.Instance.Publish(new ItemProducedEvent { CookInstance = this, ProducedItem = productData });
        
        _canMake = true;
        animator.SetBool(isCooking, false);
        AudioManager.Instance.StopSFX(CookSound);
    }

    protected override void InitializeSpecificValue(ArchDataSO archData)
    {
        currentValue = (float)productPrice;
        nextValue = (float)new BigInteger(Math.Round((decimal)productPrice * (decimal)(productUpgradeMultiplier + GameConstants.Math.PERCENTAGE_BASE) / GameConstants.Math.PERCENTAGE_BASE));
    }

    protected override void ApplySingleUpgradeFormulas()
    {
        // Cook: 생산 단가 상승
        productPrice = new BigInteger(Math.Round((decimal)productPrice * (decimal)(productUpgradeMultiplier + GameConstants.Math.PERCENTAGE_BASE) / GameConstants.Math.PERCENTAGE_BASE));
        upgradePrice = new BigInteger(Math.Round((decimal)upgradePrice * (decimal)(upgradeMultiplier + GameConstants.Math.PERCENTAGE_BASE) / GameConstants.Math.PERCENTAGE_BASE));
        currentValue = (float)productPrice;
        nextValue = (float)new BigInteger(Math.Round((decimal)productPrice * (decimal)(productUpgradeMultiplier + GameConstants.Math.PERCENTAGE_BASE) / GameConstants.Math.PERCENTAGE_BASE));
        upgradeMultiplier *= (upgradeMultiplierChangeRate + GameConstants.Math.PERCENTAGE_BASE) / GameConstants.Math.PERCENTAGE_BASE;
        productUpgradeMultiplier *= (GameConstants.Math.PERCENTAGE_BASE - productUpgradeMultiplierChangeRate) / GameConstants.Math.PERCENTAGE_BASE;

    }

    protected override bool CanUpgrade()
    {
        if (!ArchitectureManager.Instance.stands.ContainsKey(productData))
        {
            UIManager.Instance.OpenUI<WarningUI>();
            return false;
        }
        return true;
    }

    protected override void OnUpgrade()
    {
        if (upgradeLevel == 5)
            ArchitectureManager.Instance.cookConditions.TryAdd(productData, true);
    }
}