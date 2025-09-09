using System;

namespace Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Level.Area
{
    internal interface IAreaData : IDisposable
    {
        public AreaType AreaType { get; }
    }
}
