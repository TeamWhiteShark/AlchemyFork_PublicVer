using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefInteract : IInteract
{
    public void Interact(NPC npc, NPCStateMachine stateMachine)
    {
        switch (npc.targetArchType)
        {
            case ArchType.Warehouse:
                if (npc.npcInven.CurrentQuantity == npc.npcInven.MaxQuantity)
                {
                    foreach (var item in npc.npcInven.itemsDic.Keys)
                    {
                        foreach (var cook in ArchitectureManager.Instance.cooks.Values)
                        {
                            if (cook.ingredientData == item)
                            {
                                npc.targetItem = cook.productData;
                                npc.objType = ObjType.Mushroom;
                                npc.targetArchType = ArchType.Cook;
                                npc.targetObj = null;
                                stateMachine.ChangeState(stateMachine.NPCIdleState);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if (ArchitectureManager.Instance.warehouses[0].itemsDic.Count == 0)
                    {
                        if (ArchitectureManager.Instance.warehouses[1].itemsDic.Count == 0 && npc.npcInven.CurrentQuantity != 0)
                        {
                            foreach (var item in npc.npcInven.itemsDic.Keys)
                            {
                                foreach (var cook in ArchitectureManager.Instance.cooks.Values)
                                {
                                    if (cook.ingredientData == item)
                                    {
                                        npc.targetItem = cook.productData;
                                        npc.objType = ObjType.Mushroom;
                                        npc.targetArchType = ArchType.Cook;
                                        npc.targetObj = null;
                                        stateMachine.ChangeState(stateMachine.NPCIdleState);
                                        return;
                                    }
                                }
                            }
                        }
                        else if(ArchitectureManager.Instance.warehouses[1].itemsDic.Count != 0)
                        {
                            if(npc.objType == ObjType.Mushroom)
                            {
                                npc.objType = ObjType.Meat;
                                npc.targetObj = null;
                                stateMachine.ChangeState(stateMachine.NPCIdleState);
                            }
                            else
                            {
                                if (ArchitectureManager.Instance.warehouses[1].itemsDic.Count == 0 &&
                                    npc.npcInven.CurrentQuantity != 0)
                                {
                                    foreach (var item in npc.npcInven.itemsDic.Keys)
                                    {
                                        foreach (var cook in ArchitectureManager.Instance.cooks.Values)
                                        {
                                            if (cook.ingredientData == item)
                                            {
                                                npc.targetItem = cook.productData;
                                                npc.objType = ObjType.Mushroom;
                                                npc.targetArchType = ArchType.Cook;
                                                npc.targetObj = null;
                                                stateMachine.ChangeState(stateMachine.NPCIdleState);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if(ArchitectureManager.Instance.warehouses[0].itemsDic.Count != 0 && npc.objType == ObjType.Meat && ArchitectureManager.Instance.warehouses[1].itemsDic.Count == 0)
                    {
                        npc.objType = ObjType.Mushroom;
                        npc.targetObj = null;
                        stateMachine.ChangeState(stateMachine.NPCIdleState);
                    }
                }
                
                
                
                // if (ArchitectureManager.Instance.warehouses[0].itemsDic.Count != 0 && npc.npcInven.CurrentQuantity == npc.npcInven.MaxQuantity)
                // {
                //     foreach (var item in npc.npcInven.itemsDic.Keys)
                //     {
                //         foreach (var cook in ArchitectureManager.Instance.cooks.Values)
                //         {
                //             if (cook.ingredientData == item)
                //             {
                //                 npc.targetItem = cook.productData;
                //                 npc.targetArchType = ArchType.Cook;
                //                 npc.targetObj = null;
                //                 stateMachine.ChangeState(stateMachine.NPCIdleState);
                //                 return;
                //             }
                //         }
                //     }
                // }
                // else if (ArchitectureManager.Instance.warehouses[0].itemsDic.Count == 0 &&
                //          npc.npcInven.CurrentQuantity != 0 && ArchitectureManager.Instance.warehouses[0].itemsDic.Count == 0)
                // {
                //     foreach (var item in npc.npcInven.itemsDic.Keys)
                //     {
                //         foreach (var cook in ArchitectureManager.Instance.cooks.Values)
                //         {
                //             if (cook.ingredientData == item)
                //             {
                //                 npc.targetItem = cook.productData;
                //                 npc.targetArchType = ArchType.Cook;
                //                 npc.targetObj = null;
                //                 stateMachine.ChangeState(stateMachine.NPCIdleState);
                //                 return;
                //             }
                //         }
                //     }
                // }
                // else if (ArchitectureManager.Instance.warehouses[0].itemsDic.Count == 0 &&
                //          npc.npcInven.CurrentQuantity != 0 && ArchitectureManager.Instance.warehouses[0].itemsDic.Count != 0)
                // {
                //     npc.objType = ObjType.Meat;
                //     npc.targetObj = null;
                //     stateMachine.ChangeState(stateMachine.NPCIdleState);
                // }
                // else if ((ArchitectureManager.Instance.warehouses[0].itemsDic.Count != 0 &&
                //           npc.npcInven.CurrentQuantity == npc.npcInven.MaxQuantity) ||
                //          (ArchitectureManager.Instance.warehouses[0].itemsDic.Count == 0 &&
                //           npc.npcInven.CurrentQuantity != 0))
                // {
                //     npc.objType = ObjType.Mushroom;
                //     foreach (var item in npc.npcInven.itemsDic.Keys)
                //     {
                //         foreach (var cook in ArchitectureManager.Instance.cooks.Values)
                //         {
                //             if (cook.ingredientData == item)
                //             {
                //                 npc.targetItem = cook.productData;
                //                 npc.targetArchType = ArchType.Cook;
                //                 npc.targetObj = null;
                //                 stateMachine.ChangeState(stateMachine.NPCIdleState);
                //                 return;
                //             }
                //         }
                //     }
                // }

                break;
            case ArchType.Cook:
                if (npc.npcInven.CurrentQuantity == 0)
                {
                    npc.targetItem = npc.homeItem;
                    npc.targetArchType = ArchType.Warehouse;
                    npc.targetObj = null;
                    stateMachine.ChangeState(stateMachine.NPCIdleState);
                }
                else if (npc.npcInven.itemsDic.Count != 0 && !npc.npcInven.itemsDic.ContainsKey(npc.targetItem.recipe[0]))
                {
                    foreach (var item in npc.npcInven.itemsDic.Keys)
                    {
                        foreach (var cook in ArchitectureManager.Instance.cooks.Values)
                        {
                            if (cook.ingredientData == item)
                            {
                                npc.targetItem = cook.productData;
                                npc.targetArchType = ArchType.Cook;
                                npc.targetObj = null;
                                stateMachine.ChangeState(stateMachine.NPCIdleState);
                                return;
                            }
                        }
                    }
                }
                
                break;
        }
    }
}
