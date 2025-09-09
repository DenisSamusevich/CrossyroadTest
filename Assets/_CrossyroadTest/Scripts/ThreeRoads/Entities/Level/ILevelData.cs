using Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level.Area;
using System;
using System.Collections.Generic;

namespace Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level
{
    internal interface ILevelData : IDisposable
    {
        public IReadOnlyList<IAreaData> Areas { get; }
    }
}
