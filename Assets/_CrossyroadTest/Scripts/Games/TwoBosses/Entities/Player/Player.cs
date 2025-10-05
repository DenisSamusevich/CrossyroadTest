using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.Services.EntityService;

namespace Assets._CrossyroadTest.Scripts.Games.TwoBosses.Entities.Player
{
    using AEC_Player = AsyncEntityCharacteristic<ILevelData, PlayerData, IPlayerData, PlayerBehavior, IPlayerBehavior>;

    public class Player : AEC_Player.AsyncEntity
    {
        public Player(AsyncEntityFactory entityFactory) : base(entityFactory)
        {
        }
    }
}
