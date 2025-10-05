using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Interfaces
{
    public interface IBossData
    {
        bool IsDefeated { get; }
        Vector2Int BossPosition { get; }
    }
}
