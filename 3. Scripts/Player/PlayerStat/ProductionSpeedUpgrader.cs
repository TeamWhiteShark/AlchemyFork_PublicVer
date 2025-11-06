using System.Numerics;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ProductionSpeedBonusUpgrader : IStatUpgrader
{
    public string Key => "ProductionSpeedBonus";
    private const float ValueUp = -0.04f;
    private const int MaxLevel = 16;

    public void Initialize(PlayerCondition condition, PlayerInventory inventory, int level)
    {
        condition.ProductionSpeedBonusValue = ValueUp * level;
        condition.totalProductionSpeedBonus = Mathf.Max(0.01f, condition.ProductionSpeedBonus + condition.ProductionSpeedBonusValue); // 초기 total 값 계산
    }

    public void ApplyUpgrade(PlayerCondition condition, PlayerInventory inventory, ref int level, ref int cost)
    {
        level++;
        cost++;
        condition.ProductionSpeedBonusValue = ValueUp * level;
        condition.totalProductionSpeedBonus = Mathf.Max(0.01f, condition.ProductionSpeedBonus + condition.ProductionSpeedBonusValue);
    }

    public string GetCurrentValue(PlayerCondition condition, PlayerInventory inventory)
    {
        return condition.totalProductionSpeedBonus.ToString("F2");
    }

    public string GetNextValue(PlayerCondition condition, PlayerInventory inventory, int level)
    {
        if (level >= 20) return "Max"; // 최대 레벨 체크
        float nextValue = Mathf.Max(0.01f, condition.totalProductionSpeedBonus + ValueUp);
        return nextValue.ToString("F2");
    }
}
