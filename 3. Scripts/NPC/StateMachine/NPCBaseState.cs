using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class NPCBaseState : IState
{
    protected NPCStateMachine stateMachine;
    
    public NPCBaseState(NPCStateMachine NPCStateMachine)
    {
        this.stateMachine = NPCStateMachine;
    }

    public virtual string Name { get; set; }

    public virtual void Enter()
    {
    }

    public virtual void Execute()
    {
    }

    public virtual void Exit()
    {
    }

    public void StartAnimation(int animatorHash)
    {
        if (animatorHash == stateMachine.NPC.NpcAnimationData.AttackParameterHash)
        {
            stateMachine.NPC.animator.SetTrigger(animatorHash);
        }
        else
        {
            stateMachine.NPC.animator.SetBool(animatorHash, true);
        }
    }

    public void StopAnimation(int animatorHash)
    {
        stateMachine.NPC.animator.SetBool(animatorHash, false);
    }
}
