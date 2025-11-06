using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IState
{
    public void Enter();
    public void Execute();
    public void Exit();
}

public class StateMachine
{
    public IState currentState;

    public void ChangeState(IState state)
    {
        currentState?.Exit();
        currentState = state;
        currentState?.Enter();
    }

    public void Execute()
    {
        currentState?.Execute();
    }
}
