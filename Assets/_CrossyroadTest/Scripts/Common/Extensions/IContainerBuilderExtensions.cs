using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.Common.Input;
using VContainer;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts.Common.Extensions
{
    internal static class IContainerBuilderExtensions
    {
        public static IContainerBuilder RegisterCommonService(this IContainerBuilder builder)
        {
            builder.Register(typeof(EntityCreator<,>), Lifetime.Singleton);
            builder.RegisterEntryPoint<InputService>(Lifetime.Singleton).AsSelf();
            return builder;
        }
    }
}
