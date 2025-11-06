using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class AttackUpgrader : IStatUpgrader
{
    public string Key => "Attack";
    private const int ValueUp = 1; // 공격력 증가량

    public void Initialize(PlayerCondition condition, PlayerInventory inventory, int level)
    {
        condition.atkBonus = ValueUp * level;
        condition.TotalStat(condition.attackPoint, condition.atkBonus, ref condition.totalAtk);
    }

    public void ApplyUpgrade(PlayerCondition condition, PlayerInventory inventory, ref int level, ref int cost) // BigInteger로 변경
    {
        level++;
        cost++;
        condition.atkBonus = ValueUp * level;
        condition.TotalStat(condition.attackPoint, condition.atkBonus, ref condition.totalAtk); // 총 스탯 업데이트
    }

    public string GetCurrentValue(PlayerCondition condition, PlayerInventory inventory)
    {
        return condition.totalAtk.ToString();
    }

    public string GetNextValue(PlayerCondition condition, PlayerInventory inventory, int level)
    {
        if (level >= 20) return "Max";
        return (condition.totalAtk + ValueUp).ToString();
    }
    
}
