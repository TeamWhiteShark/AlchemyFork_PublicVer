using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    private float _idleTime;
    
    public IdleState(MonsterController controller) : base(controller)
    {
    }
    
    public override void Enter()
    {
        // 몬스터 데이터로부터 대기 시간을 가져옴 (확장성)
        _idleTime = Random.Range(2f, 5f);
       // Debug.Log("Idle 상태 진입");
        StartAnimation(controller.AnimationData.IdleParameterHash);
    }

    public override void Execute()
    {
        _idleTime -= Time.deltaTime;

        // 플레이어가 감지 범위 안에 들어오면 추격 상태로 변경
        if (controller.IsPlayerInDetectionRange() && monster.Condition.canAttack == true)
        {
            controller.ChangeState(controller.ChaseState);
        }
        // 대기 시간이 끝나면 순찰 상태로 변경
        else if (_idleTime <= 0)
        {
            controller.ChangeState(controller.PatrolState);
        }
    }

    public override void Exit()
    {
        //Debug.Log("Idle 상태 종료");
        StopAnimation(controller.AnimationData.IdleParameterHash);
    }
}
