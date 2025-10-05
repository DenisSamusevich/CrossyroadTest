using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area;
using System.Collections.Generic;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces
{
    public interface ITR_LevelSettings : IWorldSetting
    {
        public IReadOnlyList<AreaType> LevelAreas { get; }
        public int LevelWidth { get; }
    }
}
