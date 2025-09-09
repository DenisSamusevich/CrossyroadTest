using Assets._CrossyroadTest.Scripts.Common.Creators;
using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Bosses;
using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Level;
using Assets._CrossyroadTest.Scripts.View.TwoBosses.Boss;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MessagePipe;
using System;
using System.Threading;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses.Creators
{
    internal abstract class BossCreator<TBoss, TBossData> : MonoBehaviour where TBoss : BossBase<TBoss>
    {
        [SerializeField] private BossView<TBoss, TBossData> _bossView;
        private BossView<TBoss, TBossData> _currentBoss;
        private IObjectResolver _objectResolver;
        private IAsyncSubscriber<EntityCreator<ILevelData, TBossData>.CreateEntityCommand<TBossData>> _createPlayerPublisher;
        private ISubscriber<EntityCreator<ILevelData, TBossData>.DisposeEntityCommand<TBossData>> _disposePlayerPublisher;
        private IDisposable _disposed;

        [Inject]
        public void Initialize(IObjectResolver objectResolver,
            IAsyncSubscriber<EntityCreator<ILevelData, TBossData>.CreateEntityCommand<TBossData>> createPlayerPublisher,
            ISubscriber<EntityCreator<ILevelData, TBossData>.DisposeEntityCommand<TBossData>> disposePlayerPublisher)
        {
            _objectResolver = objectResolver;
            _createPlayerPublisher = createPlayerPublisher;
            _disposePlayerPublisher = disposePlayerPublisher;
            var bag = DisposableBag.CreateBuilder();
            _createPlayerPublisher.Subscribe(CreateBoss).AddTo(bag);
            _disposePlayerPublisher.Subscribe(DestroyBoss).AddTo(bag);
            _disposed = bag.Build();
        }
        private void OnDestroy()
        {
            _disposed.Dispose();
        }

        private async UniTask CreateBoss(EntityCreator<ILevelData, TBossData>.CreateEntityCommand<TBossData> command, CancellationToken token)
        {
            var position = GetBossPosition(command.EntityData);
            if (_currentBoss == null)
            {
                _currentBoss = _objectResolver.Instantiate(_bossView, position, Quaternion.identity);
            }
            else
            {
                await LSequence.Create().Append(LMotion.Create(_currentBoss.transform.position, position, 2).WithEase(Ease.InQuint).BindToPosition(_currentBoss.transform))
                    .Append(LMotion.Create(_currentBoss.transform.localScale, Vector3.one, 2).WithEase(Ease.InElastic).BindToLocalScale(_currentBoss.transform)).Run().ToUniTask();
            }
            _currentBoss.SetBossData(command.EntityData);
        }

        private void DestroyBoss(EntityCreator<ILevelData, TBossData>.DisposeEntityCommand<TBossData> command)
        {
            _currentBoss?.DisposeBoss();
        }

        protected abstract Vector3 GetBossPosition(TBossData bossData);
    }
}
