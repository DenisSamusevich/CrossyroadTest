using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces
{
    public interface ITR_GameSession
    {
        IEntityModel<ILevelData, ILevelBehavior> LevelModel { get; }
        IEntityModel<IPlayerData, IPlayerBehavior> PlayerModel { get; }
    }
}
