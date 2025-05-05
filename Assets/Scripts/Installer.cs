using Services;
using UI.Game;
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
        Container.Bind<IGameMediator>().To<GameMediator>().AsSingle();
        Container.Bind<IUICardFactory>().To<UICardFactory>().AsSingle();
    }

    private void BindServices()
    {
        Container.Bind<IAssetProviderService>().To<AssetProviderService>().AsSingle();
    }
}
