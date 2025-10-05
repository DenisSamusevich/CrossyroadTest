using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level.Model
{
    using ObstaclesCreator = AsyncEntityService<IAreaData, ObstaclesContainer, IReadOnlyList<IObstacleData>, IEntityModel<IReadOnlyList<IObstacleData>, IObstaclesBehavior>>;
    using SimpleAreaCreator = AsyncEntityService<(int areaPosition, int levelWidth, AreaType areaType), SimpleArea, IAreaData, IEntityModel<IAreaData>>;

    public class LevelData : ILevelData, IInitAsyncElement<ITR_LevelSettings>
    {
        private readonly List<AreaType> _areaTypes;
        private readonly List<IAreaData> _areas;
        private readonly List<IObstacleData> _obstacles;
        private readonly List<IObstaclesBehavior> _obstaclesBehaviors;
        private readonly SimpleAreaCreator _simpleAreaCreator;
        private readonly ObstaclesCreator _obstaclesCreator;
        private int _levelWidth;
        private int _index = 0;

        public LevelData(SimpleAreaCreator simpleAreaCreator,
            ObstaclesCreator obstaclesCreator)
        {
            _simpleAreaCreator = simpleAreaCreator;
            _obstaclesCreator = obstaclesCreator;
            _areas = new List<IAreaData>();
            _areaTypes = new List<AreaType>();
            _obstacles = new List<IObstacleData>();
            _obstaclesBehaviors = new List<IObstaclesBehavior>();
        }

        public IReadOnlyList<IAreaData> Areas => _areas;
        public IReadOnlyList<IObstacleData> Obstacles => _obstacles;
        public int LevelWidth => _levelWidth;
        public IReadOnlyList<IObstaclesBehavior> ObstaclesBehaviors => _obstaclesBehaviors;

        async UniTask IInitAsyncElement<ITR_LevelSettings>.InitAsyncElement(ITR_LevelSettings levelSettings)
        {
            _levelWidth = levelSettings.LevelWidth;
            _areaTypes.AddRange(levelSettings.LevelAreas);
            foreach (var areaType in _areaTypes)
            {
                switch (areaType)
                {
                    case AreaType.Free:
                    case AreaType.Start:
                    case AreaType.Final:
                        var model = await _simpleAreaCreator.CreateEntityAsync((_index, _levelWidth, areaType));
                        _areas.Add(model.EntityData);
                        break;
                    case AreaType.Road:
                        var areaModel = await _simpleAreaCreator.CreateEntityAsync((_index, _levelWidth, areaType));
                        _areas.Add(areaModel.EntityData);
                        var obstaclesModel = await _obstaclesCreator.CreateEntityAsync(areaModel.EntityData);
                        _obstacles.AddRange(obstaclesModel.EntityData);
                        _obstaclesBehaviors.Add(obstaclesModel.EntityBehavior);
                        break;
                    default:
                        break;
                }
                _index++;
            }
        }

        void IDisposable.Dispose()
        {
            _simpleAreaCreator.Dispose();
            _obstaclesCreator.Dispose();
            _areas.Clear();
            _areaTypes.Clear();
            _obstacles.Clear();
            _obstaclesBehaviors.Clear();
            _index = 0;
        }
    }
}
