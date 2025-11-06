using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCondition : Entity, IDamagable
{
    private Player player;
    public PlayerData playerData;
    private PlayerInventory playerInventory;
    private PlayerStateMachine playerStateMachine;
    private List<ItemSO> items = new List<ItemSO>();
    private GameObject magnetic;
    private bool isInvincible;
    private int LastAttackedMonsterID;
    public int atkBonus;
    public int totalAtk;
    public int healthbonus;
    public int MaxHealth;
    public float atkRateBonus;
    public float totalAtkRate;
    public int walkSpeedBonus;
    public int totalWalkSpeed;
    public float SaleBonus = 1;
    public float SaleBonusValue;
    public float totalSaleBonus;
    public float ProductionSpeedBonus = 1;
    public float ProductionSpeedBonusValue;
    public float totalProductionSpeedBonus;

    [SerializeField] private AudioClip PlayerDie;
    public AudioClip GetGold;
    public AudioClip UseGold;
    public AudioClip GetItem;

    public event Action OnPlayerDie;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerInventory = GetComponent<PlayerInventory>();
        playerStateMachine = player.playerStateMachine;
        playerData = Resources.Load<PlayerData>(GameConstants.Paths.PLAYER_DATA_PATH);
        magnetic = transform.GetChild(3).gameObject;

        MaxHealth = playerData.Playerhealth;
        health = playerData.Playerhealth;
        attackPoint = playerData.PlayerattackPoint;
        walkSpeed = playerData.PlayerwalkSpeed;
        attackRate = playerData.PlayerattackRate;
        
        totalAtk = attackPoint;
        totalWalkSpeed = walkSpeed;
        totalAtkRate = attackRate;
        totalSaleBonus = SaleBonus;
        totalProductionSpeedBonus = ProductionSpeedBonus;
    }

    public void TotalStat(int playerStat, int bonusStat,ref int totalStat)
    {
        totalStat = playerStat + bonusStat;
    }

    public void GetLastAttackedMonsterID(int ID)
    {
        LastAttackedMonsterID = ID;
    }
    
    //피격 메서드
    public void GetDamage(int damage)
    {
        if (health <= 0 || isInvincible)
            return;
        
        isInvincible = true;
        Debug.Log(health);
        player.rb.velocity = Vector2.zero;
        
        health -= damage;
        player.HPbar.fillAmount = (float)health / MaxHealth;
        
        if (health <= 0)
        {
            OnDie();
        }
        
        // 메서드 내부 지역 코루틴으로 1초 후 무적 해제
        StartCoroutine(InvincibleWindow());
        System.Collections.IEnumerator InvincibleWindow()
        {
            yield return new WaitForSeconds(1f);
            isInvincible = false;
        }
    }

    public void OnDie()
    {        
        Analytics.AddEvent("player_death_count", new Dictionary<string, object>
        {
            { "monster_id", LastAttackedMonsterID }
        });
        
        StartCoroutine(Die());
        
        AudioManager.Instance.PlaySFX(PlayerDie);
    }

    //플레이어 죽음 코루틴
    private IEnumerator Die()
    {
        magnetic.SetActive(false);
        playerStateMachine.ChangeState(playerStateMachine.PlayerDieState);
        yield return null;
        
        if (playerInventory.itemsDic != null)
        {
            Vector2 deathPosition = transform.position;
            //KeyValuePair들을 임시 리스트로 복사해서 순회
            foreach (var item in playerInventory.itemsDic.ToList())
            {
                for (int i = 0; i < item.Value; i++)
                {
                    Drop(item.Key, deathPosition);
                    yield return new WaitForSeconds(0.05f);
                }
            }
            playerInventory.itemsDic.Clear();
            
            yield return new WaitForSeconds(1f);
            
            health = MaxHealth;
            player.HPbar.fillAmount = (float)health / MaxHealth;
        }
        
        if(SceneLoadManager.Instance.NowSceneName == GameConstants.SceneNames.MAIN_GAME_SCENE)
        {
            transform.position = GameConstants.Player.PLAYER_RESPAWN_POSITION;
        }
        else if(SceneLoadManager.Instance.NowSceneName == GameConstants.SceneNames.SECOND_MAIN_GAME_SCENE)
        {
            transform.position = GameConstants.Player.PLAYER_RESPAWN_POSITION2;
        }
        playerStateMachine.ChangeState(playerStateMachine.PlayerIdleState);
        
        yield return null;
        
        magnetic.SetActive(true);
        OnPlayerDie?.Invoke();
    }

    //아이템 드롭 메서드
    private void Drop(ItemSO item, Vector2 deathPosition)
    {
        Vector2 Randomdrop = new Vector2(Random.Range(-1f,1f), Random.Range(-1f,1f));
        
        var dropitem =ObjectPoolManager.Instance.GetObject
            (item.prefab, deathPosition + Randomdrop, Quaternion.identity);
        
        dropitem.GetComponent<Item>().get = false;
        
        OnPlayerDie += dropitem.GetComponent<Item>().RemoveMagnetic;
    }

    public void Heal(int value)
    {
        if (health < MaxHealth)
        {
            health += value;
            player.HPbar.fillAmount = (float)health / MaxHealth;
        }
    }

    //자동 체력 회복 코루틴
    public IEnumerator AutoHeal()
    {
        while (true)
        {
            if (!player.InDungeon)
            {
                Heal(1);
                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return null;
            }
        }
    }
}
