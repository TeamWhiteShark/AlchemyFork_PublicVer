using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWaitState : NPCBaseState
{
    public override string Name { get => "Wait"; set{} }
    
    public NPCWaitState(NPCStateMachine NPCStateMachine) : base(NPCStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.NPC.NPCState = Name;
        if(!stateMachine.NPC.satisfied)
            stateMachine.NPC.waitCoroutine = stateMachine.NPC.StartCoroutine(stateMachine.NPC.Wait());
        StartAnimation(stateMachine.NPC.NpcAnimationData.IdleParameterHash);
    }

    public override void Execute()
    {
        stateMachine.Strategy.Wait.Decide(stateMachine.NPC, stateMachine);
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.NPC.StopCoroutine(stateMachine.NPC.waitCoroutine);
        StopAnimation(stateMachine.NPC.NpcAnimationData.IdleParameterHash);
    }
}
