using System;
using System.Collections.Generic;
using Unit;
using Unit.Archer;
using Unit.Swordsman;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private SwordsmanUnit swordsmanUnitPrefab;
        [SerializeField] private ArcherUnit archerUnitPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(gridManager);
            builder.RegisterComponent(swordsmanUnitPrefab).AsImplementedInterfaces().AsSelf();
            builder.RegisterComponent(archerUnitPrefab).AsImplementedInterfaces().AsSelf();

            builder.Register<SelectionManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<LevelController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}