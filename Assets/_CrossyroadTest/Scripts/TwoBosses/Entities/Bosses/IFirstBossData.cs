using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Bosses
{
    internal interface IFirstBossData
    {
        bool IsDefeated { get; }
        Vector2Int BossPosition { get; }
    }
}
