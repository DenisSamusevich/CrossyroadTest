using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles.Model
{
    public class ObstaclesBehavior : IInitAsyncElement<IAreaData, ObstaclesData>, IObstaclesBehavior
    {
        private List<IMoveObstacle> _obstaclesForMoving = new List<IMoveObstacle>();

        public UniTask InitAsyncElement(IAreaData areaData, ObstaclesData obstaclesData)
        {

            _obstaclesForMoving.AddRange(obstaclesData.Obstacles);
            return UniTask.CompletedTask;
        }

        public void MoveObstacles(float deltaTime)
        {
            foreach (var obstacle in _obstaclesForMoving)
            {
                obstacle.Move(deltaTime);
            }
        }
    }
}
