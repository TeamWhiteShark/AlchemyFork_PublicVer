using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterWait : IWait
{
    public void Decide(NPC npc, NPCStateMachine stateMachine)
    {
        if (npc.targetItem == npc.homeItem)
        {
            if (npc.targetObj == NPCManager.Instance.waitingPoint && npc.homeItem != null)
            {
                npc.targetObj = null;
                stateMachine.ChangeState(stateMachine.NPCIdleState);
                return;
            }
            
            if(npc.targetObj != NPCManager.Instance.waitingPoint)
            {
                stateMachine.ChangeState(stateMachine.NPCInteractState);
            }
        }
        else if (npc.targetItem == npc.awayItem)
        {
            switch (npc.objType)
            {
                case ObjType.Mushroom:
                    if (ArchitectureManager.Instance.warehouses[0].CurrentQuantity < ArchitectureManager.Instance.warehouses[0].MaxQuantity)
                    {
                        stateMachine.ChangeState(stateMachine.NPCInteractState);
                    }
                    break;
                case ObjType.Meat:
                    if (ArchitectureManager.Instance.warehouses[1].CurrentQuantity < ArchitectureManager.Instance.warehouses[1].MaxQuantity)
                    {
                        stateMachine.ChangeState(stateMachine.NPCInteractState);
                    }
                    break;
            }
        }
    }
}
