using Assets._CrossyroadTest.Scripts.Games.Common.ServiceInterfaces;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using System.Collections.Generic;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Interfases
{
    public interface IAB_LevelSettings : IWorldSetting
    {
        public int CountEnemy { get; }
        public IReadOnlyDictionary<EnemyType, EnemyCharacteristics> EnemyDictionary { get; }
        public IReadOnlyDictionary<CharacterClass, ClassCharacteristics> ClassDictionary { get; }
        public IReadOnlyDictionary<Weapon, WeaponCharacteristics> WeaponDictionary { get; }
    }
}
