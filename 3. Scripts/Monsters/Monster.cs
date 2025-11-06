using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Monster : MonoBehaviour, IPoolable
{
    public MonsterCondition Condition;
    public MonsterController Controller;
    public Rigidbody2D monsterRigid;
    public Collider2D monsterCollider;
    public bool die;
    
    public event Action<GameObject> OnBeforeReturn;
    private Action<GameObject> _returnToPool;

    public int itemPrefabIndex;
    public int spawnID;

    private Vector3 Alter;
    
    private void Awake()
    {
        Condition = GetComponent<MonsterCondition>();
        Controller = GetComponent<MonsterController>();
        monsterRigid = GetComponent<Rigidbody2D>();
        monsterCollider = GetComponent<Collider2D>();
        
        EventManager.Instance.Subscribe<MonsterDiedEvent>(HandleDeath);
        EventManager.Instance.Subscribe<MonsterDamagedEvent>(HandleDamaged);
    }

    private void HandleDeath(MonsterDiedEvent e)
    {
        if (e.MonsterCondition == this.Condition)
        {
            Controller.ChangeToDieState();
        }
    }

    public void OnHandleDeath()
    {
        Drop();
        OnDespawn();
    }

    private void HandleDamaged(MonsterDamagedEvent e)
    {
        if (e.MonsterCondition == this.Condition)
        {
            Controller.OnDamaged();
        }
    }

    private void Drop()
    {
        //나중에 몬스터별 데이터에 아이템 데이터 추가해서 그거 넣기
        ObjectPoolManager.Instance.GetObject(Condition.dropItem.prefab, this.transform.position, Quaternion.identity);       
    }

    public void Initialize(Action<GameObject> returnAction)
    {
        _returnToPool = returnAction;
    }

    public void OnSpawn()
    {
        die = false;
        Condition.InitStats();
        Controller.InitState();
        gameObject.SetActive(true);
        Condition.HpBar.localScale = GameConstants.Monster.HP_BAR_FULL_SCALE;
    }

    public void OnDespawn()
    {
        OnBeforeReturn?.Invoke(gameObject);
        _returnToPool?.Invoke(gameObject);
    }
    
    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe<MonsterDiedEvent>(HandleDeath);
        EventManager.Instance.Unsubscribe<MonsterDamagedEvent>(HandleDamaged);
    }
}
