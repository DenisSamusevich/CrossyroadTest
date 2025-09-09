using Assets._CrossyroadTest.Scripts.TwoBosses.Entities.Bosses;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Threading;
using UnityEngine;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.TwoBosses.Boss
{
    internal abstract class BossView<TBoss, TBossData> : MonoBehaviour where TBoss : BossBase<TBoss>
    {
        private IAsyncSubscriber<BossBase<TBoss>.SetStateCommand<TBoss>> _setStateSubscriber;
        private IAsyncSubscriber<BossBase<TBoss>.CreatingObstacleCommand<TBoss>> _creatingObstacleSubscriber;
        private IAsyncSubscriber<BossBase<TBoss>.CreateDamageButtonCommand<TBoss>> _createDamageButtonSubscriber;
        private IDisposable _disposable;
        protected TBossData _bossData;

        [Inject]
        public void Initialize(IAsyncSubscriber<BossBase<TBoss>.SetStateCommand<TBoss>> setStateSubscriber,
            IAsyncSubscriber<BossBase<TBoss>.CreatingObstacleCommand<TBoss>> creatingObstacleSubscriber,
            IAsyncSubscriber<BossBase<TBoss>.CreateDamageButtonCommand<TBoss>> createDamageButtonSubscriber)
        {
            _setStateSubscriber = setStateSubscriber;
            _creatingObstacleSubscriber = creatingObstacleSubscriber;
            _createDamageButtonSubscriber = createDamageButtonSubscriber;
        }

        public void SetBossData(TBossData bossData)
        {
            _bossData = bossData;
            Init();
            var bag = DisposableBag.CreateBuilder();
            _setStateSubscriber.Subscribe(SetState).AddTo(bag);
            _creatingObstacleSubscriber.Subscribe(CreatingObstacle).AddTo(bag);
            _createDamageButtonSubscriber.Subscribe(CreateDamageButton).AddTo(bag);
            _disposable = bag.Build();
        }


        public void DisposeBoss()
        {
            _disposable?.Dispose();
        }
        protected abstract void Init();

        protected abstract UniTask CreateDamageButton(BossBase<TBoss>.CreateDamageButtonCommand<TBoss> command, CancellationToken token);

        protected abstract UniTask CreatingObstacle(BossBase<TBoss>.CreatingObstacleCommand<TBoss> command, CancellationToken token);

        protected abstract UniTask SetState(BossBase<TBoss>.SetStateCommand<TBoss> command, CancellationToken token);
    }
}
