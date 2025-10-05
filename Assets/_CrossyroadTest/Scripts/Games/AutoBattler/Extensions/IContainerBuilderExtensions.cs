using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Model;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Model;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Interfases;
using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.Services.GameService.Base;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Extensions
{
    internal static class IContainerBuilderExtensions
    {
        public static IContainerBuilder RegisterAutoBattlerGame(this IContainerBuilder builder)
        {
            builder.Register<AB_Game>(Lifetime.Singleton).As<GameBase>()/*.Keyed(TR_Game.KeyGame)*/;
            builder.Register<AB_Gameplay>(Lifetime.Singleton).As<IAB_Gameplay>();
            builder.Register<AB_GameSession>(Lifetime.Singleton).As<IAB_GameSession>().AsSelf();
            builder.Register<AB_StatusGameChecker>(Lifetime.Scoped).As<IStatusGameChecker, IAB_GameService>().Keyed(AB_Game.KeyGame);
            builder.Register<FeaturesFactory>(Lifetime.Singleton);

            builder.Register<Level>(Lifetime.Transient);
            builder.Register<LevelData>(Lifetime.Transient);
            builder.Register<Player>(Lifetime.Transient);
            builder.Register<PlayerData>(Lifetime.Transient);
            builder.Register<PlayerBehavior>(Lifetime.Transient);
            builder.Register<Enemy>(Lifetime.Transient);
            builder.Register<EnemyData>(Lifetime.Transient);
            builder.Register<EnemyBehavior>(Lifetime.Transient);
            builder.Register<BoneForm>(Lifetime.Transient);
            builder.Register<DragonAttack>(Lifetime.Transient);
            builder.Register<GelForm>(Lifetime.Transient);
            builder.Register<HiddenAttack>(Lifetime.Transient);
            builder.Register<ImpulsesToAction>(Lifetime.Transient);
            builder.Register<Poison>(Lifetime.Transient);
            builder.Register<PoisonAttack>(Lifetime.Transient);
            builder.Register<Rage>(Lifetime.Transient);
            builder.Register<Shield>(Lifetime.Transient);
            builder.Register<StoneSkin>(Lifetime.Transient);
            return builder;
        }
    }
}
