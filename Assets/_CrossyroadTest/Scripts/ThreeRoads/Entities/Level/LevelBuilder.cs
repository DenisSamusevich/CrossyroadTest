using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level.Area;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level
{
    internal class LevelBuilder : IDisposable
    {
        private readonly EntityCreator<AreaType, IAreaData> _simpleAreaCreator;
        private readonly EntityCreator<int, IAreaData> _roadAreaCreator;
        private readonly List<IAreaData> _areasData;
        private readonly List<AreaType> _areaTypes;


        public LevelBuilder(EntityCreator<AreaType, IAreaData> simpleAreaCreator,
            EntityCreator<int, IAreaData> roadAreaCreator)
        {
            _simpleAreaCreator = simpleAreaCreator;
            _roadAreaCreator = roadAreaCreator;
            _areasData = new List<IAreaData>();
            _areaTypes = new List<AreaType>();
        }

        public void Dispose()
        {
            _simpleAreaCreator.Dispose();
            _roadAreaCreator.Dispose();
            _areasData.Clear();
            _areaTypes.Clear();
        }

        public void AddAAreas(IEnumerable<AreaType> areaTypes)
        {
            _areaTypes.AddRange(areaTypes);
        }

        public async UniTask<IReadOnlyList<IAreaData>> Build(CancellationToken token)
        {
            int index = 0;
            foreach (var areaType in _areaTypes)
            {
                switch (areaType)
                {
                    case AreaType.Free:
                    case AreaType.Start:
                    case AreaType.Final:
                        _areasData.Add(await _simpleAreaCreator.CreateEntityAsync<SimpleArea>(areaType, token));
                        break;
                    case AreaType.Road:
                        _areasData.Add(await _roadAreaCreator.CreateEntityAsync<RoadArea>(index, token));
                        break;
                    default:
                        break;
                }
                index++;
            }
            return _areasData;
        }


    }
}
