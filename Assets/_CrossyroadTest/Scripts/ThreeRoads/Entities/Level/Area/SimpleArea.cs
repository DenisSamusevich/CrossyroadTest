using Assets._CrossyroadTest.Scripts.Common.Creators;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level.Area
{
    internal class SimpleArea : IAreaData, IEntityInit<AreaType>
    {
        public AreaType AreaType { get; private set; }

        public void Dispose()
        {
            AreaType = AreaType.None;
        }

        public UniTask InitAsync(AreaType areaType, CancellationToken cancellationToken)
        {
            AreaType = areaType;
            return UniTask.CompletedTask;
        }
    }
}
