using VContainer;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<SelectionManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}