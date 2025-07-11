using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private GridManager gridManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(gridManager);

            builder.Register<SelectionManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}