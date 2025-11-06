using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;
using Assets.PixelFantasy.PixelMonsters.Common.Scripts;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class AttackState : State
{
    private float attackCooldown; 
    // private float rushTimer = 0f;

    public AttackState(MonsterController controller) : base(controller) { }

    public override void Enter()
    {
        // Debug.Log("Attack 상태 진입!");
        // 공격 상태에 진입하면 일단 멈춤
        controller.StopMoving();
        attackCooldown = 1f / controller.GetMonsterData().monsterAttackRate;
        //controller.rushAlert.SetActive(true);
    }

    public override void Execute()
    {        
        attackCooldown -= Time.deltaTime;
        controller.spriteRenderer.flipX = controller.GetPlayerPosition().x < controller.transform.position.x;

        // 플레이어가 공격 범위를 벗어나면 추격 상태로 변경
        if (!controller.IsPlayerInAttackRange() && monster.Condition.canAttack == true)
        {
            //ResetRushAlert(controller.rushAlert.transform.localScale);           
            controller.ChangeState(controller.ChaseState);
            return;
        }
        // 플레이어가 공격범위 안에 있다면 러쉬알렛트 채워짐 MaxScaleY을 넘어서면 돌진공격 시작
        // 쿨다운이 끝나면 공격 실행
        if (attackCooldown <= 0) 
        {        
            Attack();           
        }
    }

    private void Attack()
    { 
        Debug.Log("플레이어 공격!");        
        if (controller.IsPlayerInAttackRange())
        { 
            StartAnimation(controller.AnimationData.AttackParameterHash);            
            attackCooldown = 1f / controller.GetMonsterData().monsterAttackRate;
            controller.ChangeState(controller.IdleState);
        }
    }

    //private void Attack()
    //{
    //    //Debug.Log("플레이어 공격!");
    //    // 여기에 실제 공격 로직 (데미지 전달 등)을 구현합니다.      
    //    Vector2 startPos = controller.transform.position;
    //    Vector2 direction = ((Vector2)PlayerManager.Instance.Player.transform.position - startPos).normalized;

    //    monster.monsterRigid.velocity = Vector2.zero; // 기존 속도 제거
    //    monster.monsterRigid.AddForce(direction * controller.rushSpeed * (monster.monsterRigid.mass) * (monster.monsterRigid.drag), ForceMode2D.Impulse);
    //    //만약 플레이어가 공격범위를 벗어나면 애니메이션 멈추기
    //    StartAnimation(controller.AnimationData.AttackParameterHash);
    //}

    //private void FillRushAlert(float deltaTime)
    //{
    //    if (controller.rushAlert == null) return;

    //    controller.rushAlert.SetActive(true);
    //    Vector2 dir = (PlayerManager.Instance.Player.transform.position - controller.transform.position).normalized;

    //    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
    //    controller.rushAlert.transform.rotation = Quaternion.Euler(0f, 0f, angle);

    //    rushTimer += deltaTime;
    //    float fillSpeed =1.5f; // 1초에 0->2
    //    Vector3 scale = controller.rushAlert.transform.localScale;
    //    scale.y = (rushTimer * fillSpeed);
    //    controller.rushAlert.transform.localScale = scale;        
    //    if (scale.y >= 0.5f)
    //    {
    //        ResetRushAlert(scale);   
    //        //Attack();
    //        StartAnimation(controller.AnimationData.IdleParameterHash);
    //        AudioManager.Instance.PlaySFX(monster.Controller.attackSound);
    //    }
    //}

    //private void ResetRushAlert(Vector3 Alert)
    //{        
    //    controller.rushAlert.SetActive(false);
    //    Alert.y = 0f;
    //    controller.rushAlert.transform.localScale = Alert;
    //    rushTimer = 0f;
    //    // 쿨다운 초기화
    //    attackCooldown = 1f / controller.GetMonsterData().monsterAttackRate;
    //}  

}
