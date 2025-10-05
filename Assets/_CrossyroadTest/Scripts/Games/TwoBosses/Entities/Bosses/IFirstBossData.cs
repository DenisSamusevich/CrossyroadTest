using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses
{
    internal interface IFirstBossData
    {
        bool IsDefeated { get; }
        Vector2Int BossPosition { get; }
    }
}
