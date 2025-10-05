using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Interfases;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses
{
    internal class TB_GameSession : ITB_GameSession
    {
        public IEntityModel<ILevelData> LevelModel { get; set; }
        public IEntityModel<IPlayerData, IPlayerBehavior> PlayerModel { get; set; }
        public IEntityModel<IBossData, IBossBehavior> FirstBossModel { get; set; }
        public IEntityModel<IBossData, IBossBehavior> SecondBossModel { get; set; }
    }
}
