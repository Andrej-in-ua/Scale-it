using StateMachine.Base;
using StateMachine.Global;
using StateMachine.Global.States;
using UnityEngine;
using Zenject;

public class Bootstrapper : MonoBehaviour
{
    private StateMachineBase _globalStateMachine;

    [Inject]
    public void Construct(GlobalStateMachine globalStateMachine)
    {
        _globalStateMachine = globalStateMachine;
    }

    private void Start()
    {
        _globalStateMachine.ChangeState<BootState>();
        DontDestroyOnLoad(this);
    }
}