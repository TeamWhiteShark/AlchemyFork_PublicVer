using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected MonsterController controller;

    public State(MonsterController controller)
    {
        this.controller = controller;
    }
    
    public virtual void Enter() { } // 상태에 진입했을 때 한 번 호출
    public virtual void Execute() { } // 상태가 활성화되어 있는 동안 매 프레임 호출
    public virtual void Exit() { }  // 상태를 빠져나갈 때 한 번 호출
    public Monster monster => controller.GetComponent<Monster>();

    public void StartAnimation(int animatorHash)
    {
        if (animatorHash == controller.AnimationData.AttackParameterHash)
        {
            controller.Animator.SetTrigger(animatorHash);
        }
        else
        {
            controller.Animator.SetBool(animatorHash, true);
        }
    }
    public void StopAnimation(int animatorHash)
    {
        if (animatorHash == controller.AnimationData.AttackParameterHash)
        {
            controller.Animator.ResetTrigger(animatorHash);
        }
        else
        {
            controller.Animator.SetBool(animatorHash, false);
        }       
    }
}
