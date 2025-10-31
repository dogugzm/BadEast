using Core;
using Unit.Archer;
using Unit.Spearman;
using Unit.Swordsman;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private SwordsmanUnit swordsmanUnitPrefab;
    [SerializeField] private ArcherUnit archerUnitPrefab;
    [SerializeField] private SpearmanUnit spearmanUnitPrefab;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(gridManager);

        // Register Type-Safe Object Pools and expose them as IPool
        // Swordsman Pool
        builder.Register(resolver =>
        {
            var factory = new MonoBehaviourFactory<SwordsmanUnit>(resolver, swordsmanUnitPrefab);
            return new ObjectPool<SwordsmanUnit>(factory, 10); // Initial size 10
        }, Lifetime.Singleton).As<IPool>();

        // Archer Pool
        builder.Register(resolver =>
        {
            var factory = new MonoBehaviourFactory<ArcherUnit>(resolver, archerUnitPrefab);
            return new ObjectPool<ArcherUnit>(factory, 10); // Initial size 10
        }, Lifetime.Singleton).As<IPool>();

        // Spearman Pool
        builder.Register(resolver =>
        {
            var factory = new MonoBehaviourFactory<SpearmanUnit>(resolver, spearmanUnitPrefab);
            return new ObjectPool<SpearmanUnit>(factory, 10); // Initial size 10
        }, Lifetime.Singleton).As<IPool>();

        // Register the PoolManager
        builder.Register<PoolManager>(Lifetime.Singleton);

        builder.Register<SelectionManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<LevelController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
    }
}