using Assets._CrossyroadTest.GameServices.Base;
using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Bosses;
using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Level;
using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Player;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.TwoBosses.Extensions
{
    internal static class IContainerBuilderExtensions
    {
        public static IContainerBuilder RegisterTwoBossGame(this IContainerBuilder builder)
        {
            builder.Register<BattleWithTwoBosses>(Lifetime.Singleton).As<GameBase>();
            builder.Register<Level>(Lifetime.Transient);
            builder.Register<Player>(Lifetime.Transient);
            builder.Register<FirstBoss>(Lifetime.Transient);
            builder.Register<SecondBoss>(Lifetime.Transient);
            return builder;
        }
    }
}
