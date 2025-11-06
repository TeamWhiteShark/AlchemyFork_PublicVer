using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

public class Counter : BaseArchitecture
{
    [SerializeField]private Chest _chest;
    
    public Coroutine coroutine;
    [SerializeField] private Animator animator;
    private readonly int isOpen = Animator.StringToHash("IsOpen");
    private readonly int isLoad =  Animator.StringToHash("IsLoad");
    private Vector3 startPos;
    public bool havePlayer;
    public GameObject cashierZone;
    public InteractZone interactZone;

    [SerializeField] private TextMeshProUGUI moneyText;
    public List<ItemSO> calculateItems = new List<ItemSO>();

    public BigInteger gold;

    protected override void Start()
    {
        base.Start();
        UpdateMoney();
    }

    protected override void Update()
    {
        base.Update();
        
        if (npc[3] == null && npcDict.TryGetValue(NPCType.Cashier, out var value) && value.Count != 0)
        {
            npc[3] = value[0];
            value.RemoveAt(0);
        }
        
        if (customer != null && (npc[3] != null || playerInven != null))
        {
            GetItem();
            
        }

        if (playerInven != null && gold != 0)
        {
            ReleaseItem();
        }
    }

    public override void GetItem()
    {
        if (customer != null && canCalculate && customer.stateMachine.currentState == customer.stateMachine.NPCInteractState)
        {
            if (coroutine != null) return;
            if (npc[3] == null && !havePlayer) return;
            coroutine = StartCoroutine(Calculate());
        }
    }

    public void CancleCalculate()
    {
        if(customer != null)
        {
            customer.loading.SetActive(false);
            customer.loadingAnimator.SetBool(isLoad, false);
        }
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public IEnumerator Calculate()
    {
        try
        {
            pTime = havePlayer ? productTime : npc[3].time;
            var itemID = customer.homeItem;

            if (!customer.calculateEnd)
            {
                customer.loading.SetActive(true);
                customer.loadingAnimator.speed = 1.05f / pTime;
                customer.loadingAnimator.SetBool(isLoad, true);

                yield return new WaitForSeconds(pTime);

                customer.calculateEnd = true;
                customer.loading.SetActive(false);
                StartCoroutine(customer.ShowGold());
                customer.loadingAnimator.SetBool(isLoad, false);
                if (playerInven != null)
                {
                    playerInven.Money += Mathf.RoundToInt((int)ArchitectureManager.Instance.cooks[itemID].productPrice *
                        customer.customerInven.itemCount * PlayerManager.Instance.Player.playerCondition.totalSaleBonus);
                    Analytics.AddEvent("gold_earned_total",
                        new Dictionary<string, object>
                        {
                            { "amount", (int)ArchitectureManager.Instance.cooks[itemID].productPrice },
                            { "source", "sale" }
                        });
                    Analytics.AddEvent("production_sell_count",
                        new Dictionary<string, object> { { "item_id", itemID.itemID }, });
                    StartCoroutine(ReleaseItemCoroutine(productData.itemPrefab, customer.transform.position,
                        playerInven.transform, 0.3f));
                }
                else
                {
                    gold += Mathf.RoundToInt((int)ArchitectureManager.Instance.cooks[itemID].productPrice *
                            customer.customerInven.itemCount * PlayerManager.Instance.Player.playerCondition.totalSaleBonus);
                    Analytics.AddEvent("production_sell_count",
                        new Dictionary<string, object> { { "item_id", itemID.itemID }, });
                    UpdateMoney();
                }
                
                EventManager.Instance.Publish(new CustomerCalculatedEvent());
            }
        }
        finally
        {
            coroutine = null;
        }
    }

    public override void ReleaseItem()
    {
        if (playerInven == null)
            return;

        int actualRelease = 0;

        if (gold >= releaseAmount)
        {
            gold -= releaseAmount;
            actualRelease = releaseAmount;
            UpdateMoney();

            repeat++;
            if (repeat >= 50)
            {
                releaseAmount = Mathf.Min(releaseAmount * 2, 99999999);
            }
        }
        else
        {
            actualRelease = (int)gold;
            gold = 0;
            UpdateMoney();
        }

        // 여기서부터는 actualRelease만 사용
        _releaseItemCoroutine = StartCoroutine(ReleaseItemCoroutine(
            productData.itemPrefab, _chest.transform.position, playerInven.transform, 0.5f));

        playerInven.Money += actualRelease;

        Analytics.AddEvent("gold_earned_total", new Dictionary<string, object>
        {
            { "amount", actualRelease },
            { "source", "cashier_sale" }
        });

        Debug.Log($"플레이어 골드 {actualRelease} 증가");
    }

    public override void CheckCustomer(Customer _customer)
    {
        if (_customer.satisfied)
        {
            customerList.Add(_customer);
        }
    }
    
    private void UpdateMoney()
    {
        moneyText.text = Utils.MoneyFormat(gold);
    }

    protected override void InitializeSpecificValue(ArchDataSO archData)
    {
        currentValue = productTime;
        nextValue = productTime * (GameConstants.Math.PERCENTAGE_BASE - productUpgradeMultiplier) / GameConstants.Math.PERCENTAGE_BASE * GameConstants.Math.PERCENTAGE_BASE / GameConstants.Math.PERCENTAGE_BASE;
    }

    protected override void ApplySingleUpgradeFormulas()
    {
        // Counter: 생산 속도 단축
        productTime = Mathf.Round(((productTime * (GameConstants.Math.PERCENTAGE_BASE - productUpgradeMultiplier) / GameConstants.Math.PERCENTAGE_BASE)) * GameConstants.Math.PERCENTAGE_BASE) / GameConstants.Math.PERCENTAGE_BASE;
        upgradePrice = new BigInteger(Math.Round((decimal)upgradePrice * (decimal)(upgradeMultiplier + GameConstants.Math.PERCENTAGE_BASE) / GameConstants.Math.PERCENTAGE_BASE));
        currentValue = productTime;
        nextValue = productTime * (GameConstants.Math.PERCENTAGE_BASE - productUpgradeMultiplier) / GameConstants.Math.PERCENTAGE_BASE * GameConstants.Math.PERCENTAGE_BASE / GameConstants.Math.PERCENTAGE_BASE;
        upgradeMultiplier *= (upgradeMultiplierChangeRate + GameConstants.Math.PERCENTAGE_BASE) / GameConstants.Math.PERCENTAGE_BASE;
        productUpgradeMultiplier *= (GameConstants.Math.PERCENTAGE_BASE - productUpgradeMultiplierChangeRate) / GameConstants.Math.PERCENTAGE_BASE;
    }

    protected override bool CanUpgrade()
    {
        if (SceneManager.GetActiveScene().name == GameConstants.SceneNames.TUTORIAL_SCENE) return false;

        if ((archType == ArchType.Warehouse))
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
        }

        return true;
    }
}
