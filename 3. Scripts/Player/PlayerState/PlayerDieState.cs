using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayerDieState : PlayerBaseState
{
    private BoxCollider2D playerCollider; // Declare playerCollider as a private field

    public PlayerDieState(PlayerStateMachine PlayerStateMachine) : base(PlayerStateMachine)
    {
    }

    public override void Enter()
    {
        playerCollider = PlayerManager.Instance.Player.GetComponent<BoxCollider2D>(); // Initialize playerCollider
        playerCollider.enabled = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Item"), true);
        PlayerManager.Instance.Player.animator.SetTrigger("Die");
    }
    public override void Exit()
    {
        playerCollider.enabled = true; // Use the field declared above
        PlayerManager.Instance.Player.animator.ResetTrigger("Die");
    }
}
