using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Interfases;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System.Collections.Generic;


namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Model
{
    public struct LevelUpRequest
    {
        public List<(CharacterClass Class, LevelBonus Bonus)> Variants { get; set; }
    }

    public struct LevelUpResponse
    {
        public CharacterClass SelectedClass;
    }

    public struct OfferWeaponRequest
    {
        public Weapon WeaponType { get; set; }
    }

    public struct OfferWeaponResponse
    {
        public bool IsPickUpWeapon { get; set; }
    }

    public enum PlayerCommand
    {
        TakeDamage,
        SetDamage,
        Defeated,
        Miss,
        SelectClass,
        LevelUp,
        Win,
        MissAttacking,
        RestoreHealth,
    }
    public struct BehaviorCommand { }

    public class PlayerBehavior : IPlayerBehavior, IInitAsyncElement<ILevelData, PlayerData>
    {
        private PlayerData _playerData;
        private ILevelData _levelData;
        private readonly IAsyncRequestHandler<LevelUpRequest, LevelUpResponse> _levelUpHandler;
        private readonly IAsyncRequestHandler<OfferWeaponRequest, OfferWeaponResponse> _offerWeapomHandler;
        private readonly IAsyncPublisher<PlayerCommand, BehaviorCommand> _behaviorCommandPublisher;
        private readonly FeaturesFactory _featuresFactory;
        private readonly IAB_LevelSettings _levelSettings;

        public PlayerBehavior(
            IAsyncRequestHandler<LevelUpRequest, LevelUpResponse> levelUpHandler,
            IAsyncRequestHandler<OfferWeaponRequest, OfferWeaponResponse> offerWeapomHandler,
            IAsyncPublisher<PlayerCommand, BehaviorCommand> behaviorCommandPublisher,
            FeaturesFactory featuresFactory,
            IAB_LevelSettings levelSettings
            )
        {
            _levelUpHandler = levelUpHandler;
            _offerWeapomHandler = offerWeapomHandler;
            _behaviorCommandPublisher = behaviorCommandPublisher;
            _featuresFactory = featuresFactory;
            _levelSettings = levelSettings;
        }

        public UniTask InitAsyncElement(ILevelData levelData, PlayerData playerData)
        {
            _levelData = levelData;
            _playerData = playerData;
            return UniTask.CompletedTask;
        }

        public void AddEffect(IFeatures features)
        {
            _playerData.Features.Add(features);
        }

        public async UniTask WinBattle()
        {
            await _behaviorCommandPublisher.PublishAsync(PlayerCommand.Win, new BehaviorCommand());
        }

        public async UniTask LevelUp()
        {
            if ((_playerData.ReaverLevel + _playerData.WarriorLevel + _playerData.BarbarianLevel) < 3)
            {
                var request = new LevelUpRequest()
                {
                    Variants = new List<(CharacterClass Class, LevelBonus Bonus)>()
                {
                    GetCurrentLevelBonus(CharacterClass.Reaver, _playerData.ReaverLevel),
                    GetCurrentLevelBonus(CharacterClass.Warrior, _playerData.WarriorLevel),
                    GetCurrentLevelBonus(CharacterClass.Barbarian, _playerData.BarbarianLevel),
                }
                };
                var response = await _levelUpHandler.InvokeAsync(request);

                var bonus = _levelSettings.ClassDictionary[response.SelectedClass]
                    .LevelBonus[response.SelectedClass switch
                    {
                        CharacterClass.Reaver => _playerData.ReaverLevel,
                        CharacterClass.Warrior => _playerData.WarriorLevel,
                        CharacterClass.Barbarian => _playerData.BarbarianLevel,
                    }];
                if (_playerData.CharacterClass == CharacterClass.None)
                {
                    _playerData.CharacterClass = response.SelectedClass;
                    _playerData.Weapon = _levelSettings.WeaponDictionary[_levelSettings.ClassDictionary[_playerData.CharacterClass].StartWeapon];
                    await _behaviorCommandPublisher.PublishAsync(PlayerCommand.SelectClass, new BehaviorCommand());
                }
                AddCharacterLevel(response.SelectedClass);
                AddBonus(bonus);
            }

            _playerData.MaxHealth += _levelSettings.ClassDictionary[_playerData.CharacterClass].BonusHealthOfLevel + _playerData.Endurance;
            await _behaviorCommandPublisher.PublishAsync(PlayerCommand.LevelUp, new BehaviorCommand());

            (CharacterClass Class, LevelBonus Bonus) GetCurrentLevelBonus(CharacterClass characterClass, int classLevel)
            {
                return (characterClass, _levelSettings.ClassDictionary[characterClass].LevelBonus[classLevel]);
            }

            void AddBonus(LevelBonus bonus)
            {
                switch (bonus)
                {
                    case LevelBonus.HidenAttack:
                        AddEffect(_featuresFactory.GetFeatures(Ability.HidenAttack));
                        break;
                    case LevelBonus.ImpulsesToAction:
                        AddEffect(_featuresFactory.GetFeatures(Ability.ImpulsesToAction));
                        break;
                    case LevelBonus.PoisonAttack:
                        AddEffect(_featuresFactory.GetFeatures(Ability.PoisonAttack));
                        break;
                    case LevelBonus.Rage:
                        AddEffect(_featuresFactory.GetFeatures(Ability.Rage));
                        break;
                    case LevelBonus.Shield:
                        AddEffect(_featuresFactory.GetFeatures(Ability.Shield));
                        break;
                    case LevelBonus.StoneSkin:
                        AddEffect(_featuresFactory.GetFeatures(Ability.StoneSkin));
                        break;
                    case LevelBonus.Strength:
                        _playerData.Strength += 1;
                        break;
                    case LevelBonus.Dexterity:
                        _playerData.Dexterity += 1;
                        break;
                    case LevelBonus.Endurance:
                        _playerData.Endurance += 1;
                        break;
                    default:
                        break;
                }
            }

            void AddCharacterLevel(CharacterClass selectedClass)
            {
                switch (selectedClass)
                {
                    case CharacterClass.Reaver:
                        _playerData.ReaverLevel++;
                        break;
                    case CharacterClass.Warrior:
                        _playerData.WarriorLevel++;
                        break;
                    case CharacterClass.Barbarian:
                        _playerData.BarbarianLevel++;
                        break;
                    default:
                        break;
                }
            }
        }



        public async UniTask Miss(IDefendBehavior dealDamage)
        {
            await _behaviorCommandPublisher.PublishAsync(PlayerCommand.Miss, new BehaviorCommand());
        }

        public async UniTask OfferWeapon(Weapon weapon)
        {
            var request = new OfferWeaponRequest()
            {
                WeaponType = weapon,
            };
            var response = await _offerWeapomHandler.InvokeAsync(request);
            if (response.IsPickUpWeapon)
            {
                _playerData.Weapon = _levelSettings.WeaponDictionary[weapon];
            }
        }

        public async UniTask RestoreHealth()
        {
            _playerData.Health = _playerData.MaxHealth;
            await _behaviorCommandPublisher.PublishAsync(PlayerCommand.RestoreHealth, new BehaviorCommand());
        }

        public async UniTask SetDamage(int damage)
        {
            _playerData.Health -= damage;
            await _behaviorCommandPublisher.PublishAsync(PlayerCommand.SetDamage, new BehaviorCommand());
            if (_playerData.Health <= 0)
            {
                _playerData.IsDefeated = true;
                await _behaviorCommandPublisher.PublishAsync(PlayerCommand.Defeated, new BehaviorCommand());
                _playerData.State = PlayerState.None;
            }
        }

        public async UniTask TakeDamage(int damage, IDefendBehavior dealDamage)
        {
            await UniTask.WhenAll(
                _behaviorCommandPublisher.PublishAsync(PlayerCommand.TakeDamage, new BehaviorCommand()),
                dealDamage.SetDamage(damage)
            );
        }

        public async UniTask MissAttacking()
        {
            await _behaviorCommandPublisher.PublishAsync(PlayerCommand.MissAttacking, new BehaviorCommand());
        }
    }
}
