using Services;
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
        Container.Bind<IUIGameMediator>().To<UIGameMediator>().AsSingle();
        Container.Bind<GameTableMediator>().AsSingle();
    }
    
    private void BindUI()
    {
        Container.Bind<IUICardFactory>().To<UICardFactory>().AsSingle();
        Container.Bind<UIGameFactory>().AsSingle();
    }

    private void BindView()
    {
        Container.Bind<CardViewFactory>().AsSingle();
        Container.Bind<GridFactory>().AsSingle();
    }

    private void BindServices()
    {
        Container.Bind<IAssetProviderService>().To<AssetProviderService>().AsSingle();
        Container.Bind<InputService>().FromComponentInNewPrefabResource(Constants.InputServicePath).AsSingle();
    }
}
