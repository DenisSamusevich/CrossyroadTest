using Assets._CrossyroadTest.Scripts.Common.Creators;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Obstacle
{
    internal class ObstaclesCreator : IReadOnlyList<IObstacleData>, IEntityInit<int>
    {
        private readonly List<IObstacleData> _obstacles;

        public ObstaclesCreator()
        {
            _obstacles = new List<IObstacleData>();
        }

        public async UniTask InitAsync(int roadPosition, CancellationToken cancellationToken)
        {
            var flagDistance = false;
            for (float i = -0.5f; i < 1.5f; i += Random.Range(0.1f, 0.2f))
            {
                var obstacle = new Obstacle(0.1f, roadPosition, i);
                _obstacles.Add(obstacle);
                if (flagDistance)
                {
                    i += 0.15f;
                }
                flagDistance = !flagDistance;
            }
            await UniTask.CompletedTask;
        }

        public int Count => _obstacles.Count;

        public IObstacleData this[int index] => _obstacles[index];

        public IEnumerator<IObstacleData> GetEnumerator() => _obstacles.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
