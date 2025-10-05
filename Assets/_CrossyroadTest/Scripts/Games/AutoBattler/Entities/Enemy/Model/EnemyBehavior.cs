using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Common.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Cysharp.Threading.Tasks;
using MessagePipe;

namespace Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Model
{
    public enum EnemyCommand
    {
        TakeDamage,
        SetDamage,
        Defeated,
        Miss,
        Win,
        ActivateForBattle,
        MissAttacking,
    }

    public class EnemyBehavior : IEnemyBehavior, IInitAsyncElement<(int indexEnemy, ILevelData levelData), EnemyData>
    {
        private readonly IAsyncPublisher<(int, EnemyCommand), BehaviorCommand> _behaviorCommandPublisher;
        private EnemyData _enemyData;
        private ILevelData _levelData;

        public EnemyBehavior(IAsyncPublisher<(int, EnemyCommand), BehaviorCommand> behaviorCommandPublisher)
        {
            _behaviorCommandPublisher = behaviorCommandPublisher;
        }

        public UniTask InitAsyncElement((int indexEnemy, ILevelData levelData) data, EnemyData enemyData)
        {
            _levelData = data.levelData;
            _enemyData = enemyData;
            return UniTask.CompletedTask;
        }

        public void AddEffect(IFeatures features)
        {
            _enemyData.Features.Add(features);
        }

        public async UniTask ActivateForBattle()
        {
            await _behaviorCommandPublisher.PublishAsync((_enemyData.Id, EnemyCommand.ActivateForBattle), new BehaviorCommand());
        }

        public async UniTask Miss(IDefendBehavior dealDamage)
        {
            await _behaviorCommandPublisher.PublishAsync((_enemyData.Id, EnemyCommand.Miss), new BehaviorCommand());
        }

        public async UniTask SetDamage(int damage)
        {
            _enemyData.Health -= damage;
            await _behaviorCommandPublisher.PublishAsync((_enemyData.Id, EnemyCommand.SetDamage), new BehaviorCommand());
            if (_enemyData.Health <= 0)
            {
                _enemyData.IsDefeated = true;
                await _behaviorCommandPublisher.PublishAsync((_enemyData.Id, EnemyCommand.Defeated), new BehaviorCommand());
            }
        }

        public async UniTask TakeDamage(int damage, IDefendBehavior dealDamage)
        {
            await UniTask.WhenAll(
                _behaviorCommandPublisher.PublishAsync((_enemyData.Id, EnemyCommand.TakeDamage), new BehaviorCommand()),
                dealDamage.SetDamage(damage)
            );
        }

        public async UniTask MissAttacking()
        {
            await _behaviorCommandPublisher.PublishAsync((_enemyData.Id, EnemyCommand.MissAttacking), new BehaviorCommand());
        }
    }
}
