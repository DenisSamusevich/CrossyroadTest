using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.GameService;
using VContainer;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts.Services.Extensions
{
    internal static class IContainerBuilderExtensions
    {
        public static IContainerBuilder RegisterCommonService(this IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameService.GameService>(Lifetime.Singleton).AsSelf();
            builder.Register<GamesList>(Lifetime.Singleton);
            builder.Register(typeof(AsyncEntityService<,,,>), Lifetime.Singleton);
            builder.Register<AsyncEntityFactory>(Lifetime.Singleton);
            return builder;
        }
    }
}
