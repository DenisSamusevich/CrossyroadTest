using Assets._CrossyroadTest.Scripts.View.TwoBosses.Settings;
using VContainer;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses.Extensions
{
    internal static class IContainerBuilderExtensions
    {
        public static IContainerBuilder RegisterViewTwoBosses(this IContainerBuilder builder,
            TB_AssetsSettings assetsSettings,
            TB_LevelSettings levelSettings,
            BossView bossView, LevelView levelView, PlayerView playerView)
        {
            builder.RegisterComponent(assetsSettings).AsImplementedInterfaces();
            builder.RegisterComponent(levelSettings).AsImplementedInterfaces();
            builder.RegisterComponent(bossView);
            builder.RegisterComponent(levelView);
            builder.RegisterComponent(playerView);
            return builder;
        }
    }
}
