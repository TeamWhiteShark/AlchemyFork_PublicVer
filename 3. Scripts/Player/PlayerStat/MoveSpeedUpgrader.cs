using System.Collections.Generic;
using System.Numerics;

public class MoveSpeedUpgrader : IStatUpgrader
{
    public string Key => "MoveSpeed";
    private const int ValueUp = 2; // 이동 속도 증가량

    public void Initialize(PlayerCondition condition, PlayerInventory inventory, int level)
    {
        condition.walkSpeedBonus = ValueUp * level;
        condition.TotalStat(condition.walkSpeed, condition.walkSpeedBonus, ref condition.totalWalkSpeed);

    }

    public void ApplyUpgrade(PlayerCondition condition, PlayerInventory inventory, ref int level, ref int cost)
    {
        level++;
        cost++;
        condition.walkSpeedBonus = ValueUp * level;
        condition.TotalStat(condition.walkSpeed, condition.walkSpeedBonus, ref condition.totalWalkSpeed);
        // Player 스크립트에서 참조하는 moveSpeed도 갱신 필요 시 여기서 직접 접근하거나 이벤트 발행
        // 예: PlayerManager.Instance.Player.moveSpeed = condition.totalWalkSpeed;
    }

    public string GetCurrentValue(PlayerCondition condition, PlayerInventory inventory)
    {
        // totalWalkSpeed가 최종 이동속도를 반영하는지 확인 필요 (던전 내/외 구분 등)
        return condition.totalWalkSpeed.ToString();
    }

    public string GetNextValue(PlayerCondition condition, PlayerInventory inventory, int level)
    {
        if (level >= 20) return "Max";
        return (condition.totalWalkSpeed + ValueUp).ToString();
    }
}