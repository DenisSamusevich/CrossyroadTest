using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Model;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy
{
    using AEC_Enemy = AsyncEntityCharacteristic<(int indexEnemy, ILevelData levelData), EnemyData, IEnemyData, EnemyBehavior, IEnemyBehavior>;
    public class Enemy : AEC_Enemy.AsyncEntity
    {
        public Enemy(AsyncEntityFactory entityFactory) : base(entityFactory)
        {
        }
    }
}
