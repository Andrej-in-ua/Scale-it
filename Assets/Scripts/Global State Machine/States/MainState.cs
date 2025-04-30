using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MainState : State
{
    private StateMachineBase _stateMachine;

    public MainState(StateMachineBase stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public override void Enter()
    {
        Debug.Log("enter main state");
        SceneManager.LoadScene(1);
        Subscribe();
    }
    public void Subscribe()
    {
        Application.quitting += Exit;
    }

    public void Unsubscribe()
    {
        Application.quitting -= Exit;
    }

    public override void Exit()
    {
        Unsubscribe();
        Debug.Log("exit application");
    }

    public class Factory : PlaceholderFactory<GlobalStateMachine, MainState>
    {

    }
}
