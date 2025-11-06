using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCIdleState : NPCBaseState
{
    GameObject target;
    public override string Name { get => "Idle"; set{} }

    public NPCIdleState(NPCStateMachine NPCStateMachine) : base(NPCStateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.NPC.NPCState = Name;
        StartAnimation(stateMachine.NPC.NpcAnimationData.IdleParameterHash);
    }


    public override void Execute()
    {
        if (stateMachine.NPC.targetObj == null)
        {
            target = stateMachine.Strategy.Find.FindTarget(stateMachine.NPC, stateMachine, stateMachine.NPC.targetItem,
                stateMachine.NPC.targetArchType);
            stateMachine.NPC.targetObj = target;
        }
        else
            stateMachine.ChangeState(stateMachine.NPCMoveState);
    }
    public override void Exit()
    {
        StopAnimation(stateMachine.NPC.NpcAnimationData.IdleParameterHash);
    }
}
