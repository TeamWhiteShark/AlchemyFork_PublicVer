using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    private Vector3 randomPosition;
    private float patroltime = 3f;
    private float changeTime = 0f;

    public PatrolState(MonsterController controller) : base(controller) { }

    public override void Enter()
    {
        changeTime = 0f;
        // 순찰할 무작위 위치 설정
        float randomX = Random.Range(-2f, 2f);
        float randomY = Random.Range(-2f, 2f);
        randomPosition = controller.transform.position + new Vector3(randomX, randomY, 0);
        controller.spriteRenderer.flipX = randomX < 0;
        //float randomX, randomY;
        //do
        //{
        //    randomX = Random.Range(-2f, 2f);
        //    randomY = Random.Range(-2f, 2f);
        //    randomPosition = controller.transform.position + new Vector3(randomX, randomY, 0);
        //}
        //while (randomX >= controller.minPos.x && randomX <= controller.maxPos.x && randomY >= controller.minPos.y && randomY <= controller.maxPos.y);


        controller.MoveTo(randomPosition);
        //Debug.Log("Patrol 상태 진입");
        StartAnimation(controller.AnimationData.RunParameterHash);
    }

    public override void Execute()
    {
        changeTime += Time.deltaTime;
        // 플레이어가 감지되면 추격 상태로 전환
        if (controller.IsPlayerInDetectionRange() && monster.Condition.canAttack == true)
        {
            controller.ChangeState(new ChaseState(controller));
        }
        // 목표 지점에 도착하면 대기 상태로 전환
        else if (Vector3.Distance(controller.transform.position, randomPosition) < 0.1f || changeTime >= patroltime)
        {
            controller.ChangeState(new IdleState(controller));
        }
        
    }

    public override void Exit()
    {
        //Debug.Log("Patrol 상태 종료");
        StopAnimation(controller.AnimationData.RunParameterHash);
    }
}