using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Model;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses
{
    using AEC_Boss = AsyncEntityCharacteristic<ILevelData, BossData, IBossData, FirstBossBehavior, IBossBehavior>;

    internal class FirstBoss : AEC_Boss.AsyncEntity
    {
        public FirstBoss(AsyncEntityFactory entityFactory) : base(entityFactory)
        {
        }
    }
}
