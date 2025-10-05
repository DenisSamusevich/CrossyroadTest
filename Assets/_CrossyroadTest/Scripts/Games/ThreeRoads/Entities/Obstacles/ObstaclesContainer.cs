using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles.Model;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using System.Collections.Generic;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles
{
    using AEC_Obstacles = AsyncEntityCharacteristic<IAreaData, ObstaclesData, IReadOnlyList<IObstacleData>, ObstaclesBehavior, IObstaclesBehavior>;

    public class ObstaclesContainer : AEC_Obstacles.AsyncEntity
    {
        public ObstaclesContainer(AsyncEntityFactory entityFactory) : base(entityFactory)
        {
        }
    }
}
