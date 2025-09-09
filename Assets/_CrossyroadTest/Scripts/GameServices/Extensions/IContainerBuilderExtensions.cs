using Assets._CrossyroadTest.GameServices.Servises;
using Assets._CrossyroadTest.Scripts.GameServices.Servises.Camera;
using VContainer;
using VContainer.Unity;

namespace Assets._CrossyroadTest.GameServices.Extensions
{
    internal static class IContainerBuilderExtensions
    {
        public static IContainerBuilder RegisterGameServices(this IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameService>(Lifetime.Singleton).AsSelf();
            builder.Register<CameraService>(Lifetime.Singleton).As<ICameraService>();
            builder.Register<GamesList>(Lifetime.Singleton);
            return builder;
        }
    }
}
