using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFind
{
    public GameObject FindTarget(NPC npc, NPCStateMachine stateMachine,ItemSO item, ArchType archType);
}

public interface IMove
{
    public void Move(NPC npc, NPCStateMachine stateMachine);
}

public interface IWait
{
    public void Decide(NPC npc, NPCStateMachine stateMachine);
}

public interface IInteract
{
    public void Interact(NPC npc, NPCStateMachine stateMachine);
}

public class Strategy
{
    public IFind Find;
    public IMove Move;
    public IWait Wait;
    public IInteract Interact;
}

public class NPCStateMachine : StateMachine
{
    public NPC NPC { get; }
    public Strategy Strategy { get; }
    public Vector3 TargetPos { get; set; }


    public NPCIdleState NPCIdleState { get; private set; }
    public NPCMoveState NPCMoveState { get; private set; }
    public NPCWaitState NPCWaitState { get; private set; }
    public NPCInteractState NPCInteractState { get; private set; }
    
    public NPCStateMachine(NPC NPC, Strategy strategy)
    {
        this.NPC = NPC;
        this.Strategy = strategy;

        NPCIdleState = new NPCIdleState(this);
        NPCMoveState = new NPCMoveState(this);
        NPCWaitState = new NPCWaitState(this);
        NPCInteractState = new NPCInteractState(this);
    }
}
