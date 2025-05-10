using _Game.Scripts;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<GameSignals.OnPlayerDamageTaken>().OptionalSubscriber();
        Container.DeclareSignal<GameSignals.OnPlayerEarnPoint>().OptionalSubscriber();
    }
}