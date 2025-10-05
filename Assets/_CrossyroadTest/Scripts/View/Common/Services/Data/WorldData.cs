using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.Common.Services.Data
{
    public class WorldData : IWorldData
    {
        public Vector3 ZeroPosition { get; set; }
        public bool IsDestroyAfterGame { get; set; }
        public bool IsUsedRepeatedly { get; set; }
    }
}
