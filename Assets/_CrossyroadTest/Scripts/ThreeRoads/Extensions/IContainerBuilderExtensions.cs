using Assets._CrossyroadTest.GameServices.Base;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level.Area;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Obstacle;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Player;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.ThreeRoads.Extensions
{
    internal static class IContainerBuilderExtensions
    {
        public static IContainerBuilder RegisterThreeRoadsGame(this IContainerBuilder builder)
        {
            builder.Register<ThreeRoadsGame>(Lifetime.Singleton).As<GameBase>().Keyed(1);
            builder.Register<Level>(Lifetime.Transient);
            builder.Register<LevelBuilder>(Lifetime.Transient);
            builder.Register<SimpleArea>(Lifetime.Transient);
            builder.Register<RoadArea>(Lifetime.Transient);
            builder.Register<ObstaclesCreator>(Lifetime.Transient);
            builder.Register<Player>(Lifetime.Transient);
            return builder;
        }
    }
}
