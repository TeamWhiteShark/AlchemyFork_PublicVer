using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractState : NPCBaseState
{
    public override string Name { get => "Interact"; set{} }
    public NPCInteractState(NPCStateMachine NPCStateMachine) : base(NPCStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.NPC.NPCState = Name;
    }

    public override void Execute()
    {
        stateMachine.Strategy.Interact.Interact(stateMachine.NPC, stateMachine);
    }
}
