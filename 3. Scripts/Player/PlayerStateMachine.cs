using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어FSM
public class PlayerStateMachine : StateMachine
{
    public Player Player { get; }


    public PlayerIdleState PlayerIdleState { get; private set; }
    public PlayerMoveState PlayerMoveState { get; private set; }

    public PlayerDieState PlayerDieState { get; private set; }


    public PlayerStateMachine(Player Player)
    {
        this.Player = Player;

        PlayerIdleState = new PlayerIdleState(this);
        PlayerMoveState = new PlayerMoveState(this);
        PlayerDieState = new PlayerDieState(this);
    }
}
