using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractZone : MonoBehaviour
{
    public BaseArchitecture architecture;
    public float maxXpos;
    public float minXpos;
    public float maxYpos;
    public float minYpos;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //플레이어가 콜라이더에 들어왔을 때
        if (other.gameObject.CompareTag("Player"))
        {
            if (architecture.CompareTag("Customer")) return;
            architecture.playerInven = other.GetComponent<PlayerInventory>();
            architecture.playerIn = true;
            if (architecture.CountUI != null)
            {
                architecture.CountUI.SetActive(true);
            }
        }
        else if (other.gameObject.CompareTag("Customer"))
        {
            architecture.CheckCustomer(other.GetComponent<Customer>()); 
        }
        // else if (other.gameObject.CompareTag("NPC"))
        // {
        //     if(other.GetComponent<NPC>().stateMachine.currentState != other.GetComponent<NPC>().stateMachine.NPCMoveState && !architecture.npcDict[other.GetComponent<NPC>().npcType].Contains(other.GetComponent<NPC>()))
        //         architecture.npcDict[other.GetComponent<NPC>().npcType].Enqueue(other.GetComponent<NPC>());
        // }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            if (!architecture.npcDict[other.GetComponent<NPC>().npcType].Contains(other.GetComponent<NPC>()) && other.GetComponent<NPC>().targetObj == architecture.gameObject)
            {
                switch (other.GetComponent<NPC>().npcType)
                {
                    case NPCType.Hunter:
                        if(architecture.npc[0] != other.GetComponent<NPC>() && architecture.archType == other.GetComponent<NPC>().targetArchType && other.GetComponent<NPC>().targetObj == architecture.gameObject)
                            architecture.npcDict[other.GetComponent<NPC>().npcType].Add(other.GetComponent<NPC>());

                        break;
                    case NPCType.Chef:
                        if(architecture.npc[1] != other.GetComponent<NPC>() && architecture.archType == other.GetComponent<NPC>().targetArchType && other.GetComponent<NPC>().targetObj == architecture.gameObject)
                            architecture.npcDict[other.GetComponent<NPC>().npcType].Add(other.GetComponent<NPC>());
                        
                        break;
                    case NPCType.Waiter:
                        if(architecture.npc[2] != other.GetComponent<NPC>() && architecture.archType == other.GetComponent<NPC>().targetArchType && other.GetComponent<NPC>().targetObj == architecture.gameObject)
                            architecture.npcDict[other.GetComponent<NPC>().npcType].Add(other.GetComponent<NPC>());
                        
                        break;
                    case NPCType.Cashier:
                        if(architecture.npc[3] != other.GetComponent<NPC>() && architecture.archType == other.GetComponent<NPC>().targetArchType && other.GetComponent<NPC>().targetObj == architecture.gameObject)
                            architecture.npcDict[other.GetComponent<NPC>().npcType].Add(other.GetComponent<NPC>());
                        
                        break;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(architecture.archType != ArchType.Warehouse)
            {
                architecture.playerInven = null;
            }
            architecture.playerIn = false;
            if (architecture.CountUI != null)
            {
                architecture.CountUI.SetActive(false);
            }
        }
        else if (other.gameObject.CompareTag("NPC"))
        {
            switch (other.GetComponent<NPC>().npcType)
            {

                case NPCType.Hunter:
                    if(architecture.npc[0] == other.GetComponent<NPC>())
                        architecture.npc[0] = null;
                    else
                        architecture.npcDict[other.GetComponent<NPC>().npcType].Remove(other.GetComponent<NPC>());
                    break;
                case NPCType.Chef:
                    if(architecture.npc[1] == other.GetComponent<NPC>())
                        architecture.npc[1] = null;
                    else
                        architecture.npcDict[other.GetComponent<NPC>().npcType].Remove(other.GetComponent<NPC>());
                    break;
                case NPCType.Waiter:
                    if(architecture.npc[2] == other.GetComponent<NPC>())
                        architecture.npc[2] = null;
                    else
                        architecture.npcDict[other.GetComponent<NPC>().npcType].Remove(other.GetComponent<NPC>());
                    break;
                case NPCType.Cashier:
                    if(architecture.npc[3] == other.GetComponent<NPC>())
                        architecture.npc[3] = null;
                    else
                        architecture.npcDict[other.GetComponent<NPC>().npcType].Remove(other.GetComponent<NPC>());
                    break;
            }
        }
    }
}
