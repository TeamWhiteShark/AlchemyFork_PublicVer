using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CustomerWait : IWait
{
    public void Decide(NPC npc, NPCStateMachine stateMachine)
    {
        if (npc.anger)
        {
            npc.targetObj.GetComponent<BaseArchitecture>().customerList.Remove(npc.gameObject.GetComponent<Customer>());
            if (npc.targetObj.GetComponent<BaseArchitecture>().customer == npc.gameObject.GetComponent<Customer>())
                npc.targetObj.GetComponent<BaseArchitecture>().customer = null;
            npc.targetObj = CustomerManager.Instance.deSpawnPoint;
            stateMachine.TargetPos = CustomerManager.Instance.deSpawnPosition;
            stateMachine.ChangeState(stateMachine.NPCMoveState);
        }
        else if (npc.targetItem == npc.homeItem)
        {
            if (ArchitectureManager.Instance.stands.ContainsKey(npc.targetItem) &&
                ArchitectureManager.Instance.stands[npc.targetItem].productCount > 0 && ArchitectureManager.Instance.stands[npc.targetItem].customer == npc.gameObject.GetComponent<Customer>())
            {
                stateMachine.ChangeState(stateMachine.NPCInteractState);
            }
        }
        else if (npc.targetItem == npc.awayItem)
        {
            if(npc.targetCount == npc.customerInven.itemCount && npc.targetObj.GetComponent<BaseArchitecture>().customer == npc.gameObject.GetComponent<Customer>())
                stateMachine.ChangeState(stateMachine.NPCInteractState);
        }
    }
}
