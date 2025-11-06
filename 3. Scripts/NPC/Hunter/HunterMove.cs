using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterMove : IMove
{
    public void Move(NPC npc, NPCStateMachine stateMachine)
    {
        if (npc.homeItem == null && npc.targetObj == null)
        {
            stateMachine.ChangeState(stateMachine.NPCIdleState);
        }
        
        if(npc.targetObj == null)
            stateMachine.ChangeState(stateMachine.NPCIdleState);
        else if (npc.targetArchType == ArchType.Warehouse)
        {
            stateMachine.NPC.distance = Vector3.Distance(stateMachine.TargetPos, stateMachine.NPC.transform.position);

            if (stateMachine.NPC.agent.CalculatePath(stateMachine.TargetPos, stateMachine.NPC.path))
            {
                if(stateMachine.NPC.distance > 1f)
                    stateMachine.NPC.agent.SetDestination(stateMachine.TargetPos);
                else
                    stateMachine.ChangeState(stateMachine.NPCWaitState);
            }
        }
        else
        {
            stateMachine.NPC.distance = Vector3.Distance(npc.targetObj.transform.position, stateMachine.NPC.transform.position);

            if (stateMachine.NPC.agent.CalculatePath(npc.targetObj.transform.position, stateMachine.NPC.path))
            {
                if(stateMachine.NPC.distance > 1f)
                    stateMachine.NPC.agent.SetDestination(npc.targetObj.transform.position);
                else
                    stateMachine.ChangeState(stateMachine.NPCWaitState);
            }
        }
    }
}
