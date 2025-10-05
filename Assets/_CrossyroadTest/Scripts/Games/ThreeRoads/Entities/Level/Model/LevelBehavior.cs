using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level.Model
{
    public class LevelBehavior : ILevelBehavior, IInitAsyncElement<ITR_LevelSettings, LevelData>
    {
        private IReadOnlyList<IObstaclesBehavior> _obstaclesForUpdate;

        public void Update()
        {
            foreach (var obstacles in _obstaclesForUpdate)
            {
                obstacles.MoveObstacles(Time.deltaTime);
            }
        }

        UniTask IInitAsyncElement<ITR_LevelSettings, LevelData>.InitAsyncElement(ITR_LevelSettings data1, LevelData data2)
        {
            _obstaclesForUpdate = data2.ObstaclesBehaviors;
            return UniTask.CompletedTask;
        }
        void IDisposable.Dispose()
        {
            _obstaclesForUpdate = null;
        }
    }
}
