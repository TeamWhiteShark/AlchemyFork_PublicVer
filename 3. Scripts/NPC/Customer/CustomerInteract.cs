using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerInteract : IInteract
{
    public void Interact(NPC npc, NPCStateMachine stateMachine)
    {
        if (npc.customerInven.itemCount == npc.targetCount && !npc.calculateEnd && npc.targetArchType != ArchType.Counter)
        {
            npc.targetItem = npc.awayItem;
            npc.targetArchType = ArchType.Counter;
            npc.satisfied = true;
            npc.targetObj.GetComponent<BaseArchitecture>().customer = null;
            npc.targetObj = null;
            stateMachine.ChangeState(stateMachine.NPCIdleState);
        }
        else if (npc.calculateEnd)
        {
            npc.targetObj.GetComponent<BaseArchitecture>().customer = null;
            npc.targetObj = CustomerManager.Instance.deSpawnPoint;
            stateMachine.TargetPos = CustomerManager.Instance.deSpawnPosition;
            stateMachine.ChangeState(stateMachine.NPCMoveState);
        }
    }
}
