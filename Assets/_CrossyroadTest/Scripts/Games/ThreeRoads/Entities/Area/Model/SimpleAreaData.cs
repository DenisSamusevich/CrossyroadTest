using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using System;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area.Model
{
    public class SimpleAreaData : IInitAsyncElement<(int areaPosition, int levelWidth, AreaType areaType)>, IAreaData
    {
        public AreaType AreaType { get; private set; }
        public int AreaPosition { get; private set; }
        public int LevelWidth { get; private set; }

        UniTask IInitAsyncElement<(int areaPosition, int levelWidth, AreaType areaType)>.InitAsyncElement((int areaPosition, int levelWidth, AreaType areaType) data)
        {
            AreaPosition = data.areaPosition;
            AreaType = data.areaType;
            LevelWidth = data.levelWidth;
            return UniTask.CompletedTask;
        }

        void IDisposable.Dispose() { AreaType = AreaType.None; }
    }
}
