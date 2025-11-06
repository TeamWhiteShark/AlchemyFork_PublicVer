using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerInven : MonoBehaviour
{
    public Customer customer;
    public int itemCount;
    public int itemCalCount;
    public float productPrice;

    private void Awake()
    {
        customer = GetComponent<Customer>();
    }

    public void AddItem()
    {
        
    }

    public void RemoveItem()
    {
        itemCount--;
    }
}
