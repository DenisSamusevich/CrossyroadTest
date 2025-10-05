using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Enemy.Interfaces;
using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Level.Interfaces;
using Assets._CrossyroadTest.Scripts.Services.EntityService;
using Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using Assets._CrossyroadTest.Scripts.View.Common.Services.Interfaces;
using Cysharp.Threading.Tasks;
using LitMotion;
using MessagePipe;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.AutoBattler
{
    using CreateEnemyCommand = AsyncEntityService<(int indexEnemy, ILevelData levelData), Enemy, IEnemyData, IEntityModel<IEnemyData, IEnemyBehavior>>.CreateEntityCommand;
    using DisposeEnemyCommand = AsyncEntityService<(int indexEnemy, ILevelData levelData), Enemy, IEnemyData, IEntityModel<IEnemyData, IEnemyBehavior>>.DisposeEntityCommand;

    internal class EnemyView : MonoBehaviour
    {
        private readonly Vector3 EnemyPosition = new Vector3(0, 0, 6);
        private IAsyncSubscriber<CreateEnemyCommand> _createEnemyPublisher;
        private ISubscriber<DisposeEnemyCommand> _disposeEnemyPublisher;
        private IAB_AssetsSettings _assetsSettings;
        private IWorldObjectService _worldObjectService;
        private IDisposable _disposed;
        private Dictionary<int, EnemyBehaviorView> _enemies = new Dictionary<int, EnemyBehaviorView>();

        [Inject]
        public void Initialize(IAsyncSubscriber<CreateEnemyCommand> createEnemyPublisher,
            ISubscriber<DisposeEnemyCommand> disposeEnemyPublisher,
            IAB_AssetsSettings assetsSettings,
            IWorldObjectService worldObjectService)
        {
            _createEnemyPublisher = createEnemyPublisher;
            _disposeEnemyPublisher = disposeEnemyPublisher;
            _assetsSettings = assetsSettings;
            _worldObjectService = worldObjectService;
            var bag = DisposableBag.CreateBuilder();
            _createEnemyPublisher.Subscribe(CreateEnemy).AddTo(bag);
            _disposeEnemyPublisher.Subscribe(DestroyEnemy).AddTo(bag);
            _disposed = bag.Build();
        }

        private void OnDestroy()
        {
            _createEnemyPublisher = null;
            _disposeEnemyPublisher = null;
            _assetsSettings = null;
            _worldObjectService = null;
            _disposed?.Dispose();
        }

        private async UniTask CreateEnemy(CreateEnemyCommand command, CancellationToken token)
        {
            var enemyBehavior = _worldObjectService.GetComponent(_assetsSettings.EnemyBehavior,
                _worldObjectService.CurrentWorld.ZeroPosition + EnemyPosition,
                Quaternion.identity, transform);
            enemyBehavior.SetEnemyData(command.EntityModel.EntityData);
            _enemies.Add(command.EntityModel.EntityData.Id, enemyBehavior);
        }

        private void DestroyEnemy(DisposeEnemyCommand command)
        {
            foreach (var enemy in _enemies)
            {
                enemy.Value.DisposeEnemy();
            }
            if (_worldObjectService.CurrentWorld.IsDestroyAfterGame || _worldObjectService.CurrentWorld.IsUsedRepeatedly)
            {
                foreach (var enemy in _enemies)
                {
                    _worldObjectService.ReturnComponent(_assetsSettings.EnemyBehavior, enemy.Value);

                }
            }
            _enemies.Clear();
        }
    }
}
