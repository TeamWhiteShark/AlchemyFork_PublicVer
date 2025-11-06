using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// 몬스터의 스텟 관련
public class MonsterCondition : Entity, IDamagable
{
    public Monster monster;
    public MonsterData monsterData;
    public ItemSO dropItem;
    public Transform HpBar;
    public bool canAttack;
    
    private void Awake()
    {
        monster = GetComponent<Monster>();
        dropItem = monsterData.dropItem;        
    }
    
    private void Start()
    {
        InitStats();
        AttackAble();
    }
    
    public void InitStats()
    {
        health = monsterData.monsterHealth;
        attackPoint = monsterData.monsterAttack;
        walkSpeed = monsterData.monsterSpeed;
        attackRate = monsterData.monsterAttackRate;
        HpBar.localScale = Vector3.one;
    }
    
    private void AttackAble()
    {
        if (monsterData.monsterType == MonsterType.Plant)
        {
            canAttack = false;
        }
        else
        {
            canAttack = true;
        }
    }
    
    public void GetDamage(int damage)
    {
        if (health <= 0) return;
        
        //플레이어무기에 닿았을때 데미지 받음 (hp에서 atk만큼빼기)
        //체력닳는 애니메이션추가
        health -= damage;
        health = Mathf.Max(health, 0);
        HpBar.localScale = new Vector3((float)health / monsterData.monsterHealth, GameConstants.Monster.HP_BAR_FULL_SCALE.y, GameConstants.Monster.HP_BAR_FULL_SCALE.z);

        // monster.Controller.ChangeStateToRunOrAttacking();
        EventManager.Instance.Publish(new MonsterDamagedEvent { MonsterCondition = this });
        
        if (health <= 0)
        {
            OnDie();
        }
    }
    
    public void OnDie()
    {
        if (health <= 0)
        {
            EventManager.Instance.Publish(new MonsterDiedEvent { MonsterCondition = this });
            
            // 퀘스트에 몬스터가 처치된 것을 알림
            // QuestManager.Instance.UpdateQuestProgress(monsterData.monsterID);
            Analytics.AddEvent("monster_kill_count", new Dictionary<string, object>
            {
                { "monster_id", monsterData.monsterID },
            });
        }
    }
}
