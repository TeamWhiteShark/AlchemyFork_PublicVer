using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickUI : UIBase
{
    public override bool isDestroy => false;
    
    public PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponentInChildren<PlayerController>();
    }
}
