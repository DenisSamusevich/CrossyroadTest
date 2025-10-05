using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Player
{
    using AEC_Player = AsyncEntityCharacteristic<ITR_LevelSettings, PlayerData, IPlayerData, PlayerBehavior, IPlayerBehavior>;

    public class Player : AEC_Player.AsyncEntity
    {
        public Player(AsyncEntityFactory entityFactory) : base(entityFactory)
        {
        }
    }
}
