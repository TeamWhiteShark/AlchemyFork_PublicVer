using System.Collections.Generic;
using System.Numerics;

public class SaleBonusUpgrader : IStatUpgrader
{
    public string Key => "SaleBonus";
    private const float ValueUp = 0.1f;

    public void Initialize(PlayerCondition condition, PlayerInventory inventory, int level)
    {
        condition.SaleBonusValue = ValueUp * level;
        condition.totalSaleBonus = condition.SaleBonus + condition.SaleBonusValue;

    }

    public void ApplyUpgrade(PlayerCondition condition, PlayerInventory inventory, ref int level, ref int cost)
    {
        level++;
        cost++;
        condition.SaleBonusValue = ValueUp * level;
        condition.totalSaleBonus = condition.SaleBonus + condition.SaleBonusValue;
    }

    public string GetCurrentValue(PlayerCondition condition, PlayerInventory inventory)
    {
        return condition.totalSaleBonus.ToString("F2");
    }

    public string GetNextValue(PlayerCondition condition, PlayerInventory inventory, int level)
    {
        if (level >= 20) return "Max";
        return (condition.totalSaleBonus + ValueUp).ToString("F2");
    }
}
