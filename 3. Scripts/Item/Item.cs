using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Item : MonoBehaviour, IPoolable
{
    [SerializeField] private ItemSO itemData;
    public ItemSO ItemData => itemData;
    public bool get;
    
    private Action<GameObject> returnToPool;
    
    private Coroutine waitCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어와 부딪히면 인벤토리로 들어가는 메서드
        if (other.CompareTag("Player"))
        {
            // 자신의 오브젝트를 파괴하고 플레이어의 인벤토리에 포함된다
            if (other.TryGetComponent(out PlayerInventory player))
            {
                if (waitCoroutine == null)
                {
                    if (player.CurrentQuantity < player.MaxQuantity)
                    {
                        waitCoroutine = StartCoroutine(WaitCoroutine(player));
                    }
                    else
                    {
                        Debug.Log("인벤토리가 가득 찼습니다!");
                        return;
                    }
                }
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        // 플레이어와 부딪히면 인벤토리로 들어가는 메서드
        if (other.CompareTag("Player"))
        {
            // 자신의 오브젝트를 파괴하고 플레이어의 인벤토리에 포함된다
            if (other.TryGetComponent(out PlayerInventory player))
            {
                if (waitCoroutine == null)
                {
                    if (player.CurrentQuantity < player.MaxQuantity)
                    {
                        waitCoroutine = StartCoroutine(WaitCoroutine(player));
                    }
                    else
                    {
                        Debug.Log("인벤토리가 가득 찼습니다!");
                        return;
                    }
                }
            }
        }
    }

    private IEnumerator WaitCoroutine(PlayerInventory player)
    {
        try
        {
            player.addItemCoroutine = StartCoroutine(player.AddItem(itemData, 1));
            yield return player.addItemCoroutine;

            // QuestManager.Instance.UpdateQuestProgress(int.Parse(itemData.itemID));
        }
        finally
        {
            waitCoroutine = null;
            OnDespawn();
        }
    }

    public void RemoveMagnetic()
    {
        Debug.Log("아이템 자석 메서드 삭제");
        if (GetComponent<Magnetized>())
        {
            Destroy(GetComponent<Magnetized>());
        }
    }

    public event Action<GameObject> OnBeforeReturn;

    public void Initialize(Action<GameObject> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn()
    {
        gameObject.SetActive(true);
        get = false;
        
        var magnetized = this.gameObject.GetComponent<Magnetized>();
        Destroy(magnetized);
    }

    public void OnDespawn()
    {
        ObjectPoolManager.Instance.ReturnObject(this.gameObject);
    }
}
