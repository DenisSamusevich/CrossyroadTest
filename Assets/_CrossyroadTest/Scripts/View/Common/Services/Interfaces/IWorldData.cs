using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces
{
    public interface IWorldData
    {
        Vector3 ZeroPosition { get; }
        bool IsDestroyAfterGame { get; }
        bool IsUsedRepeatedly { get; }
    }
}
