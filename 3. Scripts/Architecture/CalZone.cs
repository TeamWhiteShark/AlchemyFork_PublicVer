using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalZone : MonoBehaviour
{
    [SerializeField]private Counter _counter;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _counter.havePlayer = true;
            _counter.canCalculate = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _counter.havePlayer = false;
            _counter.canCalculate = false;
            if(_counter.npc[3] == null)
                _counter.CancleCalculate();
        }
    }
}
