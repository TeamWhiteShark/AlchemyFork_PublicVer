using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : State
{
    private float fleeDuration = 5f; // 도망 지속 시간

    public FleeState(MonsterController controller) : base(controller) { }

    public override void Enter()
    {
        //Debug.Log("Flee 상태 진입: 도망!");
        Vector3 playerPos = controller.GetPlayerPosition();
        Vector3 fleeDirection = (controller.transform.position - playerPos).normalized;
        Vector3 fleeDestination = controller.transform.position + fleeDirection * 10f; // 10 유닛만큼 멀리 도망
        controller.spriteRenderer.flipX = fleeDirection.x < 0; // 도망 방향에 따라 스프라이트 뒤집기

        controller.MoveTo(fleeDestination);
        StartAnimation(controller.AnimationData.RunParameterHash);
    }

    public override void Execute()
    {
        fleeDuration -= Time.deltaTime;

        // 도망 시간이 끝나거나, 플레이어가 감지 범위를 벗어나면 다시 순찰 상태로 복귀
        if (fleeDuration <= 0f || !controller.IsPlayerInDetectionRange())
        {
            controller.ChangeState(controller.PatrolState);
        }
    }
    public override void Exit()
    {
         //Debug.Log("Flee 상태 종료"); 
         StopAnimation(controller.AnimationData.RunParameterHash);
    }
}