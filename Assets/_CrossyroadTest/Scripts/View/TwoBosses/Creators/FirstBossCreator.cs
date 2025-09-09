using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Bosses;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses.Creators
{
    internal class FirstBossCreator : BossCreator<FirstBoss, IFirstBossData>
    {
        [SerializeField] private LevelCreator _levelCreator;

        protected override Vector3 GetBossPosition(IFirstBossData bossData)
        {
            return _levelCreator.GetStartWorldPoint() + new Vector3(bossData.BossPosition.x, 0, bossData.BossPosition.y);
        }
    }
}
