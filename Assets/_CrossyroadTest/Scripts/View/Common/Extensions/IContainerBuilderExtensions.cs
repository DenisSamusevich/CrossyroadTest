using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.Common.Extensions
{
    internal static class IContainerBuilderExtensions
    {
        public static IContainerBuilder RegisterViewCommon(this IContainerBuilder builder)
        {
            builder.Register<PlayerViewContainer>(Lifetime.Singleton);
            return builder;
        }
    }
}
