using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Model
{
    public class EnemyWeapon : IWeapon
    {
        public EnemyWeapon(DamageType damageType, int damage)
        {
            DamageType = damageType;
            Damage = damage;
        }

        public DamageType DamageType { get; private set; }
        public int Damage { get; private set; }
    }

    public class EnemyData : IEnemyData, IInitAsyncElement<(int indexEnemy, ILevelData levelData)>
    {
        private FeaturesFactory _featuresFactory;

        public EnemyData(FeaturesFactory featuresFactory)
        {
            _featuresFactory = featuresFactory;
            Features = new List<IFeatures>();
        }

        public int Id { get; private set; }
        public bool IsDefeated { get; set; }
        public EnemyType EnemyType { get; set; }
        public string Name { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Endurance { get; set; }
        public int Health { get; set; }
        IReadOnlyList<IFeatures> IHaveFeatures.ReadOnlyFeatures { get => Features; }
        public int BaseDamage { get => Strength + Weapon.Damage; }
        public IWeapon Weapon { get; set; }
        public Weapon Reward { get; set; }
        public List<IFeatures> Features { get; private set; }


        public UniTask InitAsyncElement((int indexEnemy, ILevelData levelData) data)
        {
            Id = data.indexEnemy;
            IsDefeated = false;
            var enemy = data.levelData.LevelEnemies[data.indexEnemy];
            EnemyType = enemy.EnemyType;
            Name = enemy.Name;
            Strength = enemy.Strength;
            Dexterity = enemy.Dexterity;
            Endurance = enemy.Endurance;
            Health = enemy.Health;
            Features.Add(_featuresFactory.GetFeatures(enemy.Ability));
            Weapon = new EnemyWeapon(DamageType.Slashing, enemy.BaseDamage);
            Reward = enemy.Reward;

            return UniTask.CompletedTask;
        }
    }
}
