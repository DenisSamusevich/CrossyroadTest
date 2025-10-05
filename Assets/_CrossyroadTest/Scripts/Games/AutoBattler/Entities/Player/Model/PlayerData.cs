using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Model
{
    public class PlayerData : IPlayerData, IInitAsyncElement<ILevelData>
    {
        public bool IsDefeated { get; set; }
        public PlayerState State { get; set; }
        public string Name { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Endurance { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public IReadOnlyList<IFeatures> ReadOnlyFeatures { get => Features; }
        public List<IFeatures> Features { get; private set; }
        public int BaseDamage { get => Strength + Weapon.Damage; }
        public IWeapon Weapon { get; set; }
        public CharacterClass CharacterClass { get; set; }
        public int ReaverLevel { get; set; }
        public int WarriorLevel { get; set; }
        public int BarbarianLevel { get; set; }

        public UniTask InitAsyncElement(ILevelData levelData)
        {
            IsDefeated = false;
            State = PlayerState.WaitInput;
            Name = "Player";
            Strength = Random.Range(1, 4);
            Dexterity = Random.Range(1, 4);
            Endurance = Random.Range(1, 4);
            Health = 0;
            MaxHealth = Endurance;
            Features = new List<IFeatures>();
            return UniTask.CompletedTask;
        }
    }
}
