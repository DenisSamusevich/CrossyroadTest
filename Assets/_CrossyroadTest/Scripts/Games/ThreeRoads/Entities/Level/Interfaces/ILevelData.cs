using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Area.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles.Interfaces;
using System.Collections.Generic;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level.Interfaces
{
    public interface ILevelData
    {
        public IReadOnlyList<IAreaData> Areas { get; }
        public IReadOnlyList<IObstacleData> Obstacles { get; }
        public int LevelWidth { get; }
    }
}
