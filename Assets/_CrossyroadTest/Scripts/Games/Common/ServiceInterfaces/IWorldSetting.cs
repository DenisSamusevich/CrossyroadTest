using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces
{
    public interface IWorldSetting
    {
        Vector2 WorldSize { get; }

        bool IsDestroyAfterGame { get; }

        bool IsUsedRepeatedly { get; }
    }
}
