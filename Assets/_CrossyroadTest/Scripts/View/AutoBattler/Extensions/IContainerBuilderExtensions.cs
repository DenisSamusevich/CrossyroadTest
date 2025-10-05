using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Handlers;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts.View.AutoBattler.Extensions
{
    internal static class IContainerBuilderExtensions
    {
        public static IContainerBuilder RegisterViewAutoBattler(this IContainerBuilder builder,
            AB_AssetsSettings assetsSettings,
            AB_LevelSettings levelSettings,
            LevelView levelView,
            EnemyView enemyView,
            PlayerView playerView,
            LevelUpHandler levelUpHandler,
            OfferWeapomHandler offerWeapomHandler,
            GameMessageView gameMessageView
            )
        {
            builder.RegisterComponent(assetsSettings).AsImplementedInterfaces();
            builder.RegisterComponent(levelSettings).AsImplementedInterfaces();
            builder.RegisterComponent(levelView);
            builder.RegisterComponent(enemyView);
            builder.RegisterComponent(playerView);
            builder.RegisterComponent(levelUpHandler).As<IAsyncRequestHandler<LevelUpRequest, LevelUpResponse>>();
            builder.RegisterComponent(offerWeapomHandler).As<IAsyncRequestHandler<OfferWeaponRequest, OfferWeaponResponse>>();
            builder.RegisterComponent(gameMessageView);
            return builder;
        }
    }
}
