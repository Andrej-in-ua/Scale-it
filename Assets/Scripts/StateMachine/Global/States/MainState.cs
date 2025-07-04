using Controllers;
using DeckManager;
using Services;
using Services.Input;
using StateMachine.Base;
using UI.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using View.GameTable;
using Zenject;

namespace StateMachine.Global.States
{
    public class MainState : State
    {
        private const string SceneName = "Game Scene";

        private readonly StateMachineBase _stateMachine;
        private readonly UIGameMediator _uiGameMediator;
        private readonly GameTableMediator _gameTableMediator;
        private readonly InputService _inputService;
        private readonly CameraMover _cameraMover;
        private readonly DragService _dragService;
        private readonly CardDragController _cardDragController;

        public MainState(
            StateMachineBase stateMachine,
            UIGameMediator uiGameMediator,
            GameTableMediator gameTableMediator,
            InputService inputService,
            CameraMover cameraMover,
            DragService dragService,
            CardDragController cardDragController
        )
        {
            _stateMachine = stateMachine;
            _uiGameMediator = uiGameMediator;
            _gameTableMediator = gameTableMediator;
            _inputService = inputService;
            _cameraMover = cameraMover;
            _dragService = dragService;
            _cardDragController = cardDragController;
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
                var camera = Camera.main;
                
                _gameTableMediator.ConstructGameTable(camera);
                _uiGameMediator.ConstructUI();

                _inputService.Construct(camera);
                _cameraMover.Construct(camera);

                _dragService.Construct();

                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        private void Subscribe()
        {
            Application.quitting += Exit;
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            _cameraMover.OnCameraMove += _gameTableMediator.HandleCameraMove;
            
            SubscribeCardDragController();
            SubscribeConnectionDrawing();
        }

        private void SubscribeConnectionDrawing()
        {
            _dragService.OnStartDrag += _gameTableMediator.HandleStartDraw;
            _dragService.OnDrag += _gameTableMediator.HandleDraw;
            _dragService.OnStopDrag += _gameTableMediator.HandleStopDraw;
        } 
        
        private void SubscribeCardDragController()
        {
            // CardDragController relations
            // ... mouse enter on inventory
            _inputService.OnMouseMove += _uiGameMediator.HandleMouseMove;
            _uiGameMediator.OnMouseEnterInventory += _cardDragController.HandleMouseEnterInventory;
            _uiGameMediator.OnMouseLeaveInventory += _cardDragController.HandleMouseLeaveInventory;
            
            // ... interceptions
            _uiGameMediator.OnCardDragRollback += _cardDragController.HandleCardDragRollback;
            _gameTableMediator.OnCardDragRollback += _cardDragController.HandleCardDragRollback;

            // ... mouse inputs
            
            _dragService.OnStartDrag += _cardDragController.HandleStartDrag;
            _dragService.OnDrag += _cardDragController.HandleDrag;
            _dragService.OnStopDrag += _cardDragController.HandleStopDrag;

            // ... CardDragController <-> GameTableMediator
            _cardDragController.OnStartDrag += _gameTableMediator.HandleStartDrag;
            _cardDragController.OnDrag += _gameTableMediator.HandleDrag;
            _cardDragController.OnChangeToPreview += _gameTableMediator.HandleChangeToPreview;
            _cardDragController.OnChangeToView += _gameTableMediator.HandleChangeToView;
            _cardDragController.OnStopDrag += _gameTableMediator.HandleStopDrag;
            _cardDragController.OnRollback += _gameTableMediator.HandleRollback;

            // ... CardDragController <-> UIMediator
            _cardDragController.OnStartDrag += _uiGameMediator.HandleStartDrag;
            _cardDragController.OnDrag += _uiGameMediator.HandleDrag;
            _cardDragController.OnChangeToPreview += _uiGameMediator.HandleChangeToPreview;
            _cardDragController.OnChangeToView += _uiGameMediator.HandleChangeToView;
            _cardDragController.OnStopDrag += _uiGameMediator.HandleStopDrag;
            _cardDragController.OnRollback += _uiGameMediator.HandleRollback;
        }
        
        private void Unsubscribe()
        {
            Application.quitting -= Exit;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            _cameraMover.OnCameraMove -= _gameTableMediator.HandleCameraMove;
            
            UnsubscribeCardDagController();
            UnsubscribeConnectionDrawing();
        }

        private void UnsubscribeCardDagController()
        {
            // CardDragController relations
            // ... mouse enter on inventory
            _uiGameMediator.OnMouseEnterInventory -= _cardDragController.HandleMouseEnterInventory;
            _uiGameMediator.OnMouseLeaveInventory -= _cardDragController.HandleMouseLeaveInventory;
            
            // ... interceptions
            _uiGameMediator.OnCardDragRollback -= _cardDragController.HandleCardDragRollback;
            _gameTableMediator.OnCardDragRollback -= _cardDragController.HandleCardDragRollback;

            // ... mouse inputs
            _dragService.OnStartDrag -= _cardDragController.HandleStartDrag;
            _dragService.OnDrag -= _cardDragController.HandleDrag;
            _dragService.OnStopDrag -= _cardDragController.HandleStopDrag;
            
            // ... CardDragController <-> GameTableMediator
            _cardDragController.OnStartDrag -= _gameTableMediator.HandleStartDrag;
            _cardDragController.OnDrag -= _gameTableMediator.HandleDrag;
            _cardDragController.OnChangeToPreview -= _gameTableMediator.HandleChangeToPreview;
            _cardDragController.OnChangeToView -= _gameTableMediator.HandleChangeToView;
            _cardDragController.OnStopDrag -= _gameTableMediator.HandleStopDrag;
            _cardDragController.OnRollback -= _gameTableMediator.HandleRollback;

            // ... CardDragController <-> UIMediator
            _cardDragController.OnStartDrag -= _uiGameMediator.HandleStartDrag;
            _cardDragController.OnDrag -= _uiGameMediator.HandleDrag;
            _cardDragController.OnChangeToPreview -= _uiGameMediator.HandleChangeToPreview;
            _cardDragController.OnChangeToView -= _uiGameMediator.HandleChangeToView;
            _cardDragController.OnStopDrag -= _uiGameMediator.HandleStopDrag;
            _cardDragController.OnRollback -= _uiGameMediator.HandleRollback;
        }

        private void UnsubscribeConnectionDrawing()
        {
            _dragService.OnStartDrag -= _gameTableMediator.HandleStartDraw;
            _dragService.OnDrag -= _gameTableMediator.HandleDraw;
            _dragService.OnStopDrag -= _gameTableMediator.HandleStopDraw;
        }

        public override void Exit()
        {
            _gameTableMediator.DestructGameTable();
            _uiGameMediator.Dispose();
            Unsubscribe();
            
            _cameraMover.Dispose();
            _dragService.Dispose();
            _inputService.Dispose();

            Debug.Log("exit application");
        }

        public class Factory : PlaceholderFactory<GlobalStateMachine, MainState>
        {
        }
    }
}
