using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces.Data;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Interfaces
{
    public interface IEnemyData : ICharacterData, IGiveReward
    {
        int Id { get; }
        bool IsDefeated { get; }
        EnemyType EnemyType { get; }
    }
}
