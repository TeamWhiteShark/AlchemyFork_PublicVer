using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : State
{
    public DieState(MonsterController controller) : base(controller)
    {
    }

    public override void Enter()
    {            
        StopAnimation(controller.AnimationData.IdleParameterHash);
        StopAnimation(controller.AnimationData.AttackParameterHash);
        controller.StopMoving();        
        StartAnimation(controller.AnimationData.DieParameterHash);        
    }    
}
