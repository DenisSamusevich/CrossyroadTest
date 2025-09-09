using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Obstacle;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level.Area
{
    internal class RoadArea : IAreaData, IUpdateArea, IEntityInit<int>
    {
        private readonly EntityCreator<int, IReadOnlyList<IObstacleData>> _obstaclesCreator;
        private readonly List<IMoveObstacle> _obstaclesForMoving;
        private int _roadPosition;

        public RoadArea(EntityCreator<int, IReadOnlyList<IObstacleData>> obstaclesCreator)
        {
            _obstaclesCreator = obstaclesCreator;
            _obstaclesForMoving = new List<IMoveObstacle>();
        }

        public AreaType AreaType => AreaType.Road;

        public void Dispose()
        {
            _obstaclesCreator.Dispose();
            _obstaclesForMoving.Clear();
        }

        public void UpdateArea(float deltaTime)
        {
            foreach (var obstacle in _obstaclesForMoving)
            {
                obstacle.Move(deltaTime);
            }
        }

        public async UniTask InitAsync(int roadPosition, CancellationToken cancellationToken)
        {
            _roadPosition = roadPosition;
        }

        async UniTask IEntityInit<int>.PostInitAsync(CancellationToken cancellationToken)
        {
            var obstacles = await _obstaclesCreator.CreateEntityAsync<ObstaclesCreator>(_roadPosition, cancellationToken);
            foreach (var obstacle in obstacles)
            {
                if (obstacle is IMoveObstacle obstacleForMoving) _obstaclesForMoving.Add(obstacleForMoving);
            }
        }
    }
}
