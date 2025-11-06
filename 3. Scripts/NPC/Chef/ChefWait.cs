using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefWait : IWait
{
    public void Decide(NPC npc, NPCStateMachine stateMachine)
    {
        if (npc.targetItem == npc.homeItem)
        {
            if (ArchitectureManager.Instance.warehouses[0].itemsDic.Count > 0 ||
                ArchitectureManager.Instance.warehouses[1].itemsDic.Count > 0)
            {
                stateMachine.ChangeState(stateMachine.NPCInteractState);
            }
        }
        else
        {
            if (ArchitectureManager.Instance.cooks.ContainsKey(npc.targetItem))
            {
                stateMachine.ChangeState(stateMachine.NPCInteractState);
            }
        }
    }
}
