using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private BaseArchitecture counter;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            counter.playerInven = other.gameObject.GetComponent<PlayerInventory>();
            counter.releaseAmount = 1; 
            counter.repeat = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            counter.playerInven = null;
        }
    }
}
