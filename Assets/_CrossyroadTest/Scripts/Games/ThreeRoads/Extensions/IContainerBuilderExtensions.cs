using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area.Model;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level.Model;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles.Model;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.GameService.Base;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Extensions
{
    internal static class IContainerBuilderExtensions
    {
        public static IContainerBuilder RegisterThreeRoadsGame(this IContainerBuilder builder)
        {
            builder.Register<TR_Game>(Lifetime.Singleton).As<GameBase>()/*.Keyed(TR_Game.KeyGame)*/;
            builder.Register<TR_GameSession>(Lifetime.Singleton).As<ITR_GameSession>().AsSelf();
            builder.Register<TR_StatusGameChecker>(Lifetime.Singleton).As<IStatusGameChecker>().Keyed(TR_Game.KeyGame);

            builder.Register<Level>(Lifetime.Transient);
            builder.Register<LevelData>(Lifetime.Transient);
            builder.Register<LevelBehavior>(Lifetime.Transient);
            builder.Register<SimpleArea>(Lifetime.Transient);
            builder.Register<SimpleAreaData>(Lifetime.Transient);
            builder.Register<ObstaclesContainer>(Lifetime.Transient);
            builder.Register<ObstaclesData>(Lifetime.Transient);
            builder.Register<ObstaclesBehavior>(Lifetime.Transient);
            builder.Register<Player>(Lifetime.Transient);
            builder.Register<PlayerData>(Lifetime.Transient);
            builder.Register<PlayerBehavior>(Lifetime.Transient);
            return builder;
        }
    }
}
