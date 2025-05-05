using UI.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MainState : State
{
    private readonly StateMachineBase _stateMachine;
    private readonly IGameMediator _gameMediator;

    public MainState(
        StateMachineBase stateMachine,
        IGameMediator gameMediator
    )
    {
        _stateMachine = stateMachine;
        _gameMediator = gameMediator;
    }

    public override void Enter()
    {
        Debug.Log("enter main state");
        Subscribe();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(1);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            _gameMediator.ConstructUI();

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
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
