using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using System.Collections.Generic;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Interfases
{
    public interface IAB_GameSession
    {
        public IEntityModel<ILevelData> LevelModel { get; }
        public IEntityModel<IPlayerData, IPlayerBehavior> PlayerModel { get; }
        public IReadOnlyList<IEntityModel<IEnemyData, IEnemyBehavior>> EnemyModels { get; }
    }
}
