using System.Collections.Generic;
using System.Numerics;

public class HunterAttackUpgrader: IStatUpgrader
    {
        public string Key => "HunterAttack";
        private const int ValueUp = 1;
        public NPCManager npcManager = NPCManager.Instance;
        public void Initialize(PlayerCondition condition, PlayerInventory inventory, int level)
        {
            npcManager.hunterAttackBonus = ValueUp * level;
            npcManager.hunterTotalAttack = npcManager.hunterAttackPoint + npcManager.hunterAttackBonus;
        }

        public void ApplyUpgrade(PlayerCondition condition, PlayerInventory inventory, ref int level, ref int cost) // BigInteger로 변경
        {
            level++;
            cost++;
            npcManager.hunterAttackBonus = ValueUp * level;
            npcManager.hunterTotalAttack = npcManager.hunterAttackPoint + npcManager.hunterAttackBonus;
        }

        public string GetCurrentValue(PlayerCondition condition, PlayerInventory inventory)
        {
            return npcManager.hunterTotalAttack.ToString();
        }

        public string GetNextValue(PlayerCondition condition, PlayerInventory inventory, int level)
        {
            if (level >= 20) return "Max";
            return (npcManager.hunterTotalAttack + ValueUp).ToString();
        }
    }
