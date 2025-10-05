using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles.Model
{
    public class ObstaclesData : IInitAsyncElement<IAreaData>, IReadOnlyList<IObstacleData>
    {
        public List<Obstacle> Obstacles { get; private set; }

        UniTask IInitAsyncElement<IAreaData>.InitAsyncElement(IAreaData areaData)
        {
            Obstacles = new List<Obstacle>();
            var flagDistance = false;
            for (float i = -0.5f; i < 1.5f; i += Random.Range(0.1f, 0.2f))
            {
                var obstacle = new Obstacle(1f, areaData.AreaPosition, i * areaData.LevelWidth, areaData.LevelWidth);
                Obstacles.Add(obstacle);
                if (flagDistance)
                {
                    i += 0.15f;
                }
                flagDistance = !flagDistance;
            }
            return UniTask.CompletedTask;
        }
        void System.IDisposable.Dispose() { Obstacles.Clear(); Obstacles = null; }

        int Count => Obstacles.Count;
        int IReadOnlyCollection<IObstacleData>.Count => Count;
        IObstacleData IReadOnlyList<IObstacleData>.this[int index] => Obstacles[index];
        IEnumerator<IObstacleData> IEnumerable<IObstacleData>.GetEnumerator() => Obstacles.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Obstacles.GetEnumerator();
    }
}
