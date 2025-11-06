using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public ChaseState(MonsterController controller) : base(controller) { }

    public override void Enter()
    {
        //Debug.Log("Chase 상태 진입: 플레이어 추격!");
        StartAnimation(controller.AnimationData.RunParameterHash);
    }
    public override void Execute()
    {
        // 플레이어를 계속 따라감
        controller.MoveTo(controller.GetPlayerPosition());
        controller.spriteRenderer.flipX = controller.GetPlayerPosition().x < controller.transform.position.x;

        // 플레이어가 공격 범위에 들어오면 공격 상태로 전환
        if (controller.IsPlayerInAttackRange() && monster.Condition.canAttack == true)
        {
            //controller.ChangeState(new AttackState(controller));
            controller.ChangeState(controller.AttackState);
        }
        if (controller.IsPlayerInAttackRange() && monster.Condition.canAttack == false)
        {
            controller.ChangeState(controller.FleeState);
        }
        // 플레이어가 감지 범위를 벗어나면 다시 순찰 상태로 전환
        else if (!controller.IsPlayerInDetectionRange())
        {
            controller.ChangeState(controller.PatrolState);
        }
    }
    public override void Exit()
    {
         //Debug.Log("Chase 상태 종료");
         StopAnimation(controller.AnimationData.RunParameterHash);
    }
}