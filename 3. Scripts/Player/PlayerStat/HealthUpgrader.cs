using System.Collections.Generic;
using System.Numerics;

public class HealthUpgrader : IStatUpgrader
{
    public string Key => "Health";
    private const int ValueUp = 2; // 체력 증가량

    public void Initialize(PlayerCondition condition, PlayerInventory inventory, int level)
    {
        condition.healthbonus = ValueUp * level;
        condition.MaxHealth = condition.playerData.Playerhealth + condition.healthbonus;
        condition.health = condition.MaxHealth;
    }

    public void ApplyUpgrade(PlayerCondition condition, PlayerInventory inventory, ref int level, ref int cost)
    {
        level++;
        cost++;
        condition.healthbonus = ValueUp * level;
        condition.MaxHealth = condition.playerData.Playerhealth + condition.healthbonus;
        // 체력 회복 로직은 LaboratorySlot의 OnClickUpgradeButton에서 처리
        // condition.Heal(ValueUp); // 여기서 직접 회복하지 않음
    }

    public string GetCurrentValue(PlayerCondition condition, PlayerInventory inventory)
    {
        return condition.MaxHealth.ToString();
    }

    public string GetNextValue(PlayerCondition condition, PlayerInventory inventory, int level)
    {
        if (level >= 20) return "Max";
        return (condition.MaxHealth + ValueUp).ToString();
    }
}
