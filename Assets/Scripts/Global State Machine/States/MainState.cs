using UI.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MainState : State
{
    private const string SceneName = "Game Scene";

    private readonly StateMachineBase _stateMachine;
    private readonly IUIGameMediator _gameMediator;

    public MainState(
        StateMachineBase stateMachine,
        IUIGameMediator gameMediator
    )
    {
        _stateMachine = stateMachine;
        _gameMediator = gameMediator;
    }

    public override void Enter()
    {
        Debug.Log("enter main state");
        Subscribe();

        SceneManager.LoadScene(SceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == SceneName)
        {
            _gameMediator.ConstructUI();

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void Subscribe()
    {
        Application.quitting += Exit;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void Unsubscribe()
    {
        Application.quitting -= Exit;
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
