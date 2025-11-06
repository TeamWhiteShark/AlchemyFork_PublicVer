using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var pm = PlayerManager.Instance;
            if (pm != null && pm.Player != null)
                pm.Player.InDungeon = true;
        }
        else if (other.CompareTag("NPC"))
        {
            if (other.TryGetComponent(out Hunter hunter))
                hunter.inDungeon = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var pm = PlayerManager.Instance;
            if (pm != null && pm.Player != null)
                pm.Player.InDungeon = false;
        }
        else if (other.CompareTag("NPC"))
        {
            if (other.TryGetComponent(out Hunter hunter))
                hunter.inDungeon = false;
        }
    }
    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager.Instance.Player.InDungeon = true;
        }
        else if (other.CompareTag("NPC"))
        {
            other.TryGetComponent(out Hunter hunter);
            hunter.inDungeon = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager.Instance.Player.InDungeon = false;
        }
        else if (other.CompareTag("NPC"))
        {
            other.TryGetComponent(out Hunter hunter);
            hunter.inDungeon = false;
        }     
    }*/
}
