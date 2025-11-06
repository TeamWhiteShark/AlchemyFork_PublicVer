using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashierWait : IWait
{
    public void Decide(NPC npc, NPCStateMachine stateMachine)
    {
        if(npc.targetObj.GetComponent<BaseArchitecture>().customer != null)
            stateMachine.ChangeState(stateMachine.NPCInteractState);
    }
}
