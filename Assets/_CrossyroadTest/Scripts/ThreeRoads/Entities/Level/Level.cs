using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.Common.Interface;
using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level.Area;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level
{
    internal class Level : ILevelData, IUpdate, IEntityInit<IEnumerable<AreaType>>
    {
        private readonly LevelBuilder _levelBuilder;
        private IReadOnlyList<IUpdateArea> _areasForUpdate;

        public Level(LevelBuilder levelBuilder)
        {
            _levelBuilder = levelBuilder;
        }

        public IReadOnlyList<IAreaData> Areas { get; private set; }

        public void Dispose()
        {
            foreach (var area in Areas)
            {
                area.Dispose();
            }
            _levelBuilder.Dispose();
            _areasForUpdate = null;
        }

        public async UniTask InitAsync(IEnumerable<AreaType> initialData, CancellationToken token)
        {
            _levelBuilder.AddAAreas(initialData);
            Areas = await _levelBuilder.Build(token);
            _areasForUpdate = Areas.Where(a => a is IUpdateArea).Cast<IUpdateArea>().ToList();
        }

        public void Update()
        {
            foreach (var area in _areasForUpdate)
            {
                area.UpdateArea(Time.deltaTime);
            }
        }
    }
}
