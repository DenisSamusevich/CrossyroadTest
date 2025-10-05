using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads
{
    public class TR_GameSession : ITR_GameSession
    {
        public IEntityModel<ILevelData, ILevelBehavior> LevelModel { get; set; }
        public IEntityModel<IPlayerData, IPlayerBehavior> PlayerModel { get; set; }
    }
}
