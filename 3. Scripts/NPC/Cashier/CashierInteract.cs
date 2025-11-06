using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashierInteract : IInteract
{
    public void Interact(NPC npc, NPCStateMachine stateMachine)
    {
        if(npc.targetObj.GetComponent<BaseArchitecture>().customer == null)
        {
            stateMachine.ChangeState(stateMachine.NPCWaitState);
        }
        else
        {
            npc.targetObj.GetComponent<BaseArchitecture>().npc[3] = npc;
            npc.targetObj.GetComponent<BaseArchitecture>().canCalculate = true;
        }
    }
}
