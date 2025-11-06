using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    event Action<GameObject> OnBeforeReturn;
    
    void Initialize(Action<GameObject> returnAction); 
    
    void OnSpawn(); 
    
    void OnDespawn(); 
}
