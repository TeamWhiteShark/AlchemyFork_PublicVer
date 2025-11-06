using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Customer : NPC
{
    [Header("ItemInfo")]
    [SerializeField] private SpriteRenderer productSprite;
    [SerializeField] private TextMeshProUGUI productText;
    public GameObject loading;
    public Animator loadingAnimator;

    [SerializeField] private GameObject goldTextObj;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private CanvasGroup canvasGroup;
    
    protected override void Awake()
    {
        base.Awake();
        customerInven = GetComponent<CustomerInven>();
        canvasGroup.alpha = 0;
    }
    protected override void Update()
    {
        base.Update();
        if (customerInven.itemCount != 0)
        {
            productSprite.sprite = homeItem.itemSprite;
            
            productText.text = customerInven.itemCount.ToString();
        }
        else
        {
            productSprite.sprite = null;
            productText.text = "";
        }
        
        if (transform.position.y <= CustomerManager.Instance.deSpawnPosition.y + 1f && (calculateEnd || anger))
        {
            CustomerManager.Instance.DespawnCustomer(this);
        }
    }

    public IEnumerator ShowGold()
    {
        canvasGroup.alpha = 1;

        var rt = (RectTransform)goldTextObj.transform;
        Vector2 start = new (rt.anchoredPosition.x, 1f);
        Vector2 end   = new (start.x, 2f);
        rt.anchoredPosition = start;

        goldText.text = Utils.MoneyFormat(Mathf.RoundToInt(
            (int)ArchitectureManager.Instance.cooks[homeItem].productPrice * customerInven.itemCount *
            PlayerManager.Instance.Player.playerCondition.totalSaleBonus));

        const float duration = 1f;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            rt.anchoredPosition = Vector2.LerpUnclamped(start, end, t);
            canvasGroup.alpha = 1 - t;
            yield return null;
        }
    }

    public void SetOrder(ItemSO homeItem, ItemSO awayItem)
    {
        this.homeItem =  homeItem;
        this.awayItem = awayItem;
    }
}

