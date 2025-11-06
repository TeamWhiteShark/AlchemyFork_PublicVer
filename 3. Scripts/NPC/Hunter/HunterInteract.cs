using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterInteract : IInteract
{
    public void Interact(NPC npc, NPCStateMachine stateMachine)
    {
        switch (npc.targetArchType)
        {
            case ArchType.DungeonWall:
                if (npc.npcInven.CurrentQuantity >= npc.npcInven.MaxQuantity)
                {
                    npc.targetItem = npc.awayItem;
                    npc.targetArchType = ArchType.Warehouse;
                    npc.targetObj = null;
                    stateMachine.ChangeState(stateMachine.NPCIdleState);
                }
                else if(npc.targetObj == null)
                {
                    if (npc.targetItem == null)
                    {
                        npc.targetItem = npc.awayItem;
                        npc.targetArchType = ArchType.Warehouse;
                        stateMachine.ChangeState(stateMachine.NPCIdleState);
                    }
                    
                    if (npc.npcInven.CurrentQuantity >= npc.npcInven.MaxQuantity)
                    {
                        npc.targetItem = npc.awayItem;
                        npc.targetArchType = ArchType.Warehouse;
                        stateMachine.ChangeState(stateMachine.NPCIdleState);
                    }
                    stateMachine.ChangeState(stateMachine.NPCIdleState);
                }
                else
                {
                    if (Vector3.Distance(npc.targetObj.transform.position, stateMachine.NPC.transform.position) > 1f)
                    {
                        stateMachine.ChangeState(stateMachine.NPCMoveState);
                    }
                }
                
                break;
            case ArchType.Warehouse:
                if (npc.targetItem == null)
                {
                    npc.targetObj = null;
                    stateMachine.ChangeState(stateMachine.NPCIdleState);
                }
                else
                {
                    if (npc.npcInven.CurrentQuantity == 0)
                    {
                        npc.targetItem = npc.homeItem;
                        npc.objType = npc.homeItem.objType;
                        npc.targetArchType = ArchType.DungeonWall;
                        npc.targetObj = null;
                        stateMachine.ChangeState(stateMachine.NPCIdleState);
                    }
                    else if(npc.npcInven.itemsDic.Count != 0)
                    {
                        if(!npc.npcInven.itemsDic.ContainsKey(npc.homeItem))
                        {
                            switch (npc.objType)
                            {
                                case ObjType.Meat when ArchitectureManager.Instance.warehouses[1].CurrentQuantity <
                                                       ArchitectureManager.Instance.warehouses[1].MaxQuantity:
                                    npc.objType = ObjType.Mushroom;
                                    npc.targetObj = null;
                                    stateMachine.ChangeState(stateMachine.NPCIdleState);
                                    break;
                                case ObjType.Mushroom when ArchitectureManager.Instance.warehouses[0].CurrentQuantity <
                                                           ArchitectureManager.Instance.warehouses[0].MaxQuantity:
                                    npc.objType = ObjType.Meat;
                                    npc.targetObj = null;
                                    stateMachine.ChangeState(stateMachine.NPCIdleState);
                                    break;
                            }
                        }
                    }
                }
                
                break;
        }
    }
}
