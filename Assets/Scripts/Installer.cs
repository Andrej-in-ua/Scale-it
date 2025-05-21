using Controllers;
using Services;
using Services.Input;
using StateMachine.Global;
using StateMachine.Global.States;
using UI.Game;
using View.GameTable;
using Zenject;

public class Installer : MonoInstaller
{
    public override void InstallBindings()
    {
        BindGlobalStateMachine();
        BindMediator();
        BindUI();
        BindView();
        BindServices();
    }

    private void BindGlobalStateMachine()
    {
        Container.Bind<GlobalStateMachine>().AsSingle();
        Container.BindFactory<GlobalStateMachine, BootState, BootState.Factory>().AsSingle();
        Container.BindFactory<GlobalStateMachine, MainState, MainState.Factory>().AsSingle();
    }

    private void BindMediator()
    {
        Container.Bind<CardDragController>().AsSingle();

        Container.Bind<UIGameMediator>().AsSingle();
        Container.Bind<GameTableMediator>().AsSingle();
    }

    private void BindUI()
    {
        Container.Bind<UICardFactory>().AsSingle();
        Container.Bind<UIGameFactory>().AsSingle();
    }

    private void BindView()
    {
        Container.Bind<CardViewFactory>().AsSingle();
        Container.Bind<CardViewPool>().AsSingle();
        Container.Bind<GridFactory>().AsSingle();
        Container.Bind<GridManager>().AsSingle();
    }

    private void BindServices()
    {
        Container.Bind<PlayerInputActions>().AsSingle().NonLazy();
        Container.Bind<InputService>().AsSingle();
        Container.Bind<DragService>().AsSingle();

        Container.Bind<IAssetProviderService>().To<AssetProviderService>().AsSingle();
    }
}
