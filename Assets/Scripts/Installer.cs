using Services;
using UI.Game;
using UI.Game.Inventory;
using Zenject;

public class Installer : MonoInstaller
{
    public override void InstallBindings()
    {
        BindGlobalStateMachine();
        BindMediator();
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
        Container.Bind<IUICardFactory>().To<UICardFactory>().AsSingle();
        Container.Bind<UIGameFactory>().AsSingle();
    }

    private void BindServices()
    {
        Container.Bind<IAssetProviderService>().To<AssetProviderService>().AsSingle();
    }
}
