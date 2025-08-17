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
        builder.RegisterComponent(swordsmanUnitPrefab).AsImplementedInterfaces().AsSelf();
        builder.RegisterComponent(archerUnitPrefab).AsImplementedInterfaces().AsSelf();
        builder.RegisterComponent(spearmanUnitPrefab).AsImplementedInterfaces().AsSelf();

        builder.Register<SelectionManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<LevelController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
    }
}