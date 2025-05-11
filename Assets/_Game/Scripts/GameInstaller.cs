using _Game.Scripts;
using _Game.Scripts.Enemies;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private NormalEnemy normalPrefab;
    [SerializeField] private GrandmaEnemy grandmaPrefab;
    
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<GameSignals.OnPlayerDamageTaken>().OptionalSubscriber();
        Container.DeclareSignal<GameSignals.OnPlayerEarnPoint>().OptionalSubscriber();
        Container.DeclareSignal<GameSignals.OnPlayButtonPressed>().OptionalSubscriber();
        
        Container.BindMemoryPool<NormalEnemy, NormalEnemy.Pool>()
            .WithInitialSize(10)
            .FromComponentInNewPrefab(normalPrefab)
            .WithGameObjectName("Normal")
            .UnderTransformGroup("Normal_PoolHolder");
        
   
         Container.BindMemoryPool<GrandmaEnemy, GrandmaEnemy.Pool>()
             .WithInitialSize(10)
             .FromComponentInNewPrefab(grandmaPrefab)
             .WithGameObjectName("Grandma")
             .UnderTransformGroup("Grandma_PoolHolder");
        
    }
}