using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class MonsterSpawnUpgrader : IStatUpgrader
{
    public string Key => "MonsterSpawn";
    private const int MonsterSpawnUp = 1; // 공격력 증가량
    public EnemyManager enemyManager = EnemyManager.Instance;

    public void Initialize(PlayerCondition condition, PlayerInventory inventory, int level)
    {
        //레벨5일때 몬스터는 10마리 나와야 함
        enemyManager.monsterCountBonus = MonsterSpawnUp * level;
        enemyManager.totalMonsterCount = enemyManager.monsterCountBonus + enemyManager.maxMonsterCount;
        //레벨 5인데 몬스터 6마리

        /*condition.atkBonus = MonsterSpawnUp * level;
        condition.TotalStat(condition.attackPoint, condition.atkBonus, ref condition.totalAtk);*/
        //플레이어 기본 공격력 + 업그레이드 보너스(value * level) = 총 공격력
    }

    public void ApplyUpgrade(PlayerCondition condition, PlayerInventory inventory, ref int level, ref int cost) // BigInteger로 변경
    {
        level++;
        cost++;
        enemyManager.monsterCountBonus = MonsterSpawnUp * level;
        enemyManager.totalMonsterCount = enemyManager.monsterCountBonus + enemyManager.maxMonsterCount;
        /*condition.atkBonus = MonsterSpawnUp * level;
        condition.TotalStat(condition.attackPoint, condition.atkBonus, ref condition.totalAtk); // 총 스탯 업데이트*/
    }

    public string GetCurrentValue(PlayerCondition condition, PlayerInventory inventory)
    {
        return enemyManager.totalMonsterCount.ToString();
    }

    public string GetNextValue(PlayerCondition condition, PlayerInventory inventory, int level)
    {
        if (level >= 5) return "Max";
        return (enemyManager.totalMonsterCount + MonsterSpawnUp).ToString();
    }
}
