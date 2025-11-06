using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameConstants;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class PlayerInventory : MonoBehaviour
{
    public PlayerData playerData;
    public Player player;
    public Coroutine addItemCoroutine;
    private bool isLoading = true;
    
    private float time;
    private Coroutine _releaseItemCoroutine;

    public int diamond;
    private BigInteger money;

    public BigInteger Money
    {
        get => money;
        set 
        {
            // 돈이 증가했을 때만 소리 재생
            if (value > money)
            {
                if (player.playerCondition.GetGold != null && isLoading == false)
                {
                    if (value != 0)
                    {
                        AudioManager.Instance.PlaySFX(player.playerCondition.GetGold);
                    }
                }
            }
            else
            {                
                AudioManager.Instance.PlaySFX(player.playerCondition.UseGold);
            }

            money = value;
            if (NextSceneMoney())
            {
                EventManager.Instance.Publish(new NextMapEvent { triggerNum = (int)NextMap.C });
                Debug.Log("C조건 충족");
            }
        }
    }

    public int maxQuantity;
    public int MaxQuantity
    {
        get => maxQuantity;
        set => maxQuantity = value;
    }
    public int bonusQuantity;
    public int totalQuantity;
    
    private int currentQuantity;
    public int CurrentQuantity
    {
        get => currentQuantity;
        private set => currentQuantity = value;
    }

    public Dictionary<ItemSO, int> itemsDic= new Dictionary<ItemSO, int>();

    private void Awake()
    {
        playerData = Resources.Load<PlayerData>(GameConstants.Paths.PLAYER_DATA_PATH);
        player = GetComponentInParent<Player>();
        MaxQuantity = playerData.MaxQuantity;

        if (!SaveLoadManager.Instance.isClickedContinue && SceneManager.GetActiveScene().name != SceneNames.SECOND_MAIN_GAME_SCENE)
        {
            diamond = playerData.Diamond;
            money = playerData.Money;
            CurrentQuantity = 0;
            isLoading = false;
        }
    }

    private void Update()
    {
        CheckQuantity();
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

    public IEnumerator AddItem(ItemSO item, int count)
    {
        try
        {
            if (!itemsDic.ContainsKey(item))
            {
                Debug.Log("아이템 획득");
                itemsDic[item] = 0; 
            }

            for (int i = 0; i < count; i++)
            {
                itemsDic[item]++; 
                yield return null; 
            }
        }
        finally
        {
            Debug.Log("끝");
            addItemCoroutine = null;
            AudioManager.Instance.PlaySFX(player.playerCondition.GetItem);
        }
    }

    public void RemoveItem(ItemSO requireItem)
    {
        if (itemsDic == null) return;

        if (itemsDic.ContainsKey(requireItem))
        {
            itemsDic[requireItem]--;

            if (itemsDic[requireItem] <= 0)
            {
                itemsDic.Remove(requireItem);
            }
        }
        else
        {
            return;
        }
    }

    public void GetRewardToPlayer(int rewardID, int rewardValue)
    {
        // 일단 골드에 관한 케이스만 추가
        if (rewardID == 9999)
        {
            Debug.Log(Money);
            Money += rewardValue;
            Analytics.AddEvent("gold_earned_total", new Dictionary<string, object>
            {
                { "amount", rewardValue },
                { "source", "reward" },
            });
            Debug.Log(Money);
        }
    }

    public void InitForLoading(BigInteger loadedMoney)
    {
        Money = loadedMoney;
        isLoading = false;
    }

    public void ReleaseMoney(GameObject moneyPrefab, Vector3 endPos)
    {
        _releaseItemCoroutine = StartCoroutine(ReleaseItemCoroutine(moneyPrefab, this.transform.position, endPos, 0.1f));
    }
    
    private IEnumerator ReleaseItemCoroutine(GameObject prefab, Vector3 startPos, Vector3 endPos, float duration)
    {
        try
        {
            yield return Utils.BezierMove(prefab, startPos, endPos, duration);
        }
        finally
        {
            _releaseItemCoroutine = null;
        }
    }

    public void SetMoneySilentlyInTutorial(BigInteger amount)
    {
        money = amount;
    }

    public bool NextSceneMoney()
    {
        if (Money >= 5000000)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
