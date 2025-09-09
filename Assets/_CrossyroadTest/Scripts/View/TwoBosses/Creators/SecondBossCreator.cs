using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Bosses;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses.Creators
{
    internal class SecondBossCreator : BossCreator<SecondBoss, ISecondBossData>
    {
        [SerializeField] private LevelCreator _levelCreator;

        protected override Vector3 GetBossPosition(ISecondBossData bossData)
        {
            return _levelCreator.GetStartWorldPoint() + new Vector3(bossData.BossPosition.x, 0, bossData.BossPosition.y);
        }
    }
}
