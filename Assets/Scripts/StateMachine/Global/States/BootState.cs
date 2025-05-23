using StateMachine.Base;
using UnityEngine;
using Zenject;

namespace StateMachine.Global.States
{
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
}
