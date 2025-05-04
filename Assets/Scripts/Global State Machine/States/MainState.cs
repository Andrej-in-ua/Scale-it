using UI.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MainState : State
{
    private readonly StateMachineBase _stateMachine;
    private readonly GameMediator _gameMediator;

    public MainState(
        StateMachineBase stateMachine,
        GameMediator gameMediator
    )
    {
        _stateMachine = stateMachine;
        _gameMediator = gameMediator;
    }

    public override void Enter()
    {
        Debug.Log("enter main state");
        SceneManager.LoadScene(1);
        Subscribe();
        
        // _gameMediator.ConstructUI();
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
