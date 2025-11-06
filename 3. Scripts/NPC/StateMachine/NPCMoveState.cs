using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMoveState : NPCBaseState
{
    public override string Name { get => "Move"; set{} }
    
    public NPCMoveState(NPCStateMachine NPCStateMachine) : base(NPCStateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.NPC.NPCState = Name;
        StartAnimation(stateMachine.NPC.NpcAnimationData.RunParameterHash);
    }

    public override void Execute()
    {
        stateMachine.Strategy.Move.Move(stateMachine.NPC, stateMachine);             
    }

    public override void Exit()
    {
        StopAnimation(stateMachine.NPC.NpcAnimationData.RunParameterHash);
    }   

}
