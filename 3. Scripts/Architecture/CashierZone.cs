using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashierZone : MonoBehaviour
{
    [SerializeField]private Counter _counter;
    private NPC npc;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            npc = other.GetComponent<NPC>();
            if (npc.npcType == NPCType.Cashier)
            {
                _counter.canCalculate = true;
                _counter.npcDict[other.GetComponent<NPC>().npcType].Add(other.GetComponent<NPC>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            if (npc.npcType == NPCType.Cashier)
            {
                _counter.canCalculate = false;
                if(_counter.coroutine != null)
                    _counter.StopCoroutine(_counter.coroutine);
            }
        }
    }
}
