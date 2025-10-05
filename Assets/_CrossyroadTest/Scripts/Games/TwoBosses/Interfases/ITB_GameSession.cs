using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Bosses.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Interfases
{
    public interface ITB_GameSession
    {
        public IEntityModel<ILevelData> LevelModel { get; }
        public IEntityModel<IPlayerData, IPlayerBehavior> PlayerModel { get; }
        public IEntityModel<IBossData, IBossBehavior> FirstBossModel { get; }
        public IEntityModel<IBossData, IBossBehavior> SecondBossModel { get; }
    }
}
