using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseState : IState
{
    protected PlayerStateMachine stateMachine;

    public PlayerBaseState(PlayerStateMachine PlayerStateMachine)
    {
        this.stateMachine = PlayerStateMachine;
    }

    public virtual void Enter()
    {
    }

    public virtual void Execute()
    {
    }

    public virtual void Exit()
    {
    }
    
    protected void StartAnimation(int animatorHash)
    {
        stateMachine.Player.animator.SetBool(animatorHash, true);
    }

    protected void StopAnimation(int animatorHash)
    {
        stateMachine.Player.animator.SetBool(animatorHash, false);
    }
}
