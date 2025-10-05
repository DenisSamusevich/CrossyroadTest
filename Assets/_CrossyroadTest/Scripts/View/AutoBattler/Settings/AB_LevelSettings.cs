using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Interfases;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings
{
    public enum Weapon
    {
        None,
        Dagger,
        Club,
        Spear,
        Sword,
        Axe,
        LegendarySword,
    }

    public enum Ability
    {
        None,
        BoneForm,
        GelForm,
        DragonAttack,
        HidenAttack,
        ImpulsesToAction,
        Poison,
        PoisonAttack,
        Rage,
        Shield,
        StoneSkin,
    }

    public enum EnemyType
    {
        None,
        Goblin,
        Sceleton,
        Slime,
        Ghost,
        Golem,
        Dragon,
    }

    public enum CharacterClass
    {
        None,
        Reaver,
        Warrior,
        Barbarian,
    }

    public enum LevelBonus
    {
        HidenAttack,
        ImpulsesToAction,
        PoisonAttack,
        Rage,
        Shield,
        StoneSkin,
        Strength,
        Dexterity,
        Endurance,
    }

    [Serializable]
    public class EnemyCharacteristics : INameData, IHealth, IDamageData, IPersonData
    {
        [field: SerializeField] public EnemyType EnemyType { get; private set; }
        [field: SerializeField] public Color EnemyColor { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int Health { get; private set; }
        [field: SerializeField] public int BaseDamage { get; private set; }
        [field: SerializeField] public int Strength { get; private set; }
        [field: SerializeField] public int Dexterity { get; private set; }
        [field: SerializeField] public int Endurance { get; private set; }
        [field: SerializeField] public Ability Ability { get; private set; }
        [field: SerializeField] public Weapon Reward { get; private set; }
    }

    [Serializable]
    public class ClassCharacteristics
    {
        [field: SerializeField] public Color CharacterColor { get; private set; }
        [field: SerializeField] public CharacterClass CharacterClass { get; private set; }
        [field: SerializeField] public int BonusHealthOfLevel { get; private set; }
        [field: SerializeField] public Weapon StartWeapon { get; private set; }
        [SerializeField] private List<LevelBonus> _levelBonus;
        public IReadOnlyList<LevelBonus> LevelBonus { get => _levelBonus; }
    }

    [Serializable]
    public class WeaponCharacteristics : IWeapon
    {
        [field: SerializeField] public Weapon Weapon { get; private set; }
        [field: SerializeField] public DamageType DamageType { get; private set; }
        [field: SerializeField] public int Damage { get; private set; }
    }

    [CreateAssetMenu(fileName = "AB_LevelSettings", menuName = "GameSettings/AutoBattlerSettings/AB_LevelSettings")]
    public class AB_LevelSettings : ScriptableObject, IAB_LevelSettings, IStartable
    {
        public Vector2 WorldSize => new Vector2(10, 10);
        [field: SerializeField] public bool IsDestroyAfterGame { get; private set; }
        [field: SerializeField] public bool IsUsedRepeatedly { get; private set; }
        [field: SerializeField] public int CountEnemy { get; private set; }
        [SerializeField] private List<EnemyCharacteristics> _enemyList;
        public IReadOnlyDictionary<EnemyType, EnemyCharacteristics> EnemyDictionary { get; private set; }
        [SerializeField] private List<ClassCharacteristics> _classList;
        public IReadOnlyDictionary<CharacterClass, ClassCharacteristics> ClassDictionary { get; private set; }

        [SerializeField] private List<WeaponCharacteristics> _weapomList;
        public IReadOnlyDictionary<Weapon, WeaponCharacteristics> WeaponDictionary { get; private set; }


        public void Start()
        {
            EnemyDictionary = _enemyList.ToDictionary(e => e.EnemyType);
            ClassDictionary = _classList.ToDictionary(c => c.CharacterClass);
            WeaponDictionary = _weapomList.ToDictionary(c => c.Weapon);
        }
    }
}
