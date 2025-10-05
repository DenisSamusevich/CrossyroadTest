using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.Services.EntityService;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player
{
    using AEC_Enemy = AsyncEntityCharacteristic<ILevelData, PlayerData, IPlayerData, PlayerBehavior, IPlayerBehavior>;
    public class Player : AEC_Enemy.AsyncEntity
    {
        public Player(AsyncEntityFactory entityFactory) : base(entityFactory)
        {
        }
    }
}
