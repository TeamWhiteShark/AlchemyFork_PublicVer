using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine PlayerStateMachine) : base(PlayerStateMachine)
    {
    }

    public override void Enter()
    {
        if (PlayerManager.Instance.Player.InDungeon)
        {
            PlayerManager.Instance.Player.animator.SetTrigger("Move");
        }
        else
        {
            PlayerManager.Instance.Player.animator.SetTrigger("BroomRiding");
        }
    }

    public override void Exit()
    {
        PlayerManager.Instance.Player.animator.ResetTrigger("BroomRiding");
        PlayerManager.Instance.Player.animator.ResetTrigger("Move");
    }
}
