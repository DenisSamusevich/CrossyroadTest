using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Model;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses
{
    using AEC_Boss = AsyncEntityCharacteristic<ILevelData, BossData, IBossData, SecondBossBehavior, IBossBehavior>;

    internal class SecondBoss : AEC_Boss.AsyncEntity
    {
        public SecondBoss(AsyncEntityFactory entityFactory) : base(entityFactory)
        {
        }
    }
}
