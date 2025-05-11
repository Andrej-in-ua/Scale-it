using DeckManager;
using Services;
using UI.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using View.GameTable;
using Zenject;

public class MainState : State
{
    private const string SceneName = "Game Scene";

    private readonly StateMachineBase _stateMachine;
    private readonly IUIGameMediator _uiGameMediator;
    private readonly GameTableMediator _gameTableMediator;
    private readonly InputService _inputService;

    public MainState(
        StateMachineBase stateMachine,
        IUIGameMediator uiGameMediator,
        GameTableMediator gameTableMediator,
        InputService inputService
    )
    {
        _stateMachine = stateMachine;
        _uiGameMediator = uiGameMediator;
        _gameTableMediator = gameTableMediator;
        _inputService = inputService;
    }

    public override void Enter()
    {
        Debug.Log("enter main state");
        Subscribe();

        DeckLoader.LoadAsGlobal(Constants.DefaultDeckName);
        SceneManager.LoadScene(SceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == SceneName)
        {
            _gameTableMediator.ConstructGameTable();
            _uiGameMediator.ConstructUI();

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
        _gameTableMediator.DestructGameTable();
        Unsubscribe();
        Debug.Log("exit application");
    }

    public class Factory : PlaceholderFactory<GlobalStateMachine, MainState>
    {

    }
}
