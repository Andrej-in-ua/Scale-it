using UnityEngine;
using Zenject;

public class Bootstraper : MonoBehaviour
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