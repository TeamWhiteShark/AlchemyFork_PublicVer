using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCAnimationData
{
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string runParameterName = "Run";
    [SerializeField] private string attackParameterName = "Attack";
    [SerializeField] private string hitParameterName = "Hit";
    [SerializeField] private string dieParameterName = "Die";
    [SerializeField] private string readyParameterName = "Ready";

    public int IdleParameterHash { get; private set; }
    public int RunParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }
    public int HitParameterHash { get; private set; }
    public int DieParameterHash { get; private set; }
    public int ReadyParameterHash { get; private set; }

    public void Initialize()
    {
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        RunParameterHash = Animator.StringToHash(runParameterName);
        AttackParameterHash = Animator.StringToHash(attackParameterName);
        HitParameterHash = Animator.StringToHash(hitParameterName);
        DieParameterHash = Animator.StringToHash(dieParameterName);
        ReadyParameterHash = Animator.StringToHash(readyParameterName);
    }

}
