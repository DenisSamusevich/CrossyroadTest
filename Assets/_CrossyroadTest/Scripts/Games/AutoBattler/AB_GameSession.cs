using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Interfases;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using System.Collections.Generic;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler
{
    public class AB_GameSession : IAB_GameSession
    {
        public IEntityModel<ILevelData> LevelModel { get; set; }
        public IEntityModel<IPlayerData, IPlayerBehavior> PlayerModel { get; set; }
        public IReadOnlyList<IEntityModel<IEnemyData, IEnemyBehavior>> EnemyModels { get; set; }
    }
}
