using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine PlayerStateMachine) : base(PlayerStateMachine)
    {
    }
    public override void Enter()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Item"), false);
        if (PlayerManager.Instance.Player.InDungeon)
        {
            PlayerManager.Instance.Player.animator.SetTrigger("Idle");
        }
        else
        {
            PlayerManager.Instance.Player.animator.SetTrigger("BroomRiding");
        }
    }

    public override void Exit()
    {
        PlayerManager.Instance.Player.animator.ResetTrigger("Idle");
        PlayerManager.Instance.Player.animator.ResetTrigger("BroomRiding");
    }
}
