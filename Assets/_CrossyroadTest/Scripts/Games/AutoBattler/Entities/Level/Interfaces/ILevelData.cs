using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using System.Collections.Generic;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces
{
    public interface ILevelData
    {
        IReadOnlyList<EnemyCharacteristics> LevelEnemies { get; }
    }
}
