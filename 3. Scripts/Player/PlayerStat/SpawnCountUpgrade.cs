using System.Collections.Generic;
using System.Numerics;


public class SpawnCountUpgrader : IStatUpgrader
{
    public string Key => "SpawnCount";
    private const int ValueUp = 3; // 스폰 수 증가량
    public CustomerManager customerManager = CustomerManager.Instance;

    public void Initialize(PlayerCondition condition, PlayerInventory inventory, int level)
    {
        customerManager.spawnCountBonus = ValueUp * level;
        customerManager.totalSpawnCount = customerManager.spawnCountBonus + customerManager._maxSpawnCount;
    }

    public void ApplyUpgrade(PlayerCondition condition, PlayerInventory inventory, ref int level, ref int cost)
    {
        level++;
        cost++;
        customerManager.spawnCountBonus = ValueUp * level;
        customerManager.totalSpawnCount = customerManager.spawnCountBonus + customerManager._maxSpawnCount;
    }

    public string GetCurrentValue(PlayerCondition condition, PlayerInventory inventory)
    {
        return customerManager.totalSpawnCount.ToString();
    }

    public string GetNextValue(PlayerCondition condition, PlayerInventory inventory, int level)
    {
        if (level >= 20) return "Max";
        return (customerManager.totalSpawnCount + ValueUp).ToString();
    }
}
