using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BootState : State
{
    private StateMachineBase _stateMachine;

    public BootState(StateMachineBase stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public override void Enter()
    {
        Debug.Log("enter boot state");

        _stateMachine.ChangeState<MainState>();
    }

    public override void Exit()
    {
        Debug.Log("exit boot state");
    }

    public class Factory : PlaceholderFactory<GlobalStateMachine, BootState>
    {

    }
}
