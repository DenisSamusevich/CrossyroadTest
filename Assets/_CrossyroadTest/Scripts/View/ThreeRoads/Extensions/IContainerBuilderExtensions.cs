using Assets._CrossyroadTest.Scripts.View.ThreeRoads.Settings;
using VContainer;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts.View.ThreeRoads.Extensions
{
    internal static class IContainerBuilderExtensions
    {
        public static IContainerBuilder RegisterViewThreeRoads(this IContainerBuilder builder,
            TR_AssetsSettings assetsSettings,
            TR_LevelSettings levelSettings,
            LevelView levelView, ObstaclesView obstaclesView, PlayerView playerView)
        {
            builder.RegisterComponent(assetsSettings).AsImplementedInterfaces();
            builder.RegisterComponent(levelSettings).AsImplementedInterfaces();
            builder.RegisterComponent(levelView);
            builder.RegisterComponent(obstaclesView);
            builder.RegisterComponent(playerView);
            return builder;
        }
    }
}
