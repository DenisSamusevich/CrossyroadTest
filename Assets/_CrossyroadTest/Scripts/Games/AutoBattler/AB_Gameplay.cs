using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Features.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces.Data;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.ValueType;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Interfases;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Linq;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler
{
    public interface IDefendBehavior : IAddFeatures
    {
        UniTask SetDamage(int damage);
        UniTask MissAttacking();
    }

    public interface IAddFeatures
    {
        void AddEffect(IFeatures features);
    }

    public interface IAttackBehavior : IAddFeatures
    {
        UniTask TakeDamage(int damage, IDefendBehavior dealDamage);
        UniTask Miss(IDefendBehavior dealDamage);
    }

    public struct EndGameMessage { }
    public struct WinGameMessage { }

    public class AB_Gameplay : IAB_Gameplay
    {
        private IAsyncPublisher<EndGameMessage> _endGamePublisher;
        private IAsyncPublisher<WinGameMessage> _winGamePublisher;
        private readonly IAB_GameSession _gameSession;
        private int _currentEnemyIndex;

        public AB_Gameplay(IAsyncPublisher<EndGameMessage> endGamePublisher,
            IAsyncPublisher<WinGameMessage> winGamePublisher,
            IAB_GameSession gameSession)
        {
            _endGamePublisher = endGamePublisher;
            _winGamePublisher = winGamePublisher;
            _gameSession = gameSession;
        }

        public async UniTask GameCycle()
        {
            _currentEnemyIndex = 0;
            await _gameSession.PlayerModel.EntityBehavior.LevelUp();
            await _gameSession.PlayerModel.EntityBehavior.RestoreHealth();
            while (_currentEnemyIndex < _gameSession.EnemyModels.Count)
            {
                await _gameSession.EnemyModels[_currentEnemyIndex].EntityBehavior.ActivateForBattle();
                var battleResult = await StartBattle(_gameSession.PlayerModel, _gameSession.EnemyModels[_currentEnemyIndex]);
                if (battleResult == BattleResult.Fail)
                {
                    await _endGamePublisher.PublishAsync(new EndGameMessage());
                    return;
                }
                if (battleResult == BattleResult.Success)
                {
                    await _gameSession.PlayerModel.EntityBehavior.WinBattle();
                    await _gameSession.PlayerModel.EntityBehavior.LevelUp();
                    await _gameSession.PlayerModel.EntityBehavior.RestoreHealth();
                    await _gameSession.PlayerModel.EntityBehavior.OfferWeapon(_gameSession.EnemyModels[_currentEnemyIndex].EntityData.Reward);
                    _currentEnemyIndex++;
                }
            }
            await _winGamePublisher.PublishAsync(new WinGameMessage());
        }

        private async UniTask<BattleResult> StartBattle(IEntityModel<IPlayerData, IPlayerBehavior> playerModel, IEntityModel<IEnemyData, IEnemyBehavior> enemyModel)
        {
            var turn = 0;
            var maxRandomAccuracy = playerModel.EntityData.Dexterity + enemyModel.EntityData.Dexterity;
            if (playerModel.EntityData.Dexterity < enemyModel.EntityData.Dexterity)
            {
                await TakeDamage(maxRandomAccuracy, enemyModel, playerModel, turn % 2);
                if (playerModel.EntityData.IsDefeated) return BattleResult.Fail;
                await UniTask.WaitForSeconds(1f);
                turn++;
            }
            while (true)
            {
                await TakeDamage(maxRandomAccuracy, playerModel, enemyModel, turn % 2);
                if (enemyModel.EntityData.IsDefeated) return BattleResult.Success;
                await UniTask.WaitForSeconds(1f);
                turn++;
                await TakeDamage(maxRandomAccuracy, enemyModel, playerModel, turn % 2);
                if (playerModel.EntityData.IsDefeated) return BattleResult.Fail;
                await UniTask.WaitForSeconds(1f);
                turn++;
            }
        }

        private async UniTask TakeDamage(int maxAccuracy, IEntityModel<ICharacterData, IAttackBehavior> attacker, IEntityModel<ICharacterData, IDefendBehavior> defender, int countTurn)
        {
            if (UnityEngine.Random.Range(0, maxAccuracy) >= defender.EntityData.Dexterity)
            {
                var attackFeatures = attacker.EntityData.ReadOnlyFeatures.Where(f => f is IAttackFeatures).Cast<IAttackFeatures>();
                var attackDefendFeatures = attacker.EntityData.ReadOnlyFeatures.Where(f => f is IAttackDefendFeatures).Cast<IAttackDefendFeatures>();
                var defendFeatures = defender.EntityData.ReadOnlyFeatures.Where(f => f is IDefendFeatures).Cast<IDefendFeatures>();
                var defendAttackFeatures = defender.EntityData.ReadOnlyFeatures.Where(f => f is IDefendAttackFeatures).Cast<IDefendAttackFeatures>();
                var damage = attacker.EntityData.BaseDamage;
                var debug = $"{attacker.EntityData.Name} BaseDamage {damage}\n";
                foreach (var features in attackFeatures)
                {
                    damage = features.ModifyAttack(damage, attacker, countTurn);
                    debug += $"{features.GetType().Name} - {damage}\n";
                }
                foreach (var features in attackDefendFeatures)
                {
                    damage = features.ModifyAttack(damage, attacker, defender, countTurn);
                    debug += $"{features.GetType().Name}  - {damage}\n";
                }
                foreach (var features in defendFeatures)
                {
                    damage = features.ModifyProtection(damage, defender, countTurn);
                    debug += $"{features.GetType().Name}  - {damage}\n";
                }
                foreach (var features in defendAttackFeatures)
                {
                    damage = features.ModifyProtection(damage, defender, attacker, countTurn);
                    debug += $"{features.GetType().Name} - {damage}";
                }
                Debug.Log(debug + $"{defender.EntityData.Name}");
                damage = Math.Max(damage, 0);
                await attacker.EntityBehavior.TakeDamage(damage, defender.EntityBehavior);
            }
            else
            {
                await UniTask.WhenAll(
                    attacker.EntityBehavior.Miss(defender.EntityBehavior),
                    defender.EntityBehavior.MissAttacking()
                    );
            }
        }
    }
}
