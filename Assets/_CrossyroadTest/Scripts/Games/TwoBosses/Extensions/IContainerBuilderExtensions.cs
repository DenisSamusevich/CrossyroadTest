using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Model;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Model;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Interfases;
using Assets._CrossyroadTest.Scripts.Services.GameService.Base;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Extensions
{
    internal static class IContainerBuilderExtensions
    {
        public static IContainerBuilder RegisterTwoBossGame(this IContainerBuilder builder)
        {
            builder.Register<TB_Game>(Lifetime.Singleton).As<GameBase>()/*.Keyed(TB_Game.KeyGame)*/;
            builder.Register<TB_GameSession>(Lifetime.Singleton).As<ITB_GameSession>().AsSelf();
            builder.Register<TB_StatusGameChecker>(Lifetime.Singleton).As<IStatusGameChecker>().Keyed(TB_Game.KeyGame);
            builder.Register<Level>(Lifetime.Transient);
            builder.Register<LevelData>(Lifetime.Singleton);
            builder.Register<Player>(Lifetime.Transient);
            builder.Register<PlayerData>(Lifetime.Transient);
            builder.Register<PlayerBehavior>(Lifetime.Transient);
            builder.Register<BossData>(Lifetime.Transient);
            builder.Register<FirstBoss>(Lifetime.Transient);
            builder.Register<FirstBossBehavior>(Lifetime.Transient);
            builder.Register<SecondBoss>(Lifetime.Transient);
            builder.Register<SecondBossBehavior>(Lifetime.Transient);
            return builder;
        }
    }
}
